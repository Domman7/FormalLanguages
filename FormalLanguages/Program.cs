using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace FormalLanguages
{
    class Program
    {
        static void Main(string[] args)
        {
            //var automaton = new Automaton(@"C:\Users\manasypoves\Desktop\DA.txt");
            //automaton.Show();
            //var automaton = new NDAutomaton(@"C:\Users\manasypoves\Desktop\NDA.txt");
            //Console.WriteLine("NDA");
            //automaton.Show();
            //Console.WriteLine();

            //var temp = new Automaton(automaton);
            //Console.WriteLine("KDA");
            //temp.Show();
            //Console.WriteLine();

            var eNDA = new eNDAutomaton(@"C:\Users\Egor\Desktop\eNDA.txt");
            eNDA.Show();
            Console.WriteLine();

            var NDA = new NDAutomaton(eNDA);
            NDA.Show();
            Console.WriteLine();

            //for (; ; )
            //{
            //    var word = Console.ReadLine();
            //    eNDA.Read(word);
            //}
        }
    }
}
