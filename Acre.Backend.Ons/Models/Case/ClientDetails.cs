using System;
using System.Collections.Generic;

namespace Acre.Backend.Ons.Models.Case
{
    public class ClientDetails
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Address Address { get; set; }
        public DateTime Dob { get; set; }
        public int Dependants { get; set; }
        public string EmploymentStatus { get; set; }
        public bool IsEmployed => EmploymentStatus == "EMPLOYED";
    }
}