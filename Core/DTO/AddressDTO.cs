using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Core.DTO
{
    public class AddressDTO
    {
        public string Street { get; set; } = "";
        public string Number { get; set; } = "";
        public string City { get; set; } = "";
        public string State { get; set; } = "";
        public string Country { get; set; } = "";

        public string DisplayAddress => $"{Street} {Number}, {City}";

        public (bool IsValid, string ErrorMessage) Validate()
        {
            if (string.IsNullOrWhiteSpace(Street))
                return (false, "Street is required.");

            Regex streetRegex = new Regex(@"^[A-Za-zČĆŽŠĐčćžšđ\s\.\-]+$");
            if (!streetRegex.IsMatch(Street))
                return (false, "Street must contain only letters.");

            if (string.IsNullOrWhiteSpace(Number))
                return (false, "Number is required.");

            if (string.IsNullOrWhiteSpace(City))
                return (false, "City is required.");

            Regex cityRegex = new Regex(@"^[A-Za-zČĆŽŠĐčćžšđ\s]+$");
            if (!cityRegex.IsMatch(City))
                return (false, "City must contain only letters.");

            return (true, "");
        }
    }
}
