using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interval : IComparable
{
    public float begin;
    public float end;

    public Interval(float __begin = 0f, float __end = 0f)
    {
        begin = __begin;
        end = __end;
    }

    public bool isOverlap(Interval interval)
    {
        float b = Mathf.Max(begin, interval.begin);
        float e = Mathf.Max(end, interval.end);
        return e - b > 0;
    }

    public int CompareTo(object obj)
    {
        Interval interval = obj as Interval;
        return Comparer<float>.Default.Compare(begin, interval.begin);
    }

    private static Interval _zero = new Interval();
    public static Interval zero { get { return _zero; } }
}
