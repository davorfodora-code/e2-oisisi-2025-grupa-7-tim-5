using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

public class Address : ISerializable
{

    public string street { set; get; };
    public string number { set; get; };
    public string city { set; get; };
    public string state { set; get; };
    public string country { set; get; };
    public Adress()
	{
		private string street, number, city, state, country;
	
	}
    public Adress(string street,string number,string  city,string state,string country)
{
    street = street;
    number = number;
    city = city;
    state = state;
    country = country;

}
public override string ToString()
{
    StringBuilder sb = new StringBuilder();
    sb.Append("ULICA" + street + " ");
    sb.Append("BROJ" + number + ", ");
    sb.Append("GRAD" + city + ", ");
    sb.Append("OPSTINA" + state + ", ");
    sb.Append("DRZAVA" + country);
    return sb.ToString();
}


}
