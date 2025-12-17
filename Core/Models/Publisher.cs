using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;

public class Publisher : ISerializable
{
    public string Id { get; set; }
    public string Name { get; set; }
    public Author Manager { get; set; }
    public List<Author> Authors { get; set; }
    public List<Book> Books { get; set; }
    public Publisher()
    {
        Books = new List<Book>();
    }
    public Publisher(string id,string name,Author manager) {
    this.Id = id;
        this.Name = name;  
        this.Manager = manager;
          
    }
        

    public void FromCSV(string[] values)
    {
        throw new NotImplementedException();
    }

    public string[] ToCSV()
    {
        throw new NotImplementedException();
    }

}
