using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    // doorDuration: 문이 움직이는 시간 (s)
    public float doorDuration = 1f;
    private float speed;
    public Door inner;
    public Door[] floorOuter;

    public int currentFloor = 0;
    public int destiationFloor = 0;
    private bool isOpened = false;
    private float oppendTimeLimit = 2f;
    private float oppendTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        inner.duration = doorDuration;
        foreach (Door d in floorOuter)
            d.duration = doorDuration;

        speed = floorOuter[1].transform.position.y - floorOuter[0].transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentFloor == destiationFloor)
        {
            if (!isOpened)
            {
                Open();
                isOpened = true;
            }
            else if (oppendTime < oppendTimeLimit)
                oppendTime += Time.deltaTime;
            else Close();
            return;
        }
        Vector3 dir = currentFloor < destiationFloor ? inner.transform.up : -inner.transform.up;
        inner.transform.position += dir * Time.deltaTime * speed;
        if (currentFloor < destiationFloor)
        {
            if (inner.transform.position.y >= floorOuter[currentFloor + 1].transform.position.y)
                currentFloor += 1;
        }
        else if (inner.transform.position.y <= floorOuter[currentFloor - 1].transform.position.y)
            currentFloor -= 1;
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

    public void Call(int floor, float time = 0.0f)
    {
        destiationFloor = floor;
        isOpened = false;
        oppendTime = 0f;
    }
}
