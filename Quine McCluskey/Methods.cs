using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quine_McCluskey
{
    internal class Methods
    {
        public int CountOnesInBinary(int number)//Count how much one's has in string. For example 1010 has 2 one's
        {
            int count = 0;
            while (number > 0)
            {
                if ((number & 1) == 1)
                {
                    count++;
                }
                number >>= 1;
            }
            return count;
        }
        public List<string> Flatten(Dictionary<int, List<string>> x)//Flattens a list
        {
            List<string> flattenedItems = new List<string>();
            foreach (var item in x.Values)
            {
                flattenedItems.AddRange(item);
            }
            return flattenedItems;
        }
        public Tuple<bool, int> Compare(string a, string b)//Function for checking if 2 minterms differ by 1 bit only
        {
            int c = 0;
            int mismatchIndex = 0;
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                {
                    mismatchIndex = i;
                    c += 1;
                    if (c > 1)
                    {
                        return Tuple.Create(false, 0);
                    }
                }
            }
            return Tuple.Create(true, mismatchIndex);
        }
        public List<string> FindMinterms(string a)//Function for finding out which minterms are merged. For example, 10-1 is obtained by merging 9(1001) and 11(1011)
        {
            int gaps = a.Count(c => c == '-');
            if (gaps == 0)
            {
                return new List<string> { Convert.ToString(Convert.ToInt32(a, 2)) };
            }
            List<string> x = new List<string>();
            for (int i = 0; i < Math.Pow(2, gaps); i++)
            {
                x.Add(Convert.ToString(i, 2).PadLeft(gaps, '0'));
            }
            List<string> temp = new List<string>();
            for (int i = 0; i < Math.Pow(2, gaps); i++)
            {
                string temp2 = a;
                int ind = -1;
                foreach (char j in x[0])
                {
                    if (ind != -1)
                    {
                        ind = ind + temp2.Substring(ind + 1).IndexOf('-') + 1;
                    }
                    else
                    {
                        ind = temp2.Substring(ind + 1).IndexOf('-');
                    }
                    temp2 = temp2.Substring(0, ind) + j + temp2.Substring(ind + 1);
                }
                temp.Add(Convert.ToString(Convert.ToInt32(temp2, 2)));
                x.RemoveAt(0);
            }
            return temp;
        }
        public List<string> Refine(List<string> my_list, List<int> dc_list)// Removes don't care terms from a given list and returns refined list
        {
            List<string> res = new List<string>();
            foreach (string i in my_list)
            {
                if (!dc_list.Contains(int.Parse(i)))
                {
                    res.Add(i);
                }
            }
            return res;
        }
        public List<string> FindEPI(Dictionary<string, List<string>> x)// Function to find essential prime implicants from prime implicants chart
        {
            List<string> res = new List<string>();
            foreach (string i in x.Keys)
            {
                if (x[i].Count == 1 && !res.Contains(x[i][0]))
                {
                    res.Add(x[i][0]);
                }
            }
            return res;
        }
        public void RemoveTerms(Dictionary<string, List<string>> chart, List<string> terms)// Removes minterms which are already covered from chart
        {
            foreach (string i in terms)
            {
                foreach (string j in FindMinterms(i))
                {
                    chart.Remove(j);
                }
            }
        }
        public string FindVariables(string x)// Function to find variables in a meanterm. For example, the minterm --01 has C' and D as variables
        {
            string variables = null;
            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] == '0')
                {
                    switch (i)
                    {
                        case 0:
                            variables += "A'";
                            break;
                        case 1:
                            variables += "B'";
                            break;
                        case 2:
                            variables += "C'";
                            break;
                        case 3:
                            variables += "D'";
                            break;
                    }
                }
                else if (x[i] == '1')
                {
                    switch (i)
                    {
                        case 0:
                            variables += 'A';
                            break;
                        case 1:
                            variables += 'B';
                            break;
                        case 2:
                            variables += 'C';
                            break;
                        case 3:
                            variables += 'D';
                            break;
                    }
                }
            }
            return variables;
        }
    }
}
