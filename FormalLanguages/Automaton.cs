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

        public Automaton(NDAutomaton init)//NDA to KDA
        {
            Console.WriteLine("NDA -> KDA");

            var DelthaF = init.DelthaF;
            alphabet = init.Alphabet;
            length = init.Length;
            initial = init.Initial;

            List<List<int>> newStatuses = new List<List<int>>();
            List<int> temp = new List<int>();
            Queue<List<int>> statuses = new Queue<List<int>>();

            temp.Add(initial);
            statuses.Enqueue(temp);
            newStatuses.Add(temp);

            //-----------------------------
            Console.Write("  \t");
            foreach (var item in alphabet)
            {
                Console.Write(item + "\t");
            }
            Console.WriteLine();
            //-----------------------------

            while (statuses.Count != 0)
            {
                temp = statuses.Dequeue();

                //----------------------------
                foreach (var outs in temp)
                    Console.Write(outs + " ");
                Console.Write('\t');
                //----------------------------

                foreach (var ch in alphabet)
                {
                    var union = new List<int>();
                    foreach(var item in temp)
                    {
                        if (DelthaF[item].ContainsKey(ch))
                        {
                            foreach (var st in DelthaF[item][ch])
                            {
                                union.Add(st);
                            }
                        }
                    }
                    union.Sort();

                    bool flag = true;

                    if (union.Count == 0)
                    {
                        //-------------------
                        Console.Write("-\t");
                        //-------------------

                        flag = false;
                    }
                    else
                    {
                        //----------------------------
                        foreach (var outs in union)
                            Console.Write(outs + " ");
                        Console.Write('\t');
                        //----------------------------

                        foreach (var item in newStatuses)
                        {
                            if (Automaton.Equals(item, union))
                            {
                                flag = false;
                                break;
                            }
                        }
                    }

                    if (flag)
                    {
                        statuses.Enqueue(union);
                        newStatuses.Add(union);
                    }
                }
                Console.WriteLine();
            }

            Dictionary<string, int> newKeys = new Dictionary<string, int>();
            int counter = 0;
            foreach(var item in newStatuses)
            {
                newKeys[Automaton.ListToString(item)] = counter;
                counter++;
            }

            delthaF = new Dictionary<int, Dictionary<char, int>>();
            temp = new List<int>();
            temp.Add(init.Initial);

            final = new List<int>();

            foreach (var st in newStatuses)
            {
                bool finalFlag = false;
                var val = new Dictionary<char, int>();
                foreach (var ch in alphabet)
                {
                    var union = new List<int>();
                    foreach (var item in st)
                    {
                        if (DelthaF[item].ContainsKey(ch))
                        {
                            foreach (var stat in DelthaF[item][ch])
                            {
                                union.Add(stat);
                            }
                        }

                        if(init.Final.Contains(item))
                        {
                            finalFlag = true;
                        }
                    }
                    union.Sort();
                    if(union.Count != 0)
                        val[ch] = newKeys[Automaton.ListToString(union)];
                }

                int newKey = newKeys[Automaton.ListToString(st)];
                if(finalFlag)
                    final.Add(newKey);
                delthaF[newKey] = val;
            }

            //foreach (var item in newStatuses)
            //{
            //    foreach (var st in item)
            //    {
            //        Console.Write(st + " ");
            //    }
            //    Console.WriteLine();
            //}

            Console.WriteLine();
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
            bool accepted = true;

            Console.WriteLine("Word: " + word);
            foreach(var ch in word)
            {
                if (delthaF[status].ContainsKey(ch))
                {
                    status = delthaF[status][ch];
                    Console.Write(t + ") " + ch + ", " + status);
                    t++;
                    Console.WriteLine();
                }
                else
                {
                    accepted = false;
                    break;
                }
            }

            if (final.Contains(status) && accepted)
                Console.WriteLine("Word accepted");
            else
                Console.WriteLine("Word don't accepted");
            Console.WriteLine();
        }

        public static bool Equals(List<int> first, List<int> second)
        {
            if (first.Count != second.Count)
                return false;

            foreach (var node in first)
                if (!second.Contains(node))
                    return false;

            return true;
        }

        public static string ListToString(List<int> lst)
        {
            StringBuilder temp = new StringBuilder();
            if (lst.Count == 0)
                return "";
            else
                foreach (var item in lst)
                    temp.Append(item.ToString());
            return temp.ToString();
        }

    }
}
