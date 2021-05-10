using System.Collections.Generic;

namespace CtrlZ
{
    internal class LimitedSizeQueue<T>
    {
        private readonly int limit;
        private readonly LinkedList<T> list = new LinkedList<T>();

        public T Last => list.First.Value;

        public T First => list.Last.Value;

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

        public void Clear()
        {
            list.Clear();
        }
    }
}