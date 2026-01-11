using Core.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DAO
{
    public class AddressDAO
    {
        private readonly List<Address> addresses;
        private readonly Storage<Address> storage;

        public AddressDAO()
        {

            storage = new Storage<Address>("Addresses.txt");
            addresses = storage.Load();
        }

        // add address
        public Address AddAddress(Address a)
        {
            if(FindAddress(a.Id) != null)
            {
                return null;
            }
      
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
        public Address? RemoveAddress(Address a)
        {
            Address? address = FindAddress(a.Id);
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

        // Get all addresses

        public List<Address> GetAllAddresses()
        {
            return new List<Address>(addresses);
        }
    }

}
