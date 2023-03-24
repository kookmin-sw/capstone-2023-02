using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planner : MonoBehaviour
{
    public bool __DEBUG__ = true;

    public bool ignoreCollision = false;
    public float scale;
    public GameObject block;
    public GameObject plane;
    // For Topview
    public GameObject camera;
    public MapData mapData;

    // For Map
    private int rows;
    private int cols;
    private int edges;
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
    private SortedSet<Interval>[,] vertexCollisionInterval;
    private SortedSet<Interval>[,,] edgeCollisionInterval;
    private float unitTime = 2f;

    // Start is called before the first frame update
    void Start()
    {
        // Generate Map
        rows = map.GetLength(0);
        cols = map.GetLength(1);
        edges = Motion.positionMotions.Length;
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
        vertexCollisionInterval = new SortedSet<Interval>[rows, cols];
        edgeCollisionInterval = new SortedSet<Interval>[rows, cols, edges];
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
            {
                vertexCollisionInterval[r, c] = new SortedSet<Interval>();
                for (int e = 0; e < edges; e++)
                    edgeCollisionInterval[r, c, e] = new SortedSet<Interval>();
            }

        // Test
    }

    // Update is called once per frame
    void Update()
    {
        // Thread
        if (planQueue.Count > 0) makePlan();
    }

    public void requestPlan(Robot robot) { planQueue.Enqueue(robot, -robot.requestTime); }

    void makePlan(int limitIteration = 1000)
    {
        if (__DEBUG__) Debug.Log("Make Plan");
        Robot robot = planQueue.Peek();
        int iteration = 0;
        float[,,] g = new float[rows, cols, edges + 1];
        float[,,] h = new float[rows, cols, edges + 1];
        bool[,,] v = new bool[rows, cols, edges]; // visited
        PriorityQueue<State, float> open = new PriorityQueue<State, float>(Comparer<float>.Default);
        // If robot has no path by collision
        // robot goes to route and waits few seconds, and plans again
        PriorityQueue<State, float> route = new PriorityQueue<State, float>(Comparer<float>.Default);
        State start = new State(robot.source,
                                robot.requestTime,
                                new Interval(robot.requestTime, robot.requestTime + unitTime));
        State goal = null;

        h[(int)robot.source.x, (int)robot.source.z, edges] = g[(int)robot.source.x, (int)robot.source.z, edges] = 0;
        open.Enqueue(start, 0f);
        while (open.Count > 0 && iteration < limitIteration && goal == null)
        {
            // remove smallest h-value from open
            State curr = open.Dequeue();
            int r = (int)curr.position.x, c = (int)curr.position.z, e = curr.motion.edge;
            if (__DEBUG__) Debug.Log(r + ", " + c + ", " + e);
            List<State> successor = getSuccessors(curr);
            curr.successorCount = successor.Count;
            foreach (State next in successor)
            {
                if (__DEBUG__) Debug.Log("next: " + next);
                int nr = (int)next.position.x, nc = (int)next.position.z, ne = next.motion.edge;
                if (!v[nr, nc, ne]) h[nr, nc, ne] = g[nr, nc, ne] = Mathf.Infinity;
                if (g[nr, nc, ne] > g[r, c, e] + next.motion.time)
                {
                    if (__DEBUG__) Debug.Log("\t" + nr + ", " + nc + ", " + ne);
                    v[nr, nc, ne] = true;
                    g[nr, nc, ne] = g[r, c, e] + next.motion.time;

                    Vector3 apart = robot.destination - next.position;
                    h[nr, nc, ne] = g[r, c, e] + Mathf.Abs(apart.x) + Mathf.Abs(apart.z);
                    if (nr == (int)robot.destination.x &&
                        nc == (int)robot.destination.z)
                        goal = next;

                    curr.openCount++;
                    open.Enqueue(next, -h[nr, nc, ne]);
                }
            }

            // There is no way casued by collision 
            if (curr.openCount == 0 && curr.collisionCount > 0)
            {
                // Wait a second
                State next = null;
                Motion motion = new Motion(1f); // wait
                float time = curr.time + motion.time;
                float beginTime = curr.interval.begin + motion.time;
                float endTime = curr.interval.end + motion.time;
                Interval interval = new Interval(beginTime, endTime);
                if (!isCollisionOccur(interval, r, c))
                {
                    next = new State(curr, curr.position, motion, time, interval);
                }
                else
                {
                    int pr = (int)curr.previousState.position.x, pc = (int)curr.previousState.position.z;
                    // if (!isCollisionOccur(interval, pr, pc)) next =
                    // TODO return prev node
                }

                // if(next != null) open.Enqueue(next, 0);
                v[r, c, e] = false; // refuse current movement
            }

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
            iteration++;
        }

        // Backtracking
        State track = goal;
        Stack<Motion> reversedMotion = new Stack<Motion>();
        while (track != null)
        {
            if (__DEBUG__) Debug.Log("[" + (reversedMotion.Count + 1) + "] " + track);
            vertexCollisionInterval[(int)track.position.x, (int)track.position.z].Add(track.interval);
            reversedMotion.Push(track.motion);
            track = track.previousState;
        }
        while (reversedMotion.Count > 0) robot.motionToPath.Enqueue(reversedMotion.Pop());
        if (__DEBUG__) Debug.Log("Total Length: " + robot.motionToPath.Count);
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
        foreach (Motion motion in Motion.positionMotions)
        {
            Vector3 position = state.position + motion.deltaPosition;
            int r = (int)position.x, c = (int)position.z;
            if (r < 0 || r >= rows || c < 0 || c >= cols) continue; // out of map
            if (!map[r, c]) continue; // obstacle
            float time = state.time + motion.time;
            float beginTime = state.interval.begin + motion.time;
            float endTime = state.interval.end + motion.time;
            Interval interval = new Interval(beginTime, endTime);
            if (isCollisionOccur(interval, r, c))
            {
                state.collisionCount++;
                if (!ignoreCollision) continue;
            }

            successor.Add(new State(state, position, motion, time, interval));
        }

        return successor;
    }

    // Implement Collision Check
    // TODO: Implement O(log n) finding algorithm (lowerbound)
    //          - vertexCollisionInterval Data Type to Set(Red-Black Tree)
    bool isCollisionOccur(Interval interval, int r, int c)
    {
        foreach (Interval collision in vertexCollisionInterval[r, c])
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
                return true;
            }
        }
        return false;
    }
}
