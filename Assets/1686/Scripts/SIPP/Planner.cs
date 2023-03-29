
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
                                   new Interval(robot.requestTime, robot.requestTime + unitTime),
                                   robot.requestTime);

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
                    GeneralClass.Log("\tcheck" + next.interval + ": " + nr + ", " + nc + ", " + ni, GeneralClass.debugLevel.low);
                    GridUnit nUnit = grid.map[nr][nc][ni];

                    GeneralClass.Log("\tcheck" + next.interval + ": " + safeIntervals[nr][nc][ni], GeneralClass.debugLevel.low);
                    if (!nUnit.visited) nUnit.gValue = nUnit.hValue = Mathf.Infinity;
                    if (nUnit.gValue > unit.gValue + next.time - curr.time + next.motion.time)
                    {
                        GeneralClass.Log("\tnext: " + next, GeneralClass.debugLevel.low);
                        nUnit.visited = true;
                        nUnit.gValue = unit.gValue + next.time - curr.time + next.motion.time;

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
            Stack<State> reversedMotion = new Stack<State>();
            while (track != null)
            {
                GeneralClass.Log("[" + (reversedMotion.Count + 1) + "] " + track);
                for (int i = 0; i < safeIntervals[(int)track.position.x][(int)track.position.z].Count; i++)
                    GeneralClass.Log("\t[" + (i + 1) + "]: " + safeIntervals[(int)track.position.x][(int)track.position.z][i],
                                     GeneralClass.debugLevel.low);
                SplitInterval(safeIntervals[(int)track.position.x][(int)track.position.z], track.interval);
                for (int i = 0; i < safeIntervals[(int)track.position.x][(int)track.position.z].Count; i++)
                    GeneralClass.Log("\t[" + (i + 1) + "]: " + safeIntervals[(int)track.position.x][(int)track.position.z][i],
                                     GeneralClass.debugLevel.low);
                reversedMotion.Push(track);
                track = track.prev;
            }
            while (reversedMotion.Count > 0) robot.stateToPath.Enqueue(reversedMotion.Pop());
            GeneralClass.Log("Total Length: " + robot.stateToPath.Count);
            requests.Dequeue();
            GeneralClass.Log("Planning Successful");
        }

        public int LowerboundIndex<T>(List<T> list, T item, int startIdx = 0) where T : class, IComparable<T>
        {
            int idx = startIdx;
            for (; idx < list.Count; idx++)
                if (list[idx].CompareTo(item) >= 0)
                    break;
            return idx;
        }

        void SplitInterval(List<Interval> list, Interval splitBy)
        {
            int idx = LowerboundIndex<Interval>(list, splitBy);
            Interval interval = list[idx];
            if (interval.begin == splitBy.begin)
                interval.begin = splitBy.end;
            else
            {
                float tmp = interval.end;
                interval.end = splitBy.begin;
                if (interval.end == splitBy.end) return;
                interval = new Interval(splitBy.end, tmp);
                list.Insert(idx + 1, interval);
            }
        }

        List<State> GetSuccessors(State state)
        {
            List<State> successors = new List<State>();
            Robot robot = requests.Peek();
            int r = (int)state.position.x, c = (int)state.position.z;
            foreach (Motion motion in Motion.positionMotions)
            {
                Vector3 position = state.position + motion.deltaPosition;
                int nr = (int)position.x, nc = (int)position.z;
                if (nr < 0 || nr >= rows || nc < 0 || nc >= cols) continue; // out of map
                int i = 0;
                for (int ni = 0; ni < safeIntervals[nr][nc].Count; ni++)
                {
                    // GeneralClass.Log("\t" + safeIntervals[nr][nc][ni]);
                    // compare with least entTime
                    if (safeIntervals[nr][nc][ni].end < state.interval.end + motion.time) continue;
                    float beginTime = Mathf.Max(state.interval.begin + motion.time, safeIntervals[nr][nc][ni].begin);
                    float endTime = beginTime + motion.time;

                    // collision check on next position
                    if (safeIntervals[nr][nc][ni].end < endTime) continue;

                    Interval interval = new Interval(beginTime, endTime);
                    // GeneralClass.Log("\t\t" + interval);
                    i = LowerboundIndex<Interval>(safeIntervals[r][c], interval, i);
                    // conllistion check on current position
                    if (i < safeIntervals[r][c].Count
                       && (safeIntervals[r][c][i].begin > interval.begin
                       || safeIntervals[r][c][i].end < interval.end)) continue;
                    // GeneralClass.Log("\tSuccess");

                    State success = new State(position, interval, beginTime);
                    success.motion = motion;
                    success.prev = state;
                    successors.Add(success);
                }
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