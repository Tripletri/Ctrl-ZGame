using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlZ
{
    class LimitedSizeQueue<T>
    {
        private int limit;
        private LinkedList<T> list = new LinkedList<T>();

        public LimitedSizeQueue(int limit)
        {
            this.limit = limit;
        }

        public void Add(T item)
        {
            if (list.Count >= limit)
                list.RemoveFirst();
            list.AddLast(item);
        }

        public T Last => list.First.Value;

        public T First => list.Last.Value;

        public void Clear() =>
            list.Clear();
    }
}