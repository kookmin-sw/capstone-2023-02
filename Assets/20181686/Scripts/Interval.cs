using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interval : IComparer<Interval>
{
    public float begin;
    public float end;

    public bool isInternal(float value) { return begin < value && value < end; }

    public int Compare(Interval x, Interval y)
    {
        return Comparer<float>.Default.Compare(x.begin, y.begin);
    }
}
