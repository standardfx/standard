using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Standard.Collections.Concurrent
{
    public class ConcurrentSortedList<TKey, TValue>
    {
        private object _lock = new object();
        SortedList<TKey, TValue> _list = new SortedList<TKey, TValue>();

        public int Count
        {
            get 
            { 
                lock (_lock) 
                    return _list.Count; 
            }
        }

        public void Add(TKey key, TValue val)
        {
            lock (_lock)
            {
                if (_list.ContainsKey(key) == false)
                    _list.Add(key, val);
                else
                    _list[key] = val;
            }
        }

        public void Remove(TKey key)
        {
            if (key == null)
                return;

            lock (_lock)
                _list.Remove(key);
        }

        public TKey GetKey(int index)
        {
            lock (_lock) 
                return _list.Keys[index];
        }

        public TValue GetValue(int index)
        {
            lock (_lock)
                return _list.Values[index];
        }

        public TKey[] Keys()
        {
            lock (_lock)
            {
                TKey[] keys = new TKey[_list.Keys.Count];
                _list.Keys.CopyTo(keys, 0);
                return keys;
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)_list).GetEnumerator();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (_lock)
                return _list.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get
            {
                lock (_lock)
                    return _list[key];
            }
            set
            {
                lock (_lock)
                    _list[key] = value;
            }
        }
    }
}
