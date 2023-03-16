using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentState
{
    public int time;
    public int agentIndex;
    public Vector2 location;
    public int direction;

    public AgentState(int time, int agentIndex, Vector2 location, int direction)
    {
        this.time = time;
        this.agentIndex = agentIndex;
        this.location = location;
        this.direction = direction;
    }

    public AgentState(AgentState agentState)
    {
        time = agentState.time;
        agentIndex = agentState.agentIndex;
        location = agentState.location;
        direction = agentState.direction;
    }

    public override bool Equals(object obj)
    {
        AgentState other = (AgentState)obj;
        return time == other.time && agentIndex == other.agentIndex && location == other.location && direction == other.direction;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

public class Conflict
{
    public enum CONFLICT_TYPE { NO_CONFLICT, VERTEX, EDGE };

    public int time;
    public CONFLICT_TYPE type;

    public int agent1;
    public int agent2;

    public Vector2 vertexLocation;

    public Vector2 edgeLocation1;
    public Vector2 edgeLocation2;

    public Conflict()
    {
        type = CONFLICT_TYPE.NO_CONFLICT;
    }

    public Conflict(int time, int agent1, int agent2, Vector2 vertexLocation)
    {
        type = CONFLICT_TYPE.VERTEX;
   
        this.time = time;

        this.agent1 = agent1;
        this.agent2 = agent2;

        this.vertexLocation = vertexLocation;
    }

    public Conflict(int time, int agent1, int agent2, Vector2 edgeLocation1, Vector2 edgeLocation2)
    {
        type = CONFLICT_TYPE.EDGE;
      
        this.time = time;

        this.agent1 = agent1;
        this.agent2 = agent2;

        this.edgeLocation1 = edgeLocation1;
        this.edgeLocation2 = edgeLocation2;
    }
}

public class VertexConstraint
{
    public int time;
    public Vector2 location;

    public VertexConstraint(int time, Vector2 location)
    {
        this.time = time;
        this.location = location;
    }

    public VertexConstraint(VertexConstraint vertexConstraint)
    {
        this.time = vertexConstraint.time;
        this.location = vertexConstraint.location;
    }

    public override bool Equals(object obj)
    {
        VertexConstraint other = (VertexConstraint)obj;
        return time == other.time && location == other.location;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

public class EdgeConstraint
{
    public int time;
    public Vector2 location1;
    public Vector2 location2;

    public EdgeConstraint(int time, Vector2 location1, Vector2 location2)
    {
        this.time = time;
        this.location1 = location1;
        this.location2 = location2;
    }

    public EdgeConstraint(EdgeConstraint edgeConstraint)
    {
        this.time = edgeConstraint.time;
        this.location1 = edgeConstraint.location1;
        this.location2 = edgeConstraint.location2;
    }

    public override bool Equals(object obj)
    {
        EdgeConstraint other = (EdgeConstraint)obj;
        return time == other.time && location1 == other.location1 && location2 == other.location2;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

public class Constraints
{
    public List<VertexConstraint> vertexConstraints;
    public List<EdgeConstraint> edgeConstraints;

    public Constraints()
    {
        vertexConstraints = new List<VertexConstraint>();
        edgeConstraints = new List<EdgeConstraint>();
    }

    public Constraints(Constraints constraints)
    {
        vertexConstraints = new List<VertexConstraint>();
        edgeConstraints = new List<EdgeConstraint>();

        foreach(VertexConstraint vertexConstraint in constraints.vertexConstraints)
        {
            vertexConstraints.Add(new VertexConstraint(vertexConstraint));
        }

        foreach(EdgeConstraint edgeConstraint in constraints.edgeConstraints)
        {
            edgeConstraints.Add(new EdgeConstraint(edgeConstraint));
        }
    }

    public void addConstraint(Constraints constraints)
    {
        foreach(VertexConstraint vertexConstraint in constraints.vertexConstraints)
        {
            vertexConstraints.Add(new VertexConstraint(vertexConstraint));
        }
        foreach(EdgeConstraint edgeConstraint in constraints.edgeConstraints)
        {
            edgeConstraints.Add(new EdgeConstraint(edgeConstraint));
        }
    }
}

public class Environment
{
    public Vector2 dimension;

    public Vector2[] obstacles;

    public Vector2[] starts;
    public int[] directions;
    public Vector2[] goals;

    public int numberOfAgents;

    public Constraints[] constraints;

    public Environment(Vector2 dimension, Vector2[] obstacles, Vector2[] starts, int[] directions, Vector2[] goals)
    {
        numberOfAgents = starts.Length;

        this.dimension = dimension;

        this.obstacles = obstacles;

        this.starts = starts;
        this.directions = directions;
        this.goals = goals;

        constraints = new Constraints[numberOfAgents];
        for (int i = 0; i < numberOfAgents; i++)
        {
            constraints[i] = new Constraints();
        }
    }

    public Environment(Environment environment)
    {
        numberOfAgents = environment.numberOfAgents;

        dimension = environment.dimension;

        obstacles = (Vector2[])environment.obstacles.Clone();

        starts = (Vector2[])environment.starts.Clone();
        directions = (int[])environment.directions.Clone();
        goals = (Vector2[])environment.goals.Clone();

        constraints = new Constraints[numberOfAgents];
        for (int i = 0; i < numberOfAgents; i++)
        {
            constraints[i] = new Constraints(environment.constraints[i]);
        }
    }

    public List<AgentState> getNeighbors(AgentState agentState)
    {
        List<AgentState> neighbors = new List<AgentState>();

        AgentState waitActionState = new AgentState(agentState.time + 1, agentState.agentIndex, agentState.location, agentState.direction);
        if (isValidState(waitActionState)) neighbors.Add(waitActionState);

        AgentState turnRightActionState = new AgentState(agentState.time + 1, agentState.agentIndex, agentState.location, (agentState.direction + 1) % 4);
        if (isValidState(turnRightActionState)) neighbors.Add(turnRightActionState);

        AgentState turnLeftActionState = new AgentState(agentState.time + 1, agentState.agentIndex, agentState.location, (agentState.direction - 1 + 4) % 4);
        if (isValidState(turnLeftActionState)) neighbors.Add(turnLeftActionState);

        Vector2 goLocation = agentState.location;
        if (agentState.direction == 0) goLocation.y += 1;
        else if (agentState.direction == 1) goLocation.x += 1;
        else if (agentState.direction == 2) goLocation.y -= 1;
        else if (agentState.direction == 3) goLocation.x -= 1;
        AgentState goActionState = new AgentState(agentState.time + 1, agentState.agentIndex, goLocation, agentState.direction);
        if (isValidState(goActionState) && isValidTransition(agentState, goActionState)) neighbors.Add(goActionState);

        return neighbors;
    }


    public bool isValidState(AgentState agentState)
    {
        if (agentState.location.x < 0 || agentState.location.x >= dimension[0] || agentState.location.y < 0 || agentState.location.y >= dimension[1]) return false;

        foreach(VertexConstraint vertexConstraint in constraints[agentState.agentIndex].vertexConstraints)
        {
            if (vertexConstraint.Equals(new VertexConstraint(agentState.time, agentState.location)))
            {
                return false;
            }
        }

        foreach(Vector2 obstacle in obstacles)
        {
            if (obstacle == agentState.location)
            {
                return false;
            }
        }

        return true;
    }

    public bool isValidTransition(AgentState agentState1, AgentState agentState2)
    {
        foreach (EdgeConstraint edgeConstraint in constraints[agentState1.agentIndex].edgeConstraints)
        {
            if (edgeConstraint.Equals(new EdgeConstraint(agentState1.time, agentState1.location, agentState2.location)))
            {
                return false;
            }
        }
        return true;
    }

}

public class HighLevelNode
{
    public Environment environment;

    public List<AgentState>[] solution;
    public int cost;

    public HighLevelNode(Environment environment)
    {
        this.environment = new Environment(environment);

        solution = new List<AgentState>[this.environment.numberOfAgents];
        for (int i = 0; i < this.environment.numberOfAgents; i++)
        {
            solution[i] = new List<AgentState>();
        }
        cost = 0;
    }

    public HighLevelNode(HighLevelNode highLevelNode)
    {
        environment = new Environment(highLevelNode.environment);

        solution = new List<AgentState>[environment.numberOfAgents];
        for (int i = 0; i < environment.numberOfAgents; i++)
        {
            solution[i] = new List<AgentState>();
            for (int j = 0; j < highLevelNode.solution[i].Count; j++)
            {
                solution[i].Add(new AgentState(highLevelNode.solution[i][j]));
            }
        }
        cost = highLevelNode.cost;
    }

    public Conflict getFirstConflict()
    {
        int maxTime = 0;
        for (int i = 0; i < environment.numberOfAgents; i++)
        {
            if (maxTime < solution[i].Count) maxTime = solution[i].Count;
        }

        for (int t = 0; t < maxTime; t++)
        {
            for (int agent1 = 0; agent1 < environment.numberOfAgents; agent1++)
            {
                for (int agent2 = agent1 + 1; agent2 < environment.numberOfAgents; agent2++)
                {
                    if (solution[agent1].Count == 0 || solution[agent2].Count == 0) continue;
                    AgentState nowAgent1State = solution[agent1][Mathf.Min(solution[agent1].Count - 1, t)];
                    AgentState nowAgent2State = solution[agent2][Mathf.Min(solution[agent2].Count - 1, t)];
                    if (nowAgent1State.location == nowAgent2State.location)
                    {
                        return new Conflict(t, agent1, agent2, nowAgent1State.location);
                    }
                }
            }

            for (int agent1 = 0; agent1 < environment.numberOfAgents; agent1++)
            {
                for (int agent2 = agent1 + 1; agent2 < environment.numberOfAgents; agent2++)
                {
                    if (solution[agent1].Count == 0 || solution[agent2].Count == 0) continue;
                    AgentState agent1StateA = solution[agent1][Mathf.Min(solution[agent1].Count - 1, t)];
                    AgentState agent1StateB = solution[agent1][Mathf.Min(solution[agent1].Count - 1, t + 1)];
                    AgentState agent2StateA = solution[agent2][Mathf.Min(solution[agent2].Count - 1, t)];
                    AgentState agent2StateB = solution[agent2][Mathf.Min(solution[agent2].Count - 1, t + 1)];

                    if (agent1StateA.location == agent2StateB.location && agent1StateB.location == agent2StateA.location)
                    {
                        return new Conflict(t, agent1, agent2, agent1StateA.location, agent2StateA.location);
                    }
                }
            }
        }
        return new Conflict();
    }

    public float getHeuristicCost(Vector2 current, Vector2 goal)
    {
        return Mathf.Abs(current.x - goal.x) + Mathf.Abs(current.y - goal.y);
    }

    public bool computeAllSolution()
    {
        for (int agent = 0; agent < environment.numberOfAgents; agent++)
        {
            if (!computeAgentSolution(agent)) return false;
        }
        return true;
    }

    public bool computeAgentSolution(int agent)
    {
        cost -= solution[agent].Count;
        solution[agent].Clear();

        foreach (VertexConstraint vertexConstraint in environment.constraints[agent].vertexConstraints)
        {
            if (vertexConstraint == new VertexConstraint(0, environment.starts[agent]))
            {
                return false;
            }
        }

        List<AgentState> openSet = new List<AgentState>();
        List<AgentState> closeSet = new List<AgentState>();
        List<AgentState[]> cameFrom = new List<AgentState[]>();

        AgentState startState = new AgentState(0, agent, environment.starts[agent], environment.directions[agent]);

        openSet.Add(startState);

        List<float> timeCost = new List<float>();
        timeCost.Add(0);

        List<float> heuristicCost = new List<float>();
        heuristicCost.Add(getHeuristicCost(environment.starts[agent], environment.goals[agent]));

        while (openSet.Count != 0)
        {
            float minCost = float.PositiveInfinity;
            int minStateIndex = -1;
            for (int i = 0; i < openSet.Count; i++)
            {
                if (minCost > heuristicCost[i])
                {
                    minCost = heuristicCost[i];
                    minStateIndex = i;
                }
            }

            AgentState currentState = openSet[minStateIndex];

            //Debug.Log("----currentstate---");
            //Debug.Log(currentState.time);
            //Debug.Log(currentState.location);
            //Debug.Log(currentState.direction);
            //Debug.Log("-------");

            closeSet.Add(new AgentState(currentState));

            if (currentState.location == environment.goals[agent])
            {
                int now = -1;
                for (int i = cameFrom.Count - 1; i >= 0; i--)
                {
                    if (cameFrom[i][0].location == environment.goals[agent])
                    {
                        now = i;
                        break;
                    }
                }
                if (now == -1) return true;

                while (true)
                {
                    solution[agent].Insert(0, new AgentState(cameFrom[now][0]));
                    if (cameFrom[now][1].Equals(startState))
                    {
                        solution[agent].Insert(0, new AgentState(cameFrom[now][1]));
                        break;
                    }
                    for (int i = cameFrom.Count - 1; i >= 0; i--)
                    {
                        if (cameFrom[i][0].Equals(cameFrom[now][1]))
                        {
                            now = i;
                            break;
                        }
                    }
                }
                cost += solution[agent].Count;
                return true;
            }

            List<AgentState> neighbors = environment.getNeighbors(currentState);
            foreach (AgentState neighbor in neighbors)
            {
                //Debug.Log("----neighbor---");
                //Debug.Log(neighbor.time);
                //Debug.Log(neighbor.location);
                //Debug.Log(neighbor.direction);
                //Debug.Log("-------");

                bool isValidNeighbor = true;
                foreach (AgentState agentState in closeSet)
                {
                    if (neighbor.Equals(agentState))
                    {
                        isValidNeighbor = false;
                        break;
                    }
                }
                if (!isValidNeighbor) continue;

                int neighborOpenSetIndex = -1;
                for (int i = 0; i < openSet.Count; i++)
                {
                    AgentState agentState = openSet[i];
                    if (neighbor.Equals(agentState))
                    {
                        neighborOpenSetIndex = i;
                        break;
                    }
                }
                if (neighborOpenSetIndex == -1)
                {
                    timeCost.Add(timeCost[minStateIndex] + 1);
                    heuristicCost.Add(timeCost[minStateIndex] + getHeuristicCost(neighbor.location, environment.goals[neighbor.agentIndex]));

                    openSet.Add(neighbor);
                }
                else if (timeCost[minStateIndex] + 1 >= timeCost[minStateIndex]) continue;
                else
                {
                    timeCost[neighborOpenSetIndex] = timeCost[minStateIndex] + 1;
                    heuristicCost[neighborOpenSetIndex] = timeCost[minStateIndex] + getHeuristicCost(neighbor.location, environment.goals[neighbor.agentIndex]);
                }
                cameFrom.Add(new AgentState[2] { neighbor, currentState });
            }

            openSet.RemoveAt(minStateIndex);
            timeCost.RemoveAt(minStateIndex);
            heuristicCost.RemoveAt(minStateIndex);
        }

        return false;
    }
}


public class CBS : MonoBehaviour
{
    public MapData mapData;

    public GameObject floor;
    public GameObject robotPrefab;
    public GameObject obstaclePrefab;
    public GameObject flagPrefab;

    public List<GameObject> robots;

    public float simulationTime;
    public bool done;
    public bool autoSimulation;
    public float simulationSpeed;
    public List<AgentState>[] solution;

    // Start is called before the first frame update
    void Start()
    {
        simulationTime = 0;
        autoSimulation = false;
        simulationSpeed = 1f;
        done = false;

        Vector2 dimension = mapData.dimension;
        Vector2[] obstacles = mapData.obstacles;
        Vector2[] starts = mapData.starts.ToArray();
        int[] directions = mapData.directions.ToArray();
        Vector2[] goals = mapData.goals.ToArray();

        floor.transform.localScale = new Vector3(dimension.x, 0.1f, dimension.y);
        floor.transform.position = new Vector3(dimension.x / 2f - 0.5f, 0, dimension.y / 2f - 0.5f);
        floor.GetComponent<Renderer>().material.SetColor("_Color", Color.grey);

        Camera.main.transform.position = new Vector3(dimension.x / 2f - 0.5f, dimension.x * 2f, dimension.y / 2f - 0.5f);

        for (int i = 0; i < starts.Length; i++)
        {
            Color color = Random.ColorHSV();

            GameObject robot = Instantiate(robotPrefab, new Vector3(starts[i].x, 0.4f, starts[i].y), Quaternion.identity);
            robot.transform.localRotation = Quaternion.Euler(0, 90 * directions[i], 0);
            robot.GetComponent<Renderer>().material.SetColor("_Color", color);
            robots.Add(robot);

            GameObject flag = Instantiate(flagPrefab, new Vector3(goals[i].x, 0.051f, goals[i].y), Quaternion.identity);
            flag.GetComponent<Renderer>().material.SetColor("_Color", color);
        }

        for (int i = 0; i < obstacles.Length; i++)
        {
            Instantiate(obstaclePrefab, new Vector3(obstacles[i].x, 0.5f, obstacles[i].y), Quaternion.identity);
        }

        List<HighLevelNode> queue = new List<HighLevelNode>();
        HighLevelNode root = new HighLevelNode(new Environment(dimension, obstacles, starts, directions, goals));
        if (!root.computeAllSolution())
        {
            Debug.Log("Fail");
            return;
        }
        queue.Add(root);

        int MAXXX = 1000;

        while (queue.Count != 0)
        {
            MAXXX--;
            if (MAXXX < 0)
            {
                Debug.Log("Time out");
                break;
            }
            int minCost = int.MaxValue;
            int minNodeIndex = -1;
            for (int i = 0; i < queue.Count; i++)
            {
                if (minCost > queue[i].cost)
                {
                    minCost = queue[i].cost;
                    minNodeIndex = i;
                }
            }

            HighLevelNode nowNode = queue[minNodeIndex];

            //Debug.Log("=======================");
            //Debug.Log("Test " + minCost);
            //for (int i = 0; i < nowNode.environment.numberOfAgents; i++)
            //{
            //    string str = i + ":: ";
            //    foreach (AgentState agentState in nowNode.solution[i])
            //    {
            //        str += "[" + agentState.time + " " + agentState.location.x + "," + agentState.location.y + " " + agentState.direction;
            //        str += "//";
            //    }
            //    Debug.Log(str);
            //}

            Conflict firstConflict = nowNode.getFirstConflict();
            //Debug.Log(firstConflict.type);
            //Debug.Log(firstConflict.agent1);
            //Debug.Log(firstConflict.agent2);
            //Debug.Log(firstConflict.time);
            //Debug.Log(firstConflict.vertexLocation);
            if (firstConflict.type == Conflict.CONFLICT_TYPE.NO_CONFLICT)
            {
                Debug.Log("Found " + minCost);

                solution = new List<AgentState>[nowNode.environment.numberOfAgents];
                for (int i = 0; i < nowNode.environment.numberOfAgents; i++)
                {
                    solution[i] = new List<AgentState>();
                    string str = i + " :: ";
                    foreach(AgentState agentState in nowNode.solution[i])
                    {
                        solution[i].Add(new AgentState(agentState));
                        str += agentState.time + " " + agentState.location.x + "," + agentState.location.y + " " + agentState.direction;
                        str += " // ";
                    }
                    Debug.Log(str);
                }
                done = true;
                break;
            }
            else if (firstConflict.type == Conflict.CONFLICT_TYPE.VERTEX)
            {
                HighLevelNode agent1HighLevelNode = new HighLevelNode(nowNode);
                agent1HighLevelNode.environment.constraints[firstConflict.agent1].vertexConstraints.Add(new VertexConstraint(firstConflict.time, firstConflict.vertexLocation));
                if (agent1HighLevelNode.computeAgentSolution(firstConflict.agent1)) queue.Add(agent1HighLevelNode);

                HighLevelNode agent2HighLevelNode = new HighLevelNode(nowNode);
                agent2HighLevelNode.environment.constraints[firstConflict.agent2].vertexConstraints.Add(new VertexConstraint(firstConflict.time, firstConflict.vertexLocation));
                if (agent2HighLevelNode.computeAgentSolution(firstConflict.agent2)) queue.Add(agent2HighLevelNode);
            }
            else if (firstConflict.type == Conflict.CONFLICT_TYPE.EDGE)
            {
                HighLevelNode agent1HighLevelNode = new HighLevelNode(nowNode);
                agent1HighLevelNode.environment.constraints[firstConflict.agent1].edgeConstraints.Add(new EdgeConstraint(firstConflict.time, firstConflict.edgeLocation1, firstConflict.edgeLocation2));
                if (agent1HighLevelNode.computeAgentSolution(firstConflict.agent1)) queue.Add(agent1HighLevelNode);

                HighLevelNode agent2HighLevelNode = new HighLevelNode(nowNode);
                agent2HighLevelNode.environment.constraints[firstConflict.agent2].edgeConstraints.Add(new EdgeConstraint(firstConflict.time, firstConflict.edgeLocation2, firstConflict.edgeLocation1));
                if (agent2HighLevelNode.computeAgentSolution(firstConflict.agent2)) queue.Add(agent2HighLevelNode);
            }

            queue.RemoveAt(minNodeIndex);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (done)
        {
            for (int i = 0; i < robots.Count; i++)
            {
                if (solution[i].Count == 0) continue;

                if (simulationTime > solution[i][solution[i].Count - 1].time)
                {
                    robots[i].transform.position = new Vector3(solution[i][solution[i].Count - 1].location.x, 0.4f, solution[i][solution[i].Count - 1].location.y);
                    robots[i].transform.localRotation = Quaternion.Euler(0, 90 * solution[i][solution[i].Count - 1].direction, 0);
                    continue;
                }

                int actionIndex = -1;
                for (int j = 1; j < solution[i].Count; j++)
                {
                    if (simulationTime >= solution[i][j - 1].time && simulationTime <= solution[i][j].time)
                    {
                        actionIndex = j;
                        break;
                    }
                }

                if (actionIndex == -1) continue;

                Vector2 robotPos = Vector2.Lerp(solution[i][actionIndex - 1].location, solution[i][actionIndex].location, simulationTime - solution[i][actionIndex - 1].time);
                int previousDirection = solution[i][actionIndex - 1].direction;
                int nextDirection = solution[i][actionIndex].direction;
                if (previousDirection == 0 && nextDirection == 3) previousDirection += 4;
                if (previousDirection == 3 && nextDirection == 0) nextDirection += 4;
                float robotDirection = Mathf.Lerp(previousDirection, nextDirection, simulationTime - solution[i][actionIndex - 1].time);

                robots[i].transform.position = new Vector3(robotPos.x, 0.4f, robotPos.y);
                robots[i].transform.localRotation = Quaternion.Euler(0, 90 * robotDirection, 0);
            }
            if (autoSimulation) simulationTime += Time.deltaTime * simulationSpeed;
        }
    }
}
