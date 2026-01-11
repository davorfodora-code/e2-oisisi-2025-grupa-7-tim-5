using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;

public class WishList : ISerializable
{
    public string VisitorMemberShipNumber {  get; set; }
    public string VistorName { get; set; }
    public string VisitorLastName { get; set; }
    public string BookIsbn { get; set; }
    public string BookName { get; set; }

    public string[] ToCSV()
    {
        string[] Values =
        {
            VisitorMemberShipNumber,
            VistorName,
            VisitorLastName,
            BookIsbn,
            BookName
        };
        return Values;
    }

    public void FromCSV(string[] values)
    {
        VisitorMemberShipNumber = values[0];
        VistorName = values[1];
        VisitorLastName = values[2];
        //Address = new Address {  }
        BookIsbn = values[3];
        BookName = values[4];

    }

}

