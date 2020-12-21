using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Acre.Backend.Ons.Models.Case
{
    public class Case
    {
        public Guid CaseId { get; set; }
        public List<Client> Clients { get; set; }
        public string AssociatedPostcode => Clients.Select(c => c.Details.Address.Postcode).First();
        public bool HasVariousAges => AssociatedAges.Distinct().Count() > 1;
        public bool HasVaryingPostcodes => AssociatedPostcodes.Distinct().Count() > 1;
        public int[] AssociatedAges => Clients.Select(c => c.GetAge).ToArray();
        public string[] AssociatedPostcodes => Clients.Select(c => c.Details.Address.Postcode).ToArray();
    }
}