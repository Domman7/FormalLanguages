using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace FormalLanguages
{
    class eNDAutomaton
    {
        protected Dictionary<int, Dictionary<char, List<int>>> delthaF;
        protected List<char> alphabet;
        protected int length;
        protected int amount;
        protected int initial;
        protected List<int> final;

        public eNDAutomaton(string fileName)
        {
            string[] replace = fileName.Split("\\");
            String.Join("/", replace);
            delthaF = new Dictionary<int, Dictionary<char, List<int>>>();
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
                    alphabet.Add('\0');
                }

                if (file.Peek() != -1) //read amount of statuses
                    amount = int.Parse(file.ReadLine());

                for (int i = 0; i < amount; i++) //read statuses
                {
                    string line = file.ReadLine();
                    string[] mas = line.Split('|', StringSplitOptions.RemoveEmptyEntries);
                    Dictionary<char, List<int>> temp = new Dictionary<char, List<int>>();
                    for (int j = 1; j < mas.Length; j += 2)
                    {
                        List<int> variable = new List<int>();
                        foreach (var item in mas[j + 1].Split(','))
                            variable.Add(int.Parse(item));
                        if(!mas[j].Equals("\\0"))
                            temp.Add(char.Parse(mas[j]), variable);
                        else
                            temp.Add('\0', variable);
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

        public Dictionary<int, Dictionary<char, List<int>>> DelthaF
        {
            get
            {
                var temp = new Dictionary<int, Dictionary<char, List<int>>>(delthaF);
                return temp;
            }
        }

        public List<char> Alphabet
        {
            get
            {
                var temp = new List<char>(alphabet);
                return temp;
            }
        }

        public List<int> Final
        {
            get
            {
                var temp = new List<int>(final);
                return temp;
            }
        }

        public int Length
        {
            get
            {
                return length;
            }
        }

        public int Amount
        {
            get
            {
                return amount;
            }
        }

        public int Initial
        {
            get
            {
                return initial;
            }
        }

        public void Show()
        {
            Console.Write("  \t");
            foreach (var item in alphabet)
            {
                if (!item.Equals('\0')) 
                    Console.Write(item + "\t\t");
                else
                    Console.Write("eps" + "\t\t");
            }
            Console.WriteLine();

            foreach (var item in delthaF)
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
                    {
                        foreach (var variable in item.Value[ch])
                            Console.Write(variable + " ");
                        Console.Write("\t\t");
                    }
                    else
                        Console.Write("-\t\t");
                }

                Console.WriteLine();
            }
        }

        public void Read(string word)
        {
            int t = 0;
            SortedSet<int> curStatuses = new SortedSet<int>();
            curStatuses.Add(initial);
            bool accepted = true;

            Console.WriteLine("Word: " + word);
            foreach (var ch in word)
            {
                SortedSet<int> tempStatuses = new SortedSet<int>();
                foreach (var status in curStatuses)
                {
                    if (delthaF[status].ContainsKey('\0'))//eps move
                    {
                        foreach (var item in delthaF[status]['\0'])
                        {
                            tempStatuses.Add(item);
                            Console.Write(t + ") " + status + " " + "eps" + ", " + item);
                            t++;
                            Console.WriteLine();
                        }
                    }

                    if (delthaF[status].ContainsKey(ch))
                    {
                        foreach (var item in delthaF[status][ch])
                        {
                            tempStatuses.Add(item);
                            Console.Write(t + ") " + status + " " + ch + ", " + item);
                            t++;
                            Console.WriteLine();
                        }
                    }
                }
                if (tempStatuses.Count == 0)
                {
                    accepted = false;
                    break;
                }
                curStatuses = tempStatuses;
            }

            foreach (var item in curStatuses)
                if (!final.Contains(item))
                    accepted = false;

            if (accepted)
                Console.WriteLine("Word accepted");
            else
                Console.WriteLine("Word don't accepted");
            Console.WriteLine();
        }
    }
}
