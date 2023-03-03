using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Motion
{
    public Vector3 deltaPosition;
    public Quaternion deltaRotation;
    public float time;

    public Motion(float _time = 1.0f)
    {
        deltaPosition = Vector2.zero;
        deltaRotation = Quaternion.Euler(0f, 0f, 0f);
        time = _time;
    }

    public Motion(Vector3 _deltaPosition, float _time = 1.0f)
    {
        deltaPosition = _deltaPosition;
        deltaRotation = Quaternion.Euler(0f, 0f, 0f);
        time = _time;
    }

    public Motion(Quaternion _deltaRotation, float _time = 1.0f)
    {
        deltaPosition = Vector3.zero;
        deltaRotation = _deltaRotation;
        time = _time;
    }

    public Motion(Vector3 _deltaPosition, Quaternion _deltaRotation, float _time = 1.0f)
    {
        deltaPosition = _deltaPosition;
        deltaRotation = _deltaRotation;
        time = _time;
    }

    private static Motion _wait = new Motion();
    public static Motion wait { get { return _wait; } }
    private static Motion _forward = new Motion(Vector3.forward);
    public static Motion forward { get { return _forward; } }
    private static Motion _back = new Motion(Vector3.back);
    public static Motion back { get { return _back; } }
    private static Motion _left = new Motion(Vector3.left);
    public static Motion left { get { return _left; } }
    private static Motion _right = new Motion(Vector3.right);
    public static Motion right { get { return _right; } }
    private static Motion _cw = new Motion(Quaternion.Euler(0f, 90f, 0f));
    public static Motion cw { get { return _cw; } }
    private static Motion _ccw = new Motion(Quaternion.Euler(0f, -90f, 0f));
    public static Motion ccw { get { return _ccw; } }
    private static Motion _none = new Motion(-1.0f);
    public static Motion none { get { return _none; } }
}
