using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fhirStartersApp.Utilities
{
    public static class Extensions
    {
        public static Dictionary<TKey, TValue> Merge<TKey, TValue>(this IDictionary<TKey, TValue> dictionary1,
            IDictionary<TKey, TValue> dictionary2)
        {
            var dictionaries = new List<IDictionary<TKey, TValue>> {dictionary1, dictionary2};
            return dictionaries.Merge();
        }

        public static Dictionary<TKey, TValue> Merge<TKey, TValue>(this IEnumerable<IDictionary<TKey, TValue>> dictionaries)
        {
            //https://stackoverflow.com/questions/294138/merging-dictionaries-in-c-sharp
            return dictionaries.SelectMany(dict => dict)
                .ToLookup(pair => pair.Key, pair => pair.Value)
                .ToDictionary(group => group.Key, group => group.First());
        }
    }
}
