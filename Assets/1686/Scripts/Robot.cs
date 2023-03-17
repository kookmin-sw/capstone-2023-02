using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour
{
    public Planner planner;
    public Vector3 source;
    public Vector3 destination;
    public Queue<Motion> motionToPath = new Queue<Motion>();
    public float speed = 0.5f;
    public float angularSpeed = 1f;
    // For route priority
    public float requestTime;
    private Motion motion = Motion.none;
    private float motionTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = source * planner.scale;
        requestTime = Time.time;
        planner.requestPlan(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (motion.time <= motionTime)
        {
            if (motionToPath.Count == 0) return;
            motion = motionToPath.Dequeue();
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
        planner.requestPlan(this);
    }
}
