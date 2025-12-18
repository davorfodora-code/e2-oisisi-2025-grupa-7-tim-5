using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Serialization
{
    public class Storage<T> where T : ISerializable, new()
    {   
        private readonly string _fileName;
        private readonly Serializer<T> _serializer = new();

        public Storage(string fileName)
        {
            _fileName = Path.Combine(
            "..", "..", "..", "..",
            "Core", "Dao", "Data",
            fileName
        );
        }
        public List<T> Load()
        {
            if (!File.Exists(_fileName))
            {
                FileStream fs = File.Create(_fileName);
                fs.Close();
            }

            IEnumerable<string> lines = File.ReadLines(_fileName);
            List<T> objects = _serializer.FromCSV(lines);

            return objects;
        }

        public void Save(List<T> objects)
        {
            string serializedObjects = _serializer.ToCSV(objects);
            using (StreamWriter streamWriter = new StreamWriter(_fileName))
            {
                streamWriter.Write(serializedObjects);
            }
        }
    }
}
