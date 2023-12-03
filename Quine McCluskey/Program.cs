using Quine_McCluskey;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
class Program
{
    //Input example
    //Example for minterms: 4 5 6 9 11 12 13 14 
    //Example for dont care's: 0 1 3 7
    static void Main(string[] args)
    {
        Methods obj = new Methods();
        List<int> mt = new List<int>();
        Console.Write("Enter the minterms: ");
        string[] mtInput = Console.ReadLine().Trim().Split();
        foreach (string i in mtInput)
        {
            mt.Add(int.Parse(i));
        }

        List<int> dc = new List<int>();
        Console.Write("Enter the don't cares(can be blank): ");
        string[] dcInput = Console.ReadLine().Trim().Split();
        try
        {
            foreach (string i in dcInput)
            {
                dc.Add(int.Parse(i));
            }
        }
        catch { }
        mt.AddRange(dc);
        mt.Sort();
        int size = Convert.ToString(mt[mt.Count - 1], 2).Length - 2;

        Dictionary<int, List<string>> groups = new Dictionary<int, List<string>>();
        HashSet<string> all_pi = new HashSet<string>();
        //Primary grouping starts
        foreach (int i in mt)
        {
            try
            {

                int count = obj.CountOnesInBinary(i);
                string binary = Convert.ToString(i, 2).PadLeft(4, '0');
                if (groups.ContainsKey(count))
                {
                    groups[count].Add(binary);
                }
                else
                {
                    groups[count] = new List<string> { binary };
                }
            }
            catch (KeyNotFoundException)
            {
                int count = obj.CountOnesInBinary(i);
                string binary = Convert.ToString(i, 2).PadLeft(4, '0');
                groups[count] = new List<string> { binary };
            }
        }
        //Primary grouping ends
        //Primary group printing starts
        Console.WriteLine("\n\n\n\nGroup No.\tMinterms\tBinary of Minterms\n{0}", new string('=', 60));
        foreach (var i in groups.Keys.OrderBy(x => x))
        {
            Console.WriteLine($"{i,6}:"); // Prints group number
            foreach (var j in groups[i])
            {
                Console.WriteLine($"\t\t    {Convert.ToInt32(j, 2),-20}{j}"); // Prints minterm and its binary representation
            }
            Console.WriteLine(new string('-',60));

        }
        //Primary group printing ends
        //Process for creating tables and finding prime implicants starts
        while (true)
        {
            Dictionary<int, List<string>> tmp = new Dictionary<int, List<string>>(groups);
            groups = new Dictionary<int, List<string>>();
            int m = 0;
            HashSet<string> marked = new HashSet<string>();
            bool should_stop = true;

            List<int> l = new List<int>(tmp.Keys);
            l.Sort();

            for (int i = 0; i < l.Count - 1; i++)
            {
                foreach (string j in tmp[l[i]])//Loop which iterates through current group elements
                {
                    foreach (string k in tmp[l[i + 1]])//Loop which iterates through next group elements
                    {
                        Tuple<bool, int> res = obj.Compare(j, k);//Compare the minterms
                        if (res.Item1)//If the minterms differ by 1 bit only
                        {
                            try
                            {
                                if (!groups[m].Contains(j.Substring(0, res.Item2) + '-' + j.Substring(res.Item2 + 1)))
                                {
                                    groups[m].Add(j.Substring(0, res.Item2) + '-' + j.Substring(res.Item2 + 1));//Put a '-' in the changing bit and add it to corresponding group
                                }
                            }
                            catch (KeyNotFoundException)
                            {
                                groups[m] = new List<string>() { j.Substring(0, res.Item2) + '-' + j.Substring(res.Item2 + 1) };//If the group doesn't exist, create the group at first and then put a '-' in the changing bit and add it to the newly created group
                            }
                            should_stop = false;
                            marked.Add(j);// Mark element j
                            marked.Add(k);// Mark element k
                        }
                    }
                }
                m++;
            }

            HashSet<string> local_unmarked = new HashSet<string>(obj.Flatten(tmp)).Except(marked).ToHashSet();//Unmarked elements of each table

            all_pi.UnionWith(local_unmarked);// Adding Prime Implicants to global list
            Console.WriteLine("Unmarked elements(Prime Implicants) of this table: " + (local_unmarked.Count == 0 ? "None" : string.Join(", ", local_unmarked)));// Printing Prime Implicants of current table

            if (should_stop)//If the minterms cannot be combined further
            {
                Console.WriteLine("\n\nAll Prime Implicants: " + (all_pi.Count == 0 ? "None" : string.Join(", ", all_pi)));//Print all prime implicants
                break;
            }
            //Printing of all the next groups starts
            Console.WriteLine("\n\n\n\nGroup No.\tMinterms\tBinary of Minterms\n" + new string('=', 60));
            foreach (int i in groups.Keys.OrderBy(x => x))
            {
                Console.WriteLine($"{i,6}:");//Prints group number
                foreach (string j in groups[i])
                {
                    Console.WriteLine($"\t\t{string.Join(",", obj.FindMinterms(j))}\t{j}");//Prints minterms and its binary representation
                }
                Console.WriteLine(new string('-', 60));
            }
            // Printing of all the next groups ends
        }
        //Process for creating tables and finding prime implicants ends

        //Printing and processing of Prime Implicant chart starts

        int sz = mt.Max();//The number of digits of the largest minterm
        Dictionary<string, List<string>> chart = new Dictionary<string, List<string>>();
        Console.WriteLine("\n\n\nPrime Implicants chart:\n\n    Minterms    |{0}\n{1}", string.Join(" ", mt.Select(i => $"{new string(' ', sz - i.ToString().Length)}{i}")), new string('=', mt.Count * (sz + 1) + 16));
        foreach (string i in all_pi)
        {
            List<string> merged_minterms = obj.FindMinterms(i);
            int y = 0;
            Console.Write("{0,-16}|", string.Join(",", merged_minterms));
            foreach (string j in obj.Refine(merged_minterms, dc))
            {
                int x = mt.IndexOf(int.Parse(j)) * (sz + 1);//The position where we should put 'X'
                Console.Write("{0}{1}", new string(' ', Math.Abs(x - y)), new string(' ', sz - 1));
                Console.Write("X");
                y = x + sz;
                if (chart.ContainsKey(j))// Add minterm in chart
                {
                    if (!chart[j].Contains(i))
                    {
                        chart[j].Add(i);
                    }
                }
                else
                {
                    chart[j] = new List<string> { i };
                }
            }
            Console.WriteLine("\n{0}", new string('-', mt.Count * (sz + 1) + 16));
        }
        //Printing and processing of Prime Implicant chart ends

        List<string> EPI = obj.FindEPI(chart);//Finding essential prime implicants
        Console.WriteLine("\nEssential Prime Implicants: " + string.Join(", ", EPI));
        obj.RemoveTerms(chart, EPI);
        List<string> final_result = new List<string>();
        foreach (var i in EPI)
        {
            final_result.Add(obj.FindVariables(i));
        }
        Console.WriteLine("\nSolution: F = " + string.Join(" + ", final_result.Select(i => string.Join("", i))));



    }
}





