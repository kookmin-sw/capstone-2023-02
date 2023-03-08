using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planner : MonoBehaviour
{
    public bool __DEBUG__ = true;

    public float scale;
    public GameObject block;
    public GameObject plane;
    // For Topview
    public GameObject camera;

    // For Map
    private int rows;
    private int cols;
    // true: has way, false: obstacle
    // private bool[,] map = {
    //     {true, true, true},
    //     {false, true, false},
    //     {true, true, true}
    // };
    private bool[,] map = {
        {true, true, true},
        {false, false, true},
        {true, true, true},
        {true, false, true},
        {true, true, true}
    };

    // For Making Plan
    private PriorityQueue<Robot, float> planQueue = new PriorityQueue<Robot, float>(Comparer<float>.Default);
    private SortedSet<Interval>[,] collisionInterval;
    private float unitTime = 2f;

    // Start is called before the first frame update
    void Start()
    {
        // Generate Map
        rows = map.GetLength(0);
        cols = map.GetLength(1);
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                GameObject instance;
                if (map[r, c])
                    instance = plane;
                else
                    instance = block;
                instance = Instantiate(instance, new Vector3(scale * r, 0f, scale * c), Quaternion.Euler(0f, 0f, 0f));
                instance.transform.localScale *= scale;
            }
        }
        float sqrScale = scale * scale;

        // Setting Topview
        camera.transform.position = new Vector3(rows * sqrScale,
                                                Mathf.Max(rows, cols) * scale * 2f,
                                                cols * sqrScale);
        camera.transform.LookAt(new Vector3(rows * sqrScale,
                                            0f,
                                            cols * sqrScale));

        // Planning Setting
        collisionInterval = new SortedSet<Interval>[rows, cols];
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                collisionInterval[r, c] = new SortedSet<Interval>();

        // Test
    }

    // Update is called once per frame
    void Update()
    {
        // Thread
        if (planQueue.Count > 0) makePlan();
    }

    public void requestPlan(Robot robot) { planQueue.Enqueue(robot, -robot.requestTime); }

    void makePlan()
    {
        if (__DEBUG__) Debug.Log("Make Plan");
        Robot robot = planQueue.Peek();
        float[,] g = new float[rows, cols];
        float[,] h = new float[rows, cols]; // heuritic = estimate time + euclid distance
        bool[,] v = new bool[rows, cols]; // visited
        State[,] p = new State[rows, cols]; // path
        PriorityQueue<State, float> open = new PriorityQueue<State, float>(Comparer<float>.Default);
        // If robot has no path by collision
        // robot goes to route and waits few seconds, and plans again
        PriorityQueue<State, float> route = new PriorityQueue<State, float>(Comparer<float>.Default);
        State start = new State(robot.source,
                                robot.requestTime,
                                new Interval(robot.requestTime, robot.requestTime + unitTime));
        h[(int)robot.source.x, (int)robot.source.z] = g[(int)robot.source.x, (int)robot.source.z] = 0;
        v[(int)robot.source.x, (int)robot.source.z] = true;
        open.Enqueue(start, 0f);
        while (open.Count > 0)
        {
            // remove smallest h-value from open
            State curr = open.Dequeue();
            int r = (int)curr.position.x, c = (int)curr.position.z;
            // Debug.Log(r + ", " + c);
            List<State> successor = getSuccessors(curr);
            foreach (State next in successor)
            {
                int nr = (int)next.position.x, nc = (int)next.position.z;
                if (!v[nr, nc]) h[nr, nc] = g[nr, nc] = Mathf.Infinity;
                if (g[nr, nc] > g[r, c] + next.motion.time)
                {
                    // Debug.Log("\t" + nr + ", " + nc);
                    v[nr, nc] = true;
                    g[nr, nc] = g[r, c] + next.motion.time;

                    Vector3 apart = robot.destination - next.position;
                    h[nr, nc] = g[r, c] + Mathf.Abs(apart.x) + Mathf.Abs(apart.z);
                    p[nr, nc] = next;

                    open.Enqueue(next, -h[nr, nc]);
                }
            }
            if (v[(int)robot.destination.x, (int)robot.destination.z]) break;

            if (__DEBUG__)
            {
                string log = "Open {\n";
                List<float> priorities = open.GetPriorities();
                List<State> elements = open.GetElements();
                for (int i = 0; i < priorities.Count; i++)
                    log += "\t[" + (i + 1) + "]: " + elements[i].position + " " + priorities[i] + ",\n";
                log += "}";
                Debug.Log(log);
            }
        }

        // Backtracking
        State track = p[(int)robot.destination.x, (int)robot.destination.z];
        Stack<Motion> reversedMotion = new Stack<Motion>();
        while (track != null)
        {
            if(__DEBUG__) Debug.Log("[" + (reversedMotion.Count + 1) + "] " + track);
            collisionInterval[(int)track.position.x, (int)track.position.z].Add(track.interval);
            reversedMotion.Push(track.motion);
            track = track.previousState;
        }
        while (reversedMotion.Count > 0) robot.motionToPath.Enqueue(reversedMotion.Pop());
        if(__DEBUG__) Debug.Log("Total Length: " + robot.motionToPath.Count);
        planQueue.Dequeue();

        if (__DEBUG__) Debug.Log("Done");
    }

    void backPropagation()
    {
        // Implement Need
    }

    List<State> getSuccessors(State state)
    {
        List<State> successor = new List<State>();
        Robot robot = planQueue.Peek();
        foreach (Motion motion in robot.availableMotions)
        {
            Vector3 position = state.position + motion.deltaPosition;
            int r = (int)position.x, c = (int)position.z;
            if (r < 0 || r >= rows || c < 0 || c >= cols) continue; // out of map
            if (!map[r, c]) continue; // obstacle
            float time = state.time + motion.time;
            float beginTime = state.interval.begin + motion.time;
            float endTime = state.interval.end + motion.time;
            Interval interval = new Interval(beginTime, endTime);

            // Implement Collision Check
            // TODO: Implement O(log n) finding algorithm (lowerbound)
            bool isCollisionOccur = false;
            foreach (Interval collision in collisionInterval[r, c])
            {
                if (__DEBUG__)
                {
                    Debug.Log(r + ", " + c);
                    Debug.Log("\tcollision(" + collision.begin + ", " + collision.end + ")");
                    Debug.Log("\tInterval(" + interval.begin + ", " + interval.end + ")");
                }
                if (collision.isOverlap(interval))
                {
                    if (__DEBUG__) Debug.Log("Collision Detect!");
                    isCollisionOccur = true;
                    break;
                }
            }
            if (isCollisionOccur) continue;

            successor.Add(new State(state, position, motion, time, interval));
        }

        return successor;
    }
}
