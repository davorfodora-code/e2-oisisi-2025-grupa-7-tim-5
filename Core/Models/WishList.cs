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

}

