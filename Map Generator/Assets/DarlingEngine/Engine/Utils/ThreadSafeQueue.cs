using System.Collections.Generic;
using System.Linq;


public class ThreadSafeQueue<T>
{
    private Queue<T> queue = new Queue<T>();
    private readonly object lockObject = new object();

    public void Enqueue(T item)
    {
        lock (lockObject)
        {
            queue.Enqueue(item);
        }
    }

    public T Dequeue()
    {
        lock (lockObject)
        {
            return queue.Dequeue();
        }
    }

    public bool Any()
    {
        lock (lockObject)
        {
            return queue.Any();
        }
    }
}