using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvanceCsharp.Classes
{
    partial class PartialClass
    {
        private int id { get; set; }
        private string name { get; set; }
    }

    partial class PartialClass
    {
        List<PartialClass> partialClasses = new List<PartialClass>();
        public void add(int id ,string name)
        {
            partialClasses.Add(new PartialClass() { id = id , name = name });
        }
        public void view()
        {
            foreach (PartialClass partialClass in partialClasses)
            {
                Console.WriteLine("{0} {1}",partialClass.id,partialClass.name);
            }
        }
    }

    partial class a
    {
        public void oo()
        {
            Console.WriteLine("sa");
        }
    }

    partial class a
    {
        public void s()
        {
            oo();
        }
    }
}
