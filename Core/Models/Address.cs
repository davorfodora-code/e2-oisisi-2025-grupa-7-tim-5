using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

public class Address : ISerializable
{

    public string street { set; get; }
    public string number { set; get; }
    public string city { set; get; }
    public string state { set; get; }
    public string country { set; get; }
    public Address()
    {
        
    }
    public string[] ToCSV()
    {
        throw new NotImplementedException();
    }

    public void FromCSV(string[] values)
    {
        throw new NotImplementedException();
    }

    public Address(string street,string number,string  city,string state,string country)
{
        this.street = street;
        this.number = number;
        this.city = city;
        this.state = state;
        this.country = country;

    }
public override string ToString()
{
    StringBuilder sb = new StringBuilder();
    sb.Append("ULICA " + street + " ");
    sb.Append("BROJ " + number + ", ");
    sb.Append("GRAD " + city + ", ");
    sb.Append("OPSTINA " + state + ", ");
    sb.Append("DRZAVA " + country);
    return sb.ToString();
}


}
