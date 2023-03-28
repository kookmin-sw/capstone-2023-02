using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SIPP
{
    public class State
    {
        public State prev;
        public State next;
        public Motion motion; // reach Motion
        public Vector3 position;
        public Interval interval;
        public float time;

        public State(Vector3 _position, Interval _interval, float _time)
        {
            interval = _interval;
            position = _position;
            time = _time;

            prev = next = null;
            motion = Motion.none;
        }

        public override string ToString()
        {
            return "State(" +
                   "position: [" + (int)position.x + ", " + (int)position.z + "]," +
                   "motion: " + motion +
                   ")";
        }
    }
}