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
        public DateOnly DateOfBirth { get; set; }           // ← NOVO
        public string ContactPhone { get; set; } = "";      // ← NOVO
        public string Email { get; set; } = "";             // ← NOVO
        public AddressDTO Address { get; set; } = new AddressDTO();
        public string Status { get; set; } = "R";
        public List<BookDTO> Wishlist { get; set; } = new List<BookDTO>();

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

            // Validacija datuma — ne sme biti u budućnosti
            if (DateOfBirth == default)
                return (false, "Date of birth is required.");
            if (DateOfBirth > DateOnly.FromDateTime(DateTime.Today))
                return (false, "Date of birth cannot be in the future.");

            // Telefon — opciono, ali ako postoji mora biti validan
            if (!string.IsNullOrWhiteSpace(ContactPhone))
            {
                Regex phoneRegex = new Regex(@"^[+\d\s\-\(\)]{6,20}$");
                if (!phoneRegex.IsMatch(ContactPhone))
                    return (false, "Contact phone is not valid.");
            }

            // Email — opciono, ali ako postoji mora biti validan
            if (!string.IsNullOrWhiteSpace(Email))
            {
                Regex emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                if (!emailRegex.IsMatch(Email))
                    return (false, "Email is not valid.");
            }

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
                DateOfBirth = this.DateOfBirth,
                ContactPhone = this.ContactPhone,
                Email = this.Email,
                Address = this.Address.ToModel(),
                Status = this.Status,
                Wishlist = this.Wishlist?.Select(b => b.ToModel()).ToList() ?? new List<Book>()
            };
        }

        public static VisitorDTO FromModel(Visitor visitor)
        {
            return new VisitorDTO
            {
                MembershipNumber = visitor.MembershipNumber,
                Name = visitor.Name,
                LastName = visitor.LastName,
                DateOfBirth = visitor.DateOfBirth,
                ContactPhone = visitor.ContactPhone ?? "",
                Email = visitor.Email ?? "",
                Address = AddressDTO.FromModel(visitor.Address),
                Status = visitor.Status,
                Wishlist = visitor.Wishlist?.Select(BookDTO.FromModel).ToList() ?? new List<BookDTO>()
            };
        }
    }
}