using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO
{
    public class VisitorDTO
    {
        public string MembershipNumber { get; set; } = "";
        public string Name { get; set; } = "";
        public string LastName { get; set; } = "";
        public AddressDTO Address { get; set; } = new AddressDTO();
        public string Status { get; set; } = "P";
    }
}
