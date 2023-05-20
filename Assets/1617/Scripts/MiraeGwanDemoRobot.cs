using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiraeGwanDemoRobot : MonoBehaviour
{
    public enum MiraeGwanDemoRobotAction { FORWARD, LEFT, RIGHT, WAIT, UP, DOWN };

    public GameObject[] robots;
    public GameObject[] robotFlags;
    public string[] robotCommands;
    public Color[] robotColors;

    public GameObject elevator1DoorLeft;
    public GameObject elevator1DoorRight;
    public GameObject elevator2DoorLeft;
    public GameObject elevator2DoorRight;

    public float actionTime = 1f;

    public Vector3[] robotCurrentPositions;
    public float[] robotCurrentRotations;
    public List<MiraeGwanDemoRobotAction>[] robotActions;
    public Dictionary<int, List<KeyValuePair<int, Vector3>>> robotFlagActions;

    public Vector3 elevator1DoorLeftCurrentPosition;
    public Vector3 elevator1DoorRightCurrentPosition;
    public Vector3 elevator2DoorLeftCurrentPosition;
    public Vector3 elevator2DoorRightCurrentPosition;

    public float timer;
    public int actionIndex;

    // Start is called before the first frame update
    void Start()
    {
        robotCurrentPositions = new Vector3[robots.Length];
        robotCurrentRotations = new float[robots.Length];
        robotActions = new List<MiraeGwanDemoRobotAction>[robots.Length];
        robotFlagActions = new Dictionary<int, List<KeyValuePair<int, Vector3>>>();

        elevator1DoorLeftCurrentPosition = elevator1DoorLeft.transform.position;
        elevator1DoorRightCurrentPosition = elevator1DoorRight.transform.position;
        elevator2DoorLeftCurrentPosition = elevator2DoorLeft.transform.position;
        elevator2DoorRightCurrentPosition = elevator2DoorRight.transform.position;

        int tmp = 0;
        int tmpSign = 1;
        Vector3 v = Vector3.zero;

        for (int i = 0; i < robots.Length; i++)
        {
            robotFlags[i].SetActive(false);

            robotCurrentPositions[i] = robots[i].transform.position;

            robots[i].GetComponent<Renderer>().material.SetColor("_Color", robotColors[i]);
            robotFlags[i].GetComponent<Renderer>().material.SetColor("_Color", robotColors[i]);

            robotActions[i] = new List<MiraeGwanDemoRobotAction>();
            for (int j = 0; j < robotCommands[i].Length; j++)
            {
                char c = robotCommands[i][j];
                if (c == 'F') robotActions[i].Add(MiraeGwanDemoRobotAction.FORWARD);
                else if (c == 'L') robotActions[i].Add(MiraeGwanDemoRobotAction.LEFT);
                else if (c == 'R') robotActions[i].Add(MiraeGwanDemoRobotAction.RIGHT);
                else if (c == 'W') robotActions[i].Add(MiraeGwanDemoRobotAction.WAIT);
                else if (c == 'U') robotActions[i].Add(MiraeGwanDemoRobotAction.UP);
                else if (c == 'D') robotActions[i].Add(MiraeGwanDemoRobotAction.DOWN);
                else if (c == '/') robotActions[i].Add(MiraeGwanDemoRobotAction.WAIT);
                else if (c >= '0' && c <= '9') tmp = tmp * 10 + c - '0';
                else if (c == '-') tmpSign = -1;
                else if (c == ',')
                {
                    v.x = tmp * tmpSign;
                    tmp = 0;
                    tmpSign = 1;
                }
                else if (c == '.')
                {
                    v.y = tmp * tmpSign;
                    tmp = 0;
                    tmpSign = 1;
                }
                else if (c == ';')
                {
                    v.z = tmp * tmpSign;
                    tmp = 0;
                    tmpSign = 1;
                    if (!robotFlagActions.ContainsKey(robotActions[i].Count)) robotFlagActions[robotActions[i].Count] = new List<KeyValuePair<int, Vector3>>();
                    robotFlagActions[robotActions[i].Count].Add(new KeyValuePair<int, Vector3>(i, v));
                }
            }
        }

        timer = 0;

        if (robotFlagActions.ContainsKey(0))
        {
            foreach (KeyValuePair<int, Vector3> kv in robotFlagActions[0])
            {
                robotFlags[kv.Key].SetActive(true);
                robotFlags[kv.Key].transform.position = kv.Value;
            }
        }

        //actions = new List<MiraeGwanDemoRobotAction>();
        //foreach (char c in command)
        //{
        //    if (c == 'F') actions.Add(MiraeGwanDemoRobotAction.FORWARD);
        //    else if (c == 'L') actions.Add(MiraeGwanDemoRobotAction.LEFT);
        //    else if (c == 'R') actions.Add(MiraeGwanDemoRobotAction.RIGHT);
        //    else if (c == 'W') actions.Add(MiraeGwanDemoRobotAction.WAIT);
        //    else if (c == 'U') actions.Add(MiraeGwanDemoRobotAction.UP);
        //    else if (c == 'D') actions.Add(MiraeGwanDemoRobotAction.DOWN);
        //    else if (c == '/') actions.Add(MiraeGwanDemoRobotAction.WAIT);
        //}

        //currentPosition = transform.position;
        //currentRotation = 0;

        //actions = new List<MiraeGwanDemoRobotAction>();

    }

    // Update is called once per frame
    void Update()
    {
        if (actionIndex < 0) return;

        timer += Time.deltaTime;
        for (int i = 0; i < robots.Length; i++)
        {
            if (actionIndex + 2 < robotActions[i].Count && robotActions[i][actionIndex + 2] == MiraeGwanDemoRobotAction.UP)
            {
                elevator1DoorLeft.transform.position = Vector3.Lerp(elevator1DoorLeftCurrentPosition, elevator1DoorLeftCurrentPosition + 0.7f * Vector3.right, timer / actionTime);
                elevator1DoorRight.transform.position = Vector3.Lerp(elevator1DoorRightCurrentPosition, elevator1DoorRightCurrentPosition - 0.7f * Vector3.right, timer / actionTime);
            }

            if (actionIndex + 1 < robotActions[i].Count && robotActions[i][actionIndex + 1] == MiraeGwanDemoRobotAction.UP)
            {
                elevator1DoorLeft.transform.position = Vector3.Lerp(elevator1DoorLeftCurrentPosition, elevator1DoorLeftCurrentPosition - 0.7f * Vector3.right, timer / actionTime);
                elevator1DoorRight.transform.position = Vector3.Lerp(elevator1DoorRightCurrentPosition, elevator1DoorRightCurrentPosition + 0.7f * Vector3.right, timer / actionTime);
            }

            if (actionIndex - 1 >= 0 && actionIndex - 1 < robotActions[i].Count && robotActions[i][actionIndex - 1] == MiraeGwanDemoRobotAction.UP)
            {
                elevator2DoorLeft.transform.position = Vector3.Lerp(elevator2DoorLeftCurrentPosition, elevator2DoorLeftCurrentPosition + 0.7f * Vector3.right, timer / actionTime);
                elevator2DoorRight.transform.position = Vector3.Lerp(elevator2DoorRightCurrentPosition, elevator2DoorRightCurrentPosition - 0.7f * Vector3.right, timer / actionTime);
            }

            if (actionIndex - 2 >= 0 && actionIndex - 2 < robotActions[i].Count && robotActions[i][actionIndex - 2] == MiraeGwanDemoRobotAction.UP)
            {
                elevator2DoorLeft.transform.position = Vector3.Lerp(elevator2DoorLeftCurrentPosition, elevator2DoorLeftCurrentPosition - 0.7f * Vector3.right, timer / actionTime);
                elevator2DoorRight.transform.position = Vector3.Lerp(elevator2DoorRightCurrentPosition, elevator2DoorRightCurrentPosition + 0.7f * Vector3.right, timer / actionTime);
            }

            if (actionIndex + 2 < robotActions[i].Count && robotActions[i][actionIndex + 2] == MiraeGwanDemoRobotAction.DOWN)
            {
                elevator2DoorLeft.transform.position = Vector3.Lerp(elevator2DoorLeftCurrentPosition, elevator2DoorLeftCurrentPosition + 0.7f * Vector3.right, timer / actionTime);
                elevator2DoorRight.transform.position = Vector3.Lerp(elevator2DoorRightCurrentPosition, elevator2DoorRightCurrentPosition - 0.7f * Vector3.right, timer / actionTime);
            }

            if (actionIndex + 1 < robotActions[i].Count && robotActions[i][actionIndex + 1] == MiraeGwanDemoRobotAction.DOWN)
            {
                elevator2DoorLeft.transform.position = Vector3.Lerp(elevator2DoorLeftCurrentPosition, elevator2DoorLeftCurrentPosition - 0.7f * Vector3.right, timer / actionTime);
                elevator2DoorRight.transform.position = Vector3.Lerp(elevator2DoorRightCurrentPosition, elevator2DoorRightCurrentPosition + 0.7f * Vector3.right, timer / actionTime);
            }

            if (actionIndex - 1 >= 0 && actionIndex - 1 < robotActions[i].Count && robotActions[i][actionIndex - 1] == MiraeGwanDemoRobotAction.DOWN)
            {
                elevator1DoorLeft.transform.position = Vector3.Lerp(elevator1DoorLeftCurrentPosition, elevator1DoorLeftCurrentPosition + 0.7f * Vector3.right, timer / actionTime);
                elevator1DoorRight.transform.position = Vector3.Lerp(elevator1DoorRightCurrentPosition, elevator1DoorRightCurrentPosition - 0.7f * Vector3.right, timer / actionTime);
            }

            if (actionIndex - 2 >= 0 && actionIndex - 2 < robotActions[i].Count && robotActions[i][actionIndex - 2] == MiraeGwanDemoRobotAction.DOWN)
            {
                elevator1DoorLeft.transform.position = Vector3.Lerp(elevator1DoorLeftCurrentPosition, elevator1DoorLeftCurrentPosition - 0.7f * Vector3.right, timer / actionTime);
                elevator1DoorRight.transform.position = Vector3.Lerp(elevator1DoorRightCurrentPosition, elevator1DoorRightCurrentPosition + 0.7f * Vector3.right, timer / actionTime);
            }

            if (actionIndex >= 0 && actionIndex < robotActions[i].Count)
            {
                if (robotActions[i][actionIndex] == MiraeGwanDemoRobotAction.FORWARD)
                {
                    robots[i].transform.position = Vector3.Lerp(robotCurrentPositions[i], robotCurrentPositions[i] + robots[i].transform.forward, timer / actionTime);
                }
                else if (robotActions[i][actionIndex] == MiraeGwanDemoRobotAction.LEFT)
                {
                    robots[i].transform.rotation = Quaternion.Lerp(Quaternion.Euler(0, robotCurrentRotations[i], 0), Quaternion.Euler(0, robotCurrentRotations[i] - 90f, 0), timer / actionTime);
                }
                else if (robotActions[i][actionIndex] == MiraeGwanDemoRobotAction.RIGHT)
                {
                    robots[i].transform.rotation = Quaternion.Lerp(Quaternion.Euler(0, robotCurrentRotations[i], 0), Quaternion.Euler(0, robotCurrentRotations[i] + 90f, 0), timer / actionTime);
                }
                else if (robotActions[i][actionIndex] == MiraeGwanDemoRobotAction.UP)
                {
                    robots[i].transform.position = Vector3.Lerp(robotCurrentPositions[i], robotCurrentPositions[i] + 6.5f * robots[i].transform.up, timer / actionTime);
                    robots[i].transform.rotation = Quaternion.Lerp(Quaternion.Euler(0, robotCurrentRotations[i], 0), Quaternion.Euler(0, robotCurrentRotations[i] - 180f, 0), timer / actionTime);
                }
                else if (robotActions[i][actionIndex] == MiraeGwanDemoRobotAction.DOWN)
                {
                    robots[i].transform.position = Vector3.Lerp(robotCurrentPositions[i], robotCurrentPositions[i] - 6.5f * robots[i].transform.up, timer / actionTime);
                    robots[i].transform.rotation = Quaternion.Lerp(Quaternion.Euler(0, robotCurrentRotations[i], 0), Quaternion.Euler(0, robotCurrentRotations[i] + 180f, 0), timer / actionTime);
                }
                if (timer >= actionTime)
                {
                    if (actionIndex + 2 < robotActions[i].Count && robotActions[i][actionIndex + 2] == MiraeGwanDemoRobotAction.UP)
                    {
                        elevator1DoorLeftCurrentPosition += 0.7f * Vector3.right;
                        elevator1DoorRightCurrentPosition -= 0.7f * Vector3.right;
                    }

                    if (actionIndex + 1 < robotActions[i].Count && robotActions[i][actionIndex + 1] == MiraeGwanDemoRobotAction.UP)
                    {
                        elevator1DoorLeftCurrentPosition -= 0.7f * Vector3.right;
                        elevator1DoorRightCurrentPosition += 0.7f * Vector3.right;
                    }

                    if (actionIndex - 1 >= 0 && actionIndex - 1 < robotActions[i].Count && robotActions[i][actionIndex - 1] == MiraeGwanDemoRobotAction.UP)
                    {
                        elevator2DoorLeftCurrentPosition += 0.7f * Vector3.right;
                        elevator2DoorRightCurrentPosition -= 0.7f * Vector3.right;
                    }

                    if (actionIndex - 2 >= 0 && actionIndex - 2 < robotActions[i].Count && robotActions[i][actionIndex - 2] == MiraeGwanDemoRobotAction.UP)
                    {
                        elevator2DoorLeftCurrentPosition -= 0.7f * Vector3.right;
                        elevator2DoorRightCurrentPosition += 0.7f * Vector3.right;
                    }

                    if (actionIndex + 2 < robotActions[i].Count && robotActions[i][actionIndex + 2] == MiraeGwanDemoRobotAction.DOWN)
                    {
                        elevator2DoorLeftCurrentPosition += 0.7f * Vector3.right;
                        elevator2DoorRightCurrentPosition -= 0.7f * Vector3.right;
                    }

                    if (actionIndex + 1 < robotActions[i].Count && robotActions[i][actionIndex + 1] == MiraeGwanDemoRobotAction.DOWN)
                    {
                        elevator2DoorLeftCurrentPosition -= 0.7f * Vector3.right;
                        elevator2DoorRightCurrentPosition += 0.7f * Vector3.right;
                    }

                    if (actionIndex - 1 >= 0 && actionIndex - 1 < robotActions[i].Count && robotActions[i][actionIndex - 1] == MiraeGwanDemoRobotAction.DOWN)
                    {
                        elevator1DoorLeftCurrentPosition += 0.7f * Vector3.right;
                        elevator1DoorRightCurrentPosition -= 0.7f * Vector3.right;
                    }

                    if (actionIndex - 2 >= 0 && actionIndex - 2 < robotActions[i].Count && robotActions[i][actionIndex - 2] == MiraeGwanDemoRobotAction.DOWN)
                    {
                        elevator1DoorLeftCurrentPosition -= 0.7f * Vector3.right;
                        elevator1DoorRightCurrentPosition += 0.7f * Vector3.right;
                    }

                    if (robotActions[i][actionIndex] == MiraeGwanDemoRobotAction.FORWARD)
                    {
                        robotCurrentPositions[i] += robots[i].transform.forward;
                    }
                    else if (robotActions[i][actionIndex] == MiraeGwanDemoRobotAction.LEFT)
                    {
                        robotCurrentRotations[i] -= 90f;
                    }
                    else if (robotActions[i][actionIndex] == MiraeGwanDemoRobotAction.RIGHT)
                    {
                        robotCurrentRotations[i] += 90f;
                    }
                    else if (robotActions[i][actionIndex] == MiraeGwanDemoRobotAction.UP)
                    {
                        robotCurrentPositions[i] += 6.5f * robots[i].transform.up;
                        robotCurrentRotations[i] -= 180f;
                    }
                    else if (robotActions[i][actionIndex] == MiraeGwanDemoRobotAction.DOWN)
                    {
                        robotCurrentPositions[i] -= 6.5f * robots[i].transform.up;
                        robotCurrentRotations[i] += 180f;
                    }
                    robots[i].transform.position = robotCurrentPositions[i];
                    robots[i].transform.rotation = Quaternion.Euler(0, robotCurrentRotations[i], 0);
                    elevator1DoorLeft.transform.position = elevator1DoorLeftCurrentPosition;
                    elevator1DoorRight.transform.position = elevator1DoorRightCurrentPosition;
                    elevator2DoorLeft.transform.position = elevator2DoorLeftCurrentPosition;
                    elevator2DoorRight.transform.position = elevator2DoorRightCurrentPosition;
                }
            }
        }
        if (timer >= actionTime)
        {
            actionIndex += 1;
            timer = 0;

            if (robotFlagActions.ContainsKey(actionIndex))
            {
                foreach (KeyValuePair<int, Vector3> kv in robotFlagActions[actionIndex])
                {
                    robotFlags[kv.Key].SetActive(true);
                    robotFlags[kv.Key].transform.position = kv.Value;
                }
            }
        }


        //if (actionIndex >= 0 && actionIndex < actions.Count)
        //{
        //    timer += Time.deltaTime;
        //    if (actions[actionIndex] == MiraeGwanDemoRobotAction.FORWARD)
        //    {
        //        transform.position = Vector3.Lerp(currentPosition, currentPosition + transform.forward, timer / actionTime);
        //    }
        //    else if (actions[actionIndex] == MiraeGwanDemoRobotAction.LEFT)
        //    {
        //        transform.rotation = Quaternion.Lerp(Quaternion.Euler(0, currentRotation, 0), Quaternion.Euler(0, currentRotation - 90f, 0), timer / actionTime);
        //    }
        //    else if (actions[actionIndex] == MiraeGwanDemoRobotAction.RIGHT)
        //    {
        //        transform.rotation = Quaternion.Lerp(Quaternion.Euler(0, currentRotation, 0), Quaternion.Euler(0, currentRotation + 90f, 0), timer / actionTime);
        //    }
        //    else if (actions[actionIndex] == MiraeGwanDemoRobotAction.UP)
        //    {
        //        transform.position = Vector3.Lerp(currentPosition, currentPosition + transform.up, timer / actionTime);
        //    }
        //    else if (actions[actionIndex] == MiraeGwanDemoRobotAction.DOWN)
        //    {
        //        transform.position = Vector3.Lerp(currentPosition, currentPosition - transform.up, timer / actionTime);
        //    }
        //    if (timer >= actionTime)
        //    {
        //        if (actions[actionIndex] == MiraeGwanDemoRobotAction.FORWARD)
        //        {
        //            currentPosition += transform.forward;
        //        }
        //        else if (actions[actionIndex] == MiraeGwanDemoRobotAction.LEFT)
        //        {
        //            currentRotation -= 90f;
        //        }
        //        else if (actions[actionIndex] == MiraeGwanDemoRobotAction.RIGHT)
        //        {
        //            currentRotation += 90f;
        //        }
        //        else if (actions[actionIndex] == MiraeGwanDemoRobotAction.UP)
        //        {
        //            currentPosition += transform.up;
        //        }
        //        else if (actions[actionIndex] == MiraeGwanDemoRobotAction.DOWN)
        //        {
        //            currentPosition -= transform.up;
        //        }
        //        transform.position = currentPosition;
        //        transform.rotation = Quaternion.Euler(0, currentRotation, 0);
        //        actionIndex += 1;
        //        timer = 0;
        //    }
        //}
    }
}
