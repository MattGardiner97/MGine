using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGine.Structures
{
    public class Grouping<TKey, TValue>
    {
        private Dictionary<TKey, List<TValue>> dictionary = new Dictionary<TKey, List<TValue>>();

        private List<TValue> emptyList = new List<TValue>();

        public IEnumerable<TKey> Keys { get { return dictionary.Keys; } }

        public List<TValue> this[TKey Key]
        {
            get
            {
                if (Key != null && dictionary.ContainsKey(Key))
                    return dictionary[Key];
                return emptyList;
            }
        }

        public void Add(TKey Key, TValue Value)
        {
            List<TValue> valueList = null;
            if(Key != null && dictionary.ContainsKey(Key))
            {
                valueList = dictionary[Key];
                if (valueList == null)
                    valueList = new List<TValue>();
            }
            else
            {
                valueList = new List<TValue>();
                dictionary.Add(Key, valueList);
            }

            valueList.Add(Value);
        }

        public void Remove(TKey Key, TValue Value)
        {
            if(Key != null && dictionary.ContainsKey(Key))
                    dictionary[Key]?.Remove(Value);
        }

        public List<TValue> Get(TKey Key)
        {
            if (Key != null && dictionary.ContainsKey(Key))
                if (dictionary[Key] != null)
                    return dictionary[Key];

            return emptyList;
        }

    }
}
