using System;
using System.Collections.Generic;

namespace Acre.Backend.Ons.Models.Case
{
    public class Client
    {
        public Guid ClientId { get; set; }
        public ClientDetails Details { get; set; }
        public int GetAge => (int)((DateTime.UtcNow - Details.Dob).Days / 365.2425M);
    }
}