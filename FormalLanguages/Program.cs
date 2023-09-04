using System;

namespace FormalLanguages
{
    class Program
    {
        static void Main(string[] args)
        {
            var automaton = new Automaton(@"C:\Users\manasypoves\Desktop\Input.txt");
            automaton.Show();
            Console.WriteLine();
            automaton.Read("ab");
        }
    }
}
