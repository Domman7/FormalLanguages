using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace FormalLanguages
{
    class NDAutomaton
    {
        protected Dictionary<int, Dictionary<char, List<int>>> delthaF;
        protected List<char> alphabet;
        protected int length;
        protected int amount;
        protected int initial;
        protected List<int> final;

        public NDAutomaton(string fileName)
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
                        temp.Add(char.Parse(mas[j]), variable);
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

        public NDAutomaton(eNDAutomaton init)
        {
            length = init.Length - 1;
            amount = init.Amount;
            initial = init.Initial;
            alphabet = init.Alphabet;
            final = init.Final;
            alphabet.Remove('\0');
            delthaF = new Dictionary<int, Dictionary<char, List<int>>>();

            var temp = init.DelthaF;
            Dictionary<int, List<int>> CL = new Dictionary<int, List<int>>();

            //forming CL dictionary and check for new final statuses
            foreach (var item in temp.Keys)
            {
                if (temp[item].ContainsKey('\0'))
                {
                    var lst = new List<int>(temp[item]['\0']);
                    lst.Add(item);
                    lst.Sort();
                    List<int> added = new List<int>();
                    foreach (var finalItem in final)
                        if (lst.Contains(finalItem))
                            added.Add(item);

                    //check for CLs, whick leads to final statuses in one step
                    foreach(var addedItem in added)
                        if(!final.Contains(addedItem))
                        {
                            final.Add(addedItem);
                            final.Sort();
                        }

                    CL[item] = lst;
                }
            }

            //re-update final statuses
            //two and more steps to final status
            for (int i = 0; i < CL.Count - 1; i++)
            {
                foreach (var status in CL.Keys)
                {
                    if (final.Contains(status))
                    {
                        foreach (var item in CL.Keys)
                            if (!final.Contains(item))
                            {
                                foreach (var itemValue in CL[item])
                                    if (final.Contains(itemValue))
                                    {
                                        final.Add(item);
                                        break;
                                    }
                            }
                    }
                }
            }

            //updating delthaF
            foreach (var item in temp.Keys)
            {
                if (!temp[item].ContainsKey('\0'))
                    delthaF[item] = new Dictionary<char, List<int>>(temp[item]);
                else
                {
                    //adding eps-statuses
                    var addedDict = new Dictionary<char, List<int>>(temp[item]);
                    foreach(var ch in alphabet)
                    {
                        var addedList = new List<int>();
                        if (temp[item].ContainsKey(ch))
                            addedList = temp[item][ch];

                        //check for additional moves
                        if (CL.ContainsKey(item))
                            foreach (var CLitems in CL[item])
                                if(temp[CLitems].ContainsKey(ch))
                                    foreach (var CLsubitem in temp[CLitems][ch])
                                        if (!addedList.Contains(CLsubitem))                                   
                                            addedList.Add(CLsubitem);
                        addedList.Sort();
                        addedDict[ch] = addedList;
                    }
                    delthaF[item] = addedDict;
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
                Console.Write(item + "\t\t");
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
                if(tempStatuses.Count==0)
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
