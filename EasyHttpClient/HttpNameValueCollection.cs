using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyHttpClient
{
    public class HttpNameValueCollection : List<KeyValuePair<string, string>>
    {
        public string[] GetValues(string key)
        {
            return this.Where(i => string.Equals(key, i.Key, StringComparison.OrdinalIgnoreCase)).Select(i => i.Value).ToArray();
        }

        public bool ContainsKey(string key)
        {
           return this.Any(i => string.Equals(key, i.Key, StringComparison.OrdinalIgnoreCase));
        }

        public string Get(string key)
        {
            var m = this.FirstOrDefault(i => string.Equals(key, i.Key, StringComparison.OrdinalIgnoreCase));
            return m.Value;
        }

        public void Add(string key, string value)
        {
            this.Add(new KeyValuePair<string, string>(key, value));
        }

        public void Add(string key, IEnumerable<string> values)
        {
            foreach (var value in values)
                this.Add(key, value);
        }

        public void Remove(string key)
        {
            this.RemoveAll(i => string.Equals(key, i.Key, StringComparison.OrdinalIgnoreCase));
        }

        public void Set(string key, string value)
        {
            this.Remove(key);
            this.Add(key, value);
        }

        public void Set(string key, IEnumerable<string> values)
        {
            this.Remove(key);
            this.Add(key, values);
        }
        //
        // Summary:
        //     Gets or sets the entry with the specified key in the System.Collections.Specialized.NameValueCollection.
        //
        // Parameters:
        //   name:
        //     The System.String key of the entry to locate. The key can be null.
        //
        // Returns:
        //     A System.String that contains the comma-separated list of values associated
        //     with the specified key, if found; otherwise, null.
        //
        // Exceptions:
        //   System.NotSupportedException:
        //     The collection is read-only and the operation attempts to modify the collection.
        public string this[string name]
        {
            get
            {
                return this.Get(name);
            }
            set
            {
                this.Set(name, value);
            }
        }
    }
}
