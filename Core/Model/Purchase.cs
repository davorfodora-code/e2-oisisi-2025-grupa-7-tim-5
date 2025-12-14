using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

public class Purchase : ISerializable
{
	public Purchase()
	{

        public Visitor Visitor { get; set; }
        public Book Book { get; set; }
        public string Date_of_purchase { get; set; }
        public int Review { get; set; }
        public string Comment { get; set; }
	}
}
