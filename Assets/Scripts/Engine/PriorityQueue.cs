using System;
using System.Collections.Generic;
using System.Linq;
	 
class PriorityQueue<P,V>
{
	//code from http://goo.gl/ugzP6
	
	private SortedDictionary<P, Queue<V>> list = new SortedDictionary<P, Queue<V>>();
    
	//add element to queue
	public void Enqueue(P priority, V value)
    {
        Queue<V> q;
        if (!list.TryGetValue(priority, out q))
        {
            q = new Queue<V>();
            list.Add(priority, q);
        }
        q.Enqueue(value);
    }
	
	//remove element from queue
    public V Dequeue()
    {
        // will throw if there isn’t any first element!
        var pair = list.First();
        var v = pair.Value.Dequeue();
        if (pair.Value.Count == 0) // nothing left of the top priority.
            list.Remove(pair.Key);
        return v;
    }
    
	public bool IsEmpty
    {
        get { return !list.Any(); }
    }
}