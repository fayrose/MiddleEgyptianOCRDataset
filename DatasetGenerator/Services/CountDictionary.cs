using System;
using System.Collections.Generic;
using System.Text;

namespace DatasetGenerator
{
    class CountDictionary
    {
        public static Dictionary<string, int> Generate(IEnumerable<string> input)
        {
            Dictionary<string, int> countDict = new Dictionary<string, int>();
            foreach (string item in input)
            {
                if (countDict.ContainsKey(item))
                {
                    countDict[item] += 1;
                }
                else
                {
                    countDict.Add(item, 1);
                }
            }
            return countDict;
        }
    }
}
