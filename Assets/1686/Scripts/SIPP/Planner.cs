
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SIPP
{
    public class Planner : MonoBehaviour
    {
        private float unitTime = 1f;
        public MapData mapData;
        private List<Interval>[][] safeIntervals;
        private PriorityQueue<Robot, float> requests = new PriorityQueue<Robot, float>(Comparer<float>.Default);
        private int rows;
        private int cols;
        public float scale = 1f;
        public GameObject floorPrefab;
        public GameObject obstaclePrefab;
        public GameObject robotPrefab;
        public bool displayMap = true;


        // Start is called before the first frame update
        void Start()
        {
            rows = (int)mapData.dimension.x;
            cols = (int)mapData.dimension.y;
            safeIntervals = new List<Interval>[rows][];
            for (int r = 0; r < rows; r++)
            {
                safeIntervals[r] = new List<Interval>[cols];
                for (int c = 0; c < cols; c++)
                {
                    safeIntervals[r][c] = new List<Interval>();
                    safeIntervals[r][c].Add(new Interval(0, Mathf.Infinity));
                }
            }
            foreach (Vector2 obstacle in mapData.obstacles)
                safeIntervals[(int)obstacle.x][(int)obstacle.y].Clear();

            if (displayMap) Display();
        }



        // Update is called once per frame
        void Update()
        {
            if (requests.Count > 0) MakePlan();
        }

        public void RequestPlan(Robot robot)
        {
            requests.Enqueue(robot, -robot.requestTime);
        }


        void MakePlan(int limitIteration = 1000)
        {
            GeneralClass.Log("Make Plan");
            int iteration = 0;
            Robot robot = requests.Peek();
            GridMap grid = new GridMap(safeIntervals);
            PriorityQueue<State, float> open = new PriorityQueue<State, float>(Comparer<float>.Default);

            int gr = (int)robot.destination.x, gc = (int)robot.destination.z;
            int r = (int)robot.source.x, c = (int)robot.source.z;
            State curr = new State(robot.source,
                                   new Interval(Time.time, Time.time + unitTime),
                                   Time.time);

            int idx = LowerboundIndex<Interval>(safeIntervals[r][c], curr.interval);
            GridUnit unit = grid.map[r][c][idx];
            unit.gValue = unit.hValue = 0;
            unit.visited = true;

            open.Enqueue(curr, unit.hValue);
            while (open.Count > 0 && iteration < limitIteration)
            {
                // remove smallest h-value from open
                curr = open.Dequeue();
                r = (int)curr.position.x;
                c = (int)curr.position.z;
                // check robot reached destination
                if (r == gr && c == gc) break;
                idx = LowerboundIndex<Interval>(safeIntervals[r][c], curr.interval);
                unit = grid.map[r][c][idx];
                GeneralClass.Log("curr: " + curr, GeneralClass.debugLevel.low);
                List<State> successors = GetSuccessors(curr);
                foreach (State next in successors)
                {
                    int nr = (int)next.position.x, nc = (int)next.position.z, ni = LowerboundIndex<Interval>(safeIntervals[nr][nc], next.interval);
                    GridUnit nUnit = grid.map[nr][nc][ni];
                    if (!nUnit.visited) nUnit.gValue = nUnit.hValue = Mathf.Infinity;
                    if (nUnit.gValue > unit.gValue + next.motion.time)
                    {
                        GeneralClass.Log("\tnext: " + next, GeneralClass.debugLevel.low);
                        nUnit.visited = true;
                        nUnit.gValue = unit.gValue + next.motion.time;

                        Vector3 apart = robot.destination - next.position;
                        nUnit.hValue = unit.gValue + Mathf.Abs(apart.x) + Mathf.Abs(apart.z);

                        open.Enqueue(next, -nUnit.hValue);
                    }
                }
                iteration++;
            }

            if (r != gr || c != gc)
            {
                requests.Dequeue();
                GeneralClass.Log("Planning Fail");
                return;
            }
            // Backtracking
            State track = curr;
            Stack<Motion> reversedMotion = new Stack<Motion>();
            while (track != null)
            {
                GeneralClass.Log("[" + (reversedMotion.Count + 1) + "] " + track);
                // safeIntervals[(int)track.position.x][(int)track.position.z].Add(track.interval);
                reversedMotion.Push(track.motion);
                track = track.prev;
            }
            while (reversedMotion.Count > 0) robot.motionToPath.Enqueue(reversedMotion.Pop());
            GeneralClass.Log("Total Length: " + robot.motionToPath.Count);
            requests.Dequeue();
            GeneralClass.Log("Planning Successful");
        }

        public int LowerboundIndex<T>(List<T> list, T item, int startIdx = 0) where T : class, IComparable<T>
        {
            int idx = startIdx;
            for (; idx < list.Count; idx++)
                if (item.CompareTo(list[idx]) >= 0)
                    break;
            return idx;
        }

        List<State> GetSuccessors(State state)
        {
            List<State> successors = new List<State>();
            Robot robot = requests.Peek();
            foreach (Motion motion in Motion.positionMotions)
            {
                Vector3 position = state.position + motion.deltaPosition;
                int r = (int)position.x, c = (int)position.z;
                if (r < 0 || r >= rows || c < 0 || c >= cols) continue; // out of map
                if (safeIntervals[r][c].Count == 0) continue; // obstacle
                float time = state.time + motion.time;
                float beginTime = state.interval.begin + motion.time;
                float endTime = state.interval.end + motion.time;
                Interval interval = new Interval(beginTime, endTime);
                // if (isCollisionOccur(interval, r, c))
                // {
                //     state.collisionCount++;
                //     if (!ignoreCollision) continue;
                // }

                State success = new State(position, interval, time);
                success.motion = motion;
                success.prev = state;
                successors.Add(success);
            }
            return successors;
        }


        void Display()
        {
            Camera camera = Camera.main;
            float sqrScale = scale * scale;
            // Setting Topview
            camera.transform.position = new Vector3(rows / 2f * scale,
                                                    Mathf.Max(rows, cols) * scale * 2f,
                                                    cols / 2f * scale);
            camera.transform.LookAt(new Vector3(rows / 2f * scale,
                                                0f,
                                                cols / 2f * scale));
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    GameObject instance;
                    if (safeIntervals[r][c].Count > 0)
                        instance = floorPrefab;
                    else
                        instance = obstaclePrefab;
                    instance = Instantiate(instance,
                                           new Vector3(scale * r, 0f, scale * c),
                                           Quaternion.Euler(0f, 0f, 0f));
                    instance.transform.localScale *= scale;
                    instance.name = "floor" + r + ", " + c;
                }
            }

            for (int i = 0; i < mapData.starts.Count; i++)
            {
                Vector2 start = mapData.starts[i], goal = mapData.goals[i];
                GameObject instance = Instantiate(robotPrefab,
                                               new Vector3(scale * start.x, 0f, scale * start.y),
                                               Quaternion.Euler(0f, 0f, 0f));
                instance.transform.localScale *= scale;
                Robot robot = instance.AddComponent<Robot>();
                robot.planner = this;
                robot.speed = scale;
                robot.Request(start, goal);
                ColorRobot(robot, start, goal);
            }
        }

        void ColorRobot(Robot robot, Vector2 start, Vector2 goal)
        {
            Color color = UnityEngine.Random.ColorHSV();
            robot.gameObject.GetComponent<Renderer>().material.SetColor("_Color", color);
            //find by name
            GameObject.Find("floor" + (int)start.x + ", " + (int)start.y)
                      .GetComponent<Renderer>().material.SetColor("_Color", color);
            GameObject.Find("floor" + (int)goal.x + ", " + (int)goal.y)
                      .GetComponent<Renderer>().material.SetColor("_Color", color);
        }
    }
}