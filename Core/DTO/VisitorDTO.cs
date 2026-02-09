using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Core.DTO
{
    public class VisitorDTO
    {
        public string MembershipNumber { get; set; } = "";
        public string Name { get; set; } = "";
        public string LastName { get; set; } = "";
        public AddressDTO Address { get; set; } = new AddressDTO();
        public string Status { get; set; } = "R";

        public string DisplayAddress => Address?.DisplayAddress ?? "";

        public string DisplayStatus => Status == "R" ? "Regular" : "VIP";

        public (bool IsValid, string ErrorMessage) Validate()
        {
            if (string.IsNullOrWhiteSpace(MembershipNumber))
                return (false, "Membership number is required.");

            Regex membershipRegex = new Regex(@"^CK-\d-\d{4}$");
            if (!membershipRegex.IsMatch(MembershipNumber))
                return (false, "Membership number must be in format CK-#-####.");

            if (string.IsNullOrWhiteSpace(Name))
                return (false, "Name is required.");

            Regex nameRegex = new Regex(@"^[A-Za-zČĆŽŠĐčćžšđ\s]+$");
            if (!nameRegex.IsMatch(Name))
                return (false, "Name must contain only letters.");

            if (string.IsNullOrWhiteSpace(LastName))
                return (false, "Last name is required.");

            if (!nameRegex.IsMatch(LastName))
                return (false, "Last name must contain only letters.");

            var addressValid = Address.Validate();
            if (!addressValid.IsValid)
                return addressValid;

            if (Status != "R" && Status != "V")
                return (false, "Status must be 'R' or 'V'.");

            return (true, "");
        }

        public Visitor ToModel()
        {
            return new Visitor
            {
                MembershipNumber = this.MembershipNumber,
                Name = this.Name,
                LastName = this.LastName,
                Address = this.Address.ToModel(),
                Status = this.Status
            };
        }

        public static VisitorDTO FromModel(Visitor visitor)
        {
            return new VisitorDTO
            {
                MembershipNumber = visitor.MembershipNumber,
                Name = visitor.Name,
                LastName = visitor.LastName,
                Address = AddressDTO.FromModel(visitor.Address),
                Status = visitor.Status
            };
        }
    }
}
