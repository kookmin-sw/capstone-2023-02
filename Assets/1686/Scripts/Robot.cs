using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour
{
    public Planner planner;
    public Motion[] availableMotions = {
        Motion.forward,
        Motion.back,
        Motion.left,
        Motion.right,
    };
    public Vector3 source;
    public Vector3 destination;
    public Queue<Motion> motionToPath = new Queue<Motion>();
    public float speed = 0.5f;
    // For route priority
    public float requestTime;
    private Motion motion = Motion.none;

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
        if (motionToPath.Count == 0) return;
        if (motion.time <= 0f) motion = motionToPath.Dequeue();
        Vector3 velocity = motion.deltaPosition * speed;
        transform.position += velocity * Time.deltaTime;
        motion.time -= Time.deltaTime;
    }

    public void setRoute(Vector3 route)
    {
        source = route;
        planner.requestPlan(this);
    }
}
