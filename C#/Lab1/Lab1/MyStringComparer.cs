using System;
using System.Collections.Generic;

namespace Lab1
{
    [Serializable]
    public class MyStringComparer : IComparer<string>
    {
        public int Compare(string a, string b)
        {
            if (a.Length > b.Length)
            {
                return 1;
            }
            else if (a.Length < b.Length)
            {
                return -1;
            }
            else
            {
                return a.CompareTo(b);
            }
        }
    }
}
