using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class Address : ISerializable
{

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

        Street = values[0];
        Number = values[1];
        City = values[2];
        State = values[3];
        Country = values[4];
    }

    public string[] ToCSV()
    {
        string[] values =
        {
           Street, Number, City, State, Country
        };
        return values;
    }

    public Address(string street,string number,string  city,string state,string country)
{
        this.Street = street;
        this.Number = number;
        this.City = city;
        this.State = state;
        this.Country = country;

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
