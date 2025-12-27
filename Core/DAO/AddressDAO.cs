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

        // #dodavanje_adrese
        public Address AddAddress(Address address)
        {
            if(FindAddress(address) != null)
            {
                return null;
            }
      
            addresses.Add(address);
            storage.Save(addresses);
            return address;
        }

        // #izmena_adrese
        public Address? UpdateAddress(Address a)
        {
            Address? oldAddress = FindAddress(a);
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

        // #brisanje_adrese
        public Address? RemoveAddress(Address a)
        {
            Address? address = FindAddress(a);
            if (address == null)
                return null;

            addresses.Remove(address);
            storage.Save(addresses);
            return address;
        }

       
        private Address? FindAddress(Address a)
        {
            return addresses.Find(addr =>
                addr.Street == a.Street &&
                addr.Number == a.Number &&
                addr.City == a.City &&
                addr.State == a.State &&
                addr.Country == a.Country
            );
        }

        
        public List<Address> GetAllAddresses()
        {
            return new List<Address>(addresses);
        }
    }

}
