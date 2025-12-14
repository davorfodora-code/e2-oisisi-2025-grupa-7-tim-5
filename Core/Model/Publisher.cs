using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;

public class Publisher : ISerializable
{
	public Publisher()
	{

        public string Id { get; set; }
        public string Name { get; set; }
        public Author Manager { get; set; }
        public list<Author> Authors { get; set; }
        public list<Book> Books { get; set; }

    }
}
