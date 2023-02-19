using System.Collections.Generic;

// Peek with high priority
public class PriorityQueue<TElement, TPriority>
{
    private class Node
    {
        public TElement data { get; private set; }
        public TPriority priority { get; set; }

        public Node(TElement _data, TPriority _priority)
        {
            data = _data;
            priority = _priority;
        }
    }

    private List<Node> nodes = new List<Node>();

    public int Count => nodes.Count;
    private Comparer<TPriority> comparer;
    public PriorityQueue(Comparer<TPriority> _comparer) { comparer = _comparer; }

    public void Enqueue(TElement data, TPriority priority)
    {
        nodes.Add(new Node(data, priority));
        int current = nodes.Count - 1;
        while (current > 0)
        {
            int parent = (current - 1) >> 1;
            if (comparer.Compare(nodes[parent].priority, nodes[current].priority) >= 0) break;
            Swap(parent, current);

            current = parent;
        }
    }

    public TElement Dequeue()
    {
        TElement ret = Peek();
        Swap(0, nodes.Count - 1);
        nodes.RemoveAt(nodes.Count - 1);

        int currennt = 0, left = 1, right = 2;
        while (left < nodes.Count)
        {
            int child = (right >= nodes.Count || comparer.Compare(nodes[left].priority, nodes[right].priority) > 0) ? left : right;
            if (comparer.Compare(nodes[currennt].priority, nodes[child].priority) >= 0) break;
            Swap(currennt, child);
            currennt = child;
            left = (currennt << 1) + 1;
            right = left + 1;
        }

        return ret;
    }

    public TElement Peek()
    {
        if (nodes.Count > 0)
            return nodes[0].data;
        return default(TElement);
    }

    private void Swap(int a, int b)
    {
        Node tmp = nodes[a];
        nodes[a] = nodes[b];
        nodes[b] = tmp;
    }

    public List<TPriority> GetPriorities()
    {
        List<TPriority> ret = new List<TPriority>();
        for (int i = 0; i < nodes.Count; i++)
            ret.Add(nodes[i].priority);
        return ret;
    }

    public List<TElement> GetElements()
    {
        List<TElement> ret = new List<TElement>();
        for (int i = 0; i < nodes.Count; i++)
            ret.Add(nodes[i].data);
        return ret;
    }
}