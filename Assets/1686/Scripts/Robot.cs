using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SIPP;

public class Robot : MonoBehaviour
{
    public Planner planner;
    public Vector3 source;
    public Vector3 destination;
    public Queue<State> stateToPath = new Queue<State>();
    public float speed = 0.5f;
    public float angularSpeed = 1f;
    // For route priority
    public float requestTime;
    private Motion motion = Motion.none;
    private float motionTime = 0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (motion.time <= motionTime)
        {
            if (stateToPath.Count == 0) return;
            if (stateToPath.Peek().time > Time.time) return;
            motion = stateToPath.Dequeue().motion;
            motionTime = 0;
        }
        Vector3 velocity = motion.deltaPosition * speed;
        transform.position += velocity * Time.deltaTime;
        float rotateDegree = motion.deltaRotation * angularSpeed;
        transform.Rotate(Vector3.up, rotateDegree * Time.deltaTime, Space.Self);
        motionTime += Time.deltaTime;
    }

    public void setRoute(Vector3 route)
    {
        source = route;
        // planner.requestPlan(this);
    }

    public void Request(Vector2 start, Vector2 goal)
    {
        source.x = start.x;
        source.z = start.y;
        destination.x = goal.x;
        destination.z = goal.y;
        requestTime = Time.time;
        planner.RequestPlan(this);
    }
}
