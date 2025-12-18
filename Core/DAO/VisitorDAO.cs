using Core.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DAO
{
    public class VisitorDAO
    {
        private readonly List<Visitor> visitors;
        private readonly Storage<Visitor> storage;

        public VisitorDAO()
        {
            storage = new Storage<Visitor>("DataBase.txt");
            visitors = storage.Load();
        }

        public Visitor AddVisitor(Visitor visitor)
        {
            visitors.Add(visitor);
            storage.Save(visitors);
            return visitor;
        }

        public List<Visitor> ShowVisitors()
        {
            return visitors;
        }

    }
}
