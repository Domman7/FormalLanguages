using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace FormalLanguages
{
    class Automaton
    {
        protected Dictionary<int, Dictionary<char, int>> delthaF;
        protected List<char> alphabet;
        protected int length;
        protected int amount;
        protected int initial;
        protected List<int> final;

        public Automaton(string fileName)
        {
            string[] replace = fileName.Split("\\");
            String.Join("/", replace);
            delthaF = new Dictionary<int, Dictionary<char, int>>();
            alphabet = new List<char>();
            final = new List<int>();

            using (StreamReader file = new StreamReader(fileName))
            {
                if (file.Peek() != -1) //read alphabet
                {
                    var line = file.ReadLine().Split('|', StringSplitOptions.RemoveEmptyEntries);
                    length = int.Parse(line[0]);
                    foreach (var item in line[1])
                        alphabet.Add(item);
                }

                if (file.Peek() != -1) //read amount of statuses
                    amount = int.Parse(file.ReadLine());

                for (int i = 0; i < amount; i++) //read statuses
                {
                    string line = file.ReadLine();
                    string[] mas = line.Split('|', StringSplitOptions.RemoveEmptyEntries);
                    Dictionary<char, int> temp = new Dictionary<char, int>();
                    for (int j = 1; j < mas.Length; j += 2)
                    {
                        temp.Add(char.Parse(mas[j]), int.Parse(mas[j + 1]));
                    }
                    delthaF.Add(int.Parse(mas[0]), temp);
                }

                if (file.Peek() != -1) //read initial status
                {
                    initial = int.Parse(file.ReadLine());
                }

                if (file.Peek() != -1) //read final statuses
                {
                    var line = file.ReadLine().Split('|', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in line)
                        final.Add(int.Parse(item));
                }
            }
        }

        public void Show()
        {
            Console.Write("  \t");
            foreach(var item in alphabet)
            {
                Console.Write(item + "\t");
            }
            Console.WriteLine();

            foreach(var item in delthaF)
            {
                if (item.Key == initial && final.Contains(item.Key))
                    Console.Write("*>" + item.Key + "\t");
                else
                    if (item.Key == initial)
                    Console.Write("->" + item.Key + "\t");
                else
                    if (final.Contains(item.Key))
                    Console.Write(" *" + item.Key + "\t");
                else
                    Console.Write("  " + item.Key + "\t");

                foreach (var ch in alphabet)
                {
                    if (item.Value.ContainsKey(ch))
                        Console.Write(item.Value[ch] + "\t");
                    else
                        Console.Write("-\t");
                }

                Console.WriteLine();
            }
        }

        public void Read(string word)
        {
            int t = 0;
            int status = initial;

            foreach(var ch in word)
            {
                status = delthaF[status][ch];
                Console.Write(t + ") " + ch + ", " + status);
                t++;
                Console.WriteLine();
            }
        }
    }
}
