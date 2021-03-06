The ONS data is initially parsed into relational database models which are then stored in a local sqlite database for querying. Each spreadsheet occupies its own table with each data entry being associated to a subcategory which in turn belongs to a main category.
Ex: “Medical products, appliances and equipment” is a subcategory of “6. Health” which is comprised of both “6.1 Medical products, appliances and equipment” and “6.2 Hospital services”
My idea was to have the application be as data driven as possible with minimal effort needed (or none at all) to allow the application to accommodate changes or extensions to the datasets.

**Note that minor tweaks to dataset's category names were made to keep them inline with the other documents:**
- ONSByAge2019: "Life assurance <b>&</b> contributions to pension funds" --> "Life assurance<b>,</b> contributions to pension funds"
- ONSByRegion2019: "Housing: mortgage interest payments council tax etc." --> Housing: mortgage interest payments<b>,</b> council tax etc."

The schema is as follows:

![Schema](https://i.imgur.com/cccoSqE.png)

Interactive version: https://dbdiagram.io/d/5fdd35ef9a6c525a03bba3e5 

## Usage:
_Requires .NET Core SDK 3.1+_

From within the **Acre.Backend.Ons folder**:

(1) Before being able to launch the application both `dotnet restore` and then `dotnet ef database update` must be run for installing dependencies and building the sqlite database respectively.

(2) To parse ONS datasets into the database, and then subsequently start the api server, run the following command: `dotnet run seed=true`

(3) The endpoint documentation can be viewed through the swagger document at https://localhost:5001/swagger whilst the server is running.

(4) Once the database has been built the server can be started on subsequent runs with just: `dotnet run` 

When making changes to the datasets the database must be rebuilt before those changes can be reflected in the application, this can be done by re-running command (2)

By default the application will return values based on the region of the postcode associated with a specific case, however to accommodate for the “Hybrid Values” extension outlined in the original spec, an optional parameter can be passed to the API endpoint to have it return the average value of those found in both the ONS by region and ONS by age datasets.
http://localhost:5000/api/case/{caseId}/outgoings/by/region<b>?useAge=true</b>

## Implementation:

- Written in C#
- Code first database with migrations generated by Entity Framework (Object Relational Mapper)
- Sqlite for a lightweight local database.
- EPPlus for reading and traversing .xlxs files
- Swashbuckle for swagger generation
- Integrates with [Postcode.io](https://postcodes.io/) API in order to map postcodes to regions.

I decided to use a [Strategy Pattern](https://refactoring.guru/design-patterns/strategy) approach for handling the parsing logic. Creating a ParserContext class with access to all the available DatasetParsers (defined as those implementing the IDatasetParser interface) that can then resolve the correct parser to be used for a given dataset spreadsheet based on its associated DocumentType (enum). These are configured within the appSettings.Development.Json file:

![Settings](https://i.imgur.com/RezONxm.png)

Each parser also defines its supported document types through a property SupportedDocumentTypes:

![Parser](https://i.imgur.com/U0VJhB2.png)

This is used by the ParserContext to resolve the correct strategy.

If in future additional spreadsheets were added with the same format as an existing document, i.e OnsByAge2020, then by simply extending the Ons json object above to include:

![Settings2](https://i.imgur.com/pNszNP0.png)

This data would be picked up and imported into the database when running the seed=true command without requiring any code changes.

If a new type of spreadsheet was added this implementation could be cleanly extended by adding a new DocumentType, creating an associated class implementing the IDatasetParser interface and then registering it in the .NET dependency injection container.

**Please note that the case.json file must contain a JSON array of case objects not a single JSON object. The application will log an error in this scenario but be aware in advance if you plan on plugging test case data into the case.json file.**

## Testing:
I wrote unit tests for the RegionLookupService that interfaces with the PostcodesIo REST API and the OnsRepository which queries the parsed datasets to calculate the outgoings given a certain client’s details such as region.
All network requests were mocked using stub data from JSON files to ensure the tests were deterministic in nature. The OnsRepository tests also run against an in-memory database seeded with static test data to achieve the aforementioned result.

The tests are contained within the Acre.Backend.Ons.Test project and can be run from within that folder using `dotnet test` after installing dependencies and building via  `dotnet build`.


## Potential Improvements (In hindsight):
1) When parsing data from the ONS datasets into the sqlite database only categories (i.e 1. Food & non-alcoholic drinks) and their immediate subcategories (i.e 1.1 Food) are included. Due to lack of granularity I wasn't able to retrieve the relevant information for outgoings_sports (9.4.1 Sports admissions, subscriptions, leisure class fees) and outgoings_television_licenses (9.4.3 TV, video, satellite rental, cable subscriptions) which is why they were omitted from the result.

I’ve built the application to be extensible such that if needed it could be relatively easily modified to accommodate categories within subcategories (i.e 1.1.1 Bread, rice and cereals) with a minor change to the relational database models and parsing script.

![ProposedRelationship1](https://i.imgur.com/n4gSEQM.png)

The above entity relationship could quickly snowball out of control depending on how deep the category nesting went on future documents.

An alternate idea could be a self referential table to allow for unlimited nesting of categories:

![ProposedRelationship2](https://i.imgur.com/CGAlVD5.png)

2) I wrote three parsers in total to accommodate for the three spreadsheets, however the the OnsByAge and OnsByRegion parsers are almost identical, and with some refactoring could be combined into a single shared parser. In this case the configuration file would need to contain additional data for each dataset to indicate the data model into which its data should be parsed.

3) Assuming this application would support subsequent years of ONS data then the Ons_by_x tables should have an added column to indicate the year or range of years from which the data was collected.

4) An improved solution, which would require knowing the average age of those surveyed in the ONS by region dataset, would allow us to do the following:

Scenario:

- Tom is 35 and lives in England.

- According to the regional dataset this should predict him with an outgoing water expenditure of around £9.80 per month.

- However if the average reported age that made up the regional dataset was 55, and we know from looking at the ONS by age dataset that those in the 50-64 age bracket (the median and therefore most influential bracket) typically spend 2.5% less on their water expenditure ( 100 - (9.6 / 9.8 x 100) ) than those in the 30-49 bracket we can assume Tom’s actual expenditure is slightly higher than the £9.80 shown in the ONS by region dataset.

- We could then use a weighted prediction towards 2.5% more of the value itself in order to accommodate for the above and predict it as £9.85 ( 9.8 + (9.8 x (0.025 / 5)) ) assuming a 20% weighting towards +2.5%. This weight could be adjusted based on confidence in this prediction method.

