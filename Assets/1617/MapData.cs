using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Map Data", menuName = "Scriptable Map Data", order = 0)]
public class MapData : ScriptableObject
{
    public Vector2 dimension;
    public Vector2[] obstacles;
    public List<Vector2> starts;
    public List<int> directions;
    public List<Vector2> goals;

    public void generateRandomMap()
    {
        dimension.x = Random.Range(3, 10);
        dimension.y = Random.Range(3, 10);
        float obstacleDensity = Random.Range(0f, 0.8f);
        int numberOfObstacles = (int)(dimension.x * dimension.y * obstacleDensity);
        obstacles = new Vector2[numberOfObstacles];
        for (int i = 0; i < numberOfObstacles; i++)
        {
            while (true)
            {
                obstacles[i] = new Vector2(Random.Range(0, (int)dimension.x), Random.Range(0, (int)dimension.y));
                bool valid = true;
                for (int j = 0; j < i; j++)
                {
                    if (obstacles[j] == obstacles[i])
                    {
                        valid = false;
                        break;
                    }
                }
                if (valid) break;
            }
        }
    }

    public void addRobot()
    {
        while (true)
        {
            Vector2 newStart = new Vector2(Random.Range(0, (int)dimension.x), Random.Range(0, (int)dimension.y));
            bool valid = true;
            foreach (Vector2 start in starts)
            {
                if (start == newStart)
                {
                    valid = false;
                }
            }
            if (valid) starts.Add(newStart);
            else continue;
            directions.Add(Random.Range(0, 4));
            break;
        }
        while (true)
        {
            Vector2 newGoal = new Vector2(Random.Range(0, (int)dimension.x), Random.Range(0, (int)dimension.y));
            bool valid = true;
            foreach (Vector2 goal in goals)
            {
                if (goal == newGoal)
                {
                    valid = false;
                }
            }
            if (valid) goals.Add(newGoal);
            else continue;
            break;
        }
    }
}
