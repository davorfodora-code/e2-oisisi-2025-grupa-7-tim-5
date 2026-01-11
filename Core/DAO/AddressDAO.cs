using Core.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Core.DAO
{
    public class AddressDAO
    {
        private readonly List<Address> addresses;
        private readonly Storage<Address> storage;
        private int nextId;

        public AddressDAO()
        {
            storage = new Storage<Address>("Addresses.txt");
            addresses = storage.Load();
       


            nextId = addresses.Any() ? addresses.Max(a => a.Id) + 1 : 1;
        }
       

        // Add address
        public Address AddAddress(Address a)
        {
            var existing = FindAddressByDetails(a);
            if (existing != null) return existing;

            a.Id = nextId++;
            addresses.Add(a);
            storage.Save(addresses);
            return a;
        }

      


        // Update address
        public Address? UpdateAddress(Address a)
        {
            Address? oldAddress = FindAddress(a.Id);
            if (oldAddress == null)
                return null;

            oldAddress.Street = a.Street;
            oldAddress.Number = a.Number;
            oldAddress.City = a.City;
            oldAddress.State = a.State;
            oldAddress.Country = a.Country;

            storage.Save(addresses);
            return oldAddress;
        }

        // Remove address
        public Address? RemoveAddress(int id)
        {
            Address? address = FindAddress(id);
            if (address == null)
                return null;

            addresses.Remove(address);
            storage.Save(addresses);
            return address;
        }

        // FindAddress by id
        private Address? FindAddress(int id)
        {
            return addresses.Find(a => a.Id == id);

        }
        private Address? FindAddressByDetails(Address a)
        {
            return addresses.FirstOrDefault(addr =>
                string.Equals(addr.Street, a.Street, StringComparison.OrdinalIgnoreCase) &&
                addr.Number == a.Number &&
                string.Equals(addr.City, a.City, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(addr.State, a.State, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(addr.Country, a.Country, StringComparison.OrdinalIgnoreCase)
            );
        }


   


        // Get all addresses

        public List<Address> GetAllAddresses()
        {
            return new List<Address>(addresses);
        }
    }

}
