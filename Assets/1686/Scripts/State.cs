using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    public State previousState;
    public Interval interval;
    public Motion motion; // reach Motion
    public Vector3 position;
    public float time;
    public int successorCount = 0;

    public State(Vector3 _position, float _time)
    {
        previousState = null;
        motion = Motion.wait;
        position = _position;
        time = _time;
    }

    public State(State _previousState, Vector3 _position, Motion _motion, float _time)
    {
        previousState = _previousState;
        motion = _motion;
        position = _position;
        time = _time;
    }
}
