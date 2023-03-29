using System;
using System.Collections.Generic;
using UnityEngine;

namespace SIPP
{
    public class Interval : IComparable<Interval>
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

        public int CompareTo(Interval interval)
        {
            Debug.Log("Compare " + end + " and " + interval.end + ":" + Comparer<float>.Default.Compare(begin, interval.begin));
            return Comparer<float>.Default.Compare(end, interval.end);
        }

        public override string ToString()
        {
            return "Interval(" + begin + ", " + end + ")";
        }

        private static Interval _zero = new Interval();
        private static Interval _inf = new Interval(Mathf.Infinity, Mathf.Infinity);
    }
}