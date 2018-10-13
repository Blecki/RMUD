using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleFrontEnd
{
    class Program
    {
        static void Main(string[] args)
        {
            Game.Game.Begin(Console.Write);

            while (true)
                Game.Game.Input(Console.ReadLine());
        }
    }
}
