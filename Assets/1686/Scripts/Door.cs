using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Door : MonoBehaviour
{
    public GameObject left;
    public GameObject right;
    private Vector3 direction;
    internal float duration = 1f;
    private float durationSum;

    // status: 1=open, -1=close
    private bool closed = true;
    private int speed = 0;

    // Start is called before the first frame update
    void Start()
    {
        direction = (left.transform.position - right.transform.position) * 0.95f;
    }

    // Update is called once per frame
    void Update()
    {
        if (speed == 0) return;
        float time = Mathf.Min(Time.deltaTime, duration - durationSum);

        Vector3 deltaPosition = speed * (direction / duration) * time;
        left.transform.position += deltaPosition;
        right.transform.position -= deltaPosition;

        durationSum += time;
        if (durationSum >= duration) speed = 0;
    }

    public void Open()
    {
        if (!closed) return;
        speed = 1;
        durationSum = 0;
        closed = false;
    }
    public void Close()
    {
        if (closed) return;
        speed = -1;
        durationSum = 0;
        closed = true;
    }
}
