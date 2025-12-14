using System;


public interface ISerializable
{
    string[] ToCSV();

    void FromCSV(string[] values);
}


