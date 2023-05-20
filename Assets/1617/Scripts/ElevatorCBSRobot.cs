using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorCBSRobot : MonoBehaviour
{
    public string command;

    public enum ElevatorCBSRobotAction { FORWARD, LEFT, RIGHT, WAIT, UP, DOWN };
    public float actionTime = 1f;

    public Vector3 currentPosition;
    public float currentRotation;

    public List<ElevatorCBSRobotAction> actions;

    public float timer;
    public int actionIndex;

    // Start is called before the first frame update
    void Start()
    {
        actions = new List<ElevatorCBSRobotAction>();
        foreach (char c in command)
        {
            if (c == 'F') actions.Add(ElevatorCBSRobotAction.FORWARD);
            else if (c == 'L') actions.Add(ElevatorCBSRobotAction.LEFT);
            else if (c == 'R') actions.Add(ElevatorCBSRobotAction.RIGHT);
            else if (c == 'W') actions.Add(ElevatorCBSRobotAction.WAIT);
            else if (c == 'U') actions.Add(ElevatorCBSRobotAction.UP);
            else if (c == 'D') actions.Add(ElevatorCBSRobotAction.DOWN);
            else if (c == '/') actions.Add(ElevatorCBSRobotAction.WAIT);
        }

        currentPosition = transform.position;
        currentRotation = 0;

        //actions = new List<ElevatorCBSRobotAction>();
        //timer = 0;
        //actionIndex = -1;
    }

    // Update is called once per frame
    void Update()
    {
        if (actionIndex >= 0 && actionIndex < actions.Count)
        {
            timer += Time.deltaTime;
            if (actions[actionIndex] == ElevatorCBSRobotAction.FORWARD)
            {
                transform.position = Vector3.Lerp(currentPosition, currentPosition + transform.forward, timer / actionTime);
            }
            else if (actions[actionIndex] == ElevatorCBSRobotAction.LEFT)
            {
                transform.rotation = Quaternion.Lerp(Quaternion.Euler(0, currentRotation, 0), Quaternion.Euler(0, currentRotation - 90f, 0), timer / actionTime);
            }
            else if (actions[actionIndex] == ElevatorCBSRobotAction.RIGHT)
            {
                transform.rotation = Quaternion.Lerp(Quaternion.Euler(0, currentRotation, 0), Quaternion.Euler(0, currentRotation + 90f, 0), timer / actionTime);
            }
            else if (actions[actionIndex] == ElevatorCBSRobotAction.UP)
            {
                transform.position = Vector3.Lerp(currentPosition, currentPosition + transform.up, timer / actionTime);
            }
            else if (actions[actionIndex] == ElevatorCBSRobotAction.DOWN)
            {
                transform.position = Vector3.Lerp(currentPosition, currentPosition - transform.up, timer / actionTime);
            }
            if (timer >= actionTime)
            {
                if (actions[actionIndex] == ElevatorCBSRobotAction.FORWARD)
                {
                    currentPosition += transform.forward;
                }
                else if (actions[actionIndex] == ElevatorCBSRobotAction.LEFT)
                {
                    currentRotation -= 90f;
                }
                else if (actions[actionIndex] == ElevatorCBSRobotAction.RIGHT)
                {
                    currentRotation += 90f;
                }
                else if (actions[actionIndex] == ElevatorCBSRobotAction.UP)
                {
                    currentPosition += transform.up;
                }
                else if (actions[actionIndex] == ElevatorCBSRobotAction.DOWN)
                {
                    currentPosition -= transform.up;
                }
                transform.position = currentPosition;
                transform.rotation = Quaternion.Euler(0, currentRotation, 0);
                actionIndex += 1;
                timer = 0;
            }
        }
    }
}
