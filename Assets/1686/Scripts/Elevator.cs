using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    // doorDuration: 문이 움직이는 시간 (s)
    public float doorDuration = 1f;
    public Door inner;
    public Door[] floorOuter;

    public int currentFloor = 0;
    public int destiationFloor = 0;

    // Start is called before the first frame update
    void Start()
    {
        inner.duration = doorDuration;
        foreach (Door d in floorOuter)
            d.duration = doorDuration;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentFloor == destiationFloor) return;
    }



    public void Open()
    {
        inner.Open();
        floorOuter[currentFloor].Open();
    }

    public void Close()
    {
        inner.Close();
        floorOuter[currentFloor].Close();
    }
}
