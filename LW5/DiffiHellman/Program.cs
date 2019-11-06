using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffiHellman
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Node> x = new List<Node>();
            string[] names = { "Alice", "Bob", "Jack", "Ann", "Katy" };
            for (int i = 0; i < names.Length; i++)
            {
                x.Add(new Node(names[i]));
            }
            MultiDiffiHellman w = new MultiDiffiHellman(x, 9973,10);
            Console.WriteLine($"Parameters:\n\tP {w.P}\n\tq {w.q}");
            w.ExchangeOfGroup();
            bool q = w.CompareCommonKeys();
            Console.WriteLine("Common keys are "+((q)?"equal! :)":"not equal! :("));
        }
    }
}
