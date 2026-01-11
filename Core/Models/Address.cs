using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class Address : ISerializable
{
    public int Id { set; get; }
    public string Street { set; get; }
    public string Number { set; get; }
    public string City { set; get; }
    public string State { set; get; }
    public string Country { set; get; }
    public Address()
    {
        
    }
    public void FromCSV(string[] values)
    {
        
        Id = int.Parse(values[0]);
        Street = values[1];
        Number = values[2];
        City = values[3];
        State = values[4];
        Country = values[5];
    }

    public string[] ToCSV()
    {
        return new string[]
        {
        Id.ToString(), Street, Number, City, State, Country
        };
    }


    public Address( string street, string number, string city, string state, string country)
    {
        Street = street;
        Number = number;
        City = city;
        State = state;
        Country = country;
    }

    public override string ToString()
{
    StringBuilder sb = new StringBuilder();
    sb.Append("ULICA " + Street + " ");
    sb.Append("BROJ " + Number + ", ");
    sb.Append("GRAD " + City + ", ");
    sb.Append("OPSTINA " + State + ", ");
    sb.Append("DRZAVA " + Country);
    return sb.ToString();
}


}
