using System;
using System.Collections.Generic;
using System.Linq;

namespace ChangesetViewer.Core.Model
{
    public class EventQueue<T>
    {
        private readonly Queue<T> queue = new Queue<T>();
        public event EventHandler Enqueued;
        public bool IsAllowedToEnqueue { get; set; }
        protected virtual void OnEnqueued()
        {
            if (Enqueued != null)
            {
                Enqueued(this, new EventArgs());
            }
        }
        public virtual void Enqueue(T item)
        {
            if (IsAllowedToEnqueue)
            {
                queue.Enqueue(item);
                OnEnqueued();
            }

        }
        public int Count
        {
            get
            {
                return queue.Count;
            }
        }
        public virtual T Dequeue()
        {
            T item = queue.Dequeue();
            OnEnqueued();
            return item;
        }

        public virtual void Clear()
        {
            queue.Clear();
            OnEnqueued();
        }

        public virtual bool Any()
        {
            return queue.Any();
        }
    }
}
