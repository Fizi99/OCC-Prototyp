using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task
{
    /*
    public string name;
    public float prio;
    public bool taskConcluded = false;
    public bool taskStarted = false;
    public string animation;

    //variables for wander/flee task
    public float speed;
    public Rigidbody rigid;
    public Vector3 destination;

    //variables for flee/follow task
    public Transform target;
    public float range;

    //variables for wait task
    public float waitTime;
    public float passedWaitTime;

    public Task(string name, float prio, string animation){
        this.name = name;
        this.prio = prio;
        this.animation = animation;
    }

    public Task(string name, float prio, string animation, float range){
        this.name = name;
        this.prio = prio;
        this.animation = animation;
        this.range = range;
    }
    */
    

    public string name;
    public bool taskConcluded = false;
    public bool taskStarted = false;
    public string animation;

    public float duration;
    public float passedTime;

    public Transform targetNpc;

    public Task(string name, float duration, string animation)
    {
        this.name = name;
        this.animation = animation;
        this.duration = duration;
    }

    public Task(string name, float duration, string animation, Transform targetNpc)
    {
        this.name = name;
        this.animation = animation;
        this.duration = duration;
        this.targetNpc = targetNpc;
    }

    public bool PassTime()
    {
        if(passedTime >= duration)
        {
            return false;
        }

        passedTime += Time.deltaTime;
        return true;
    }

    public void Reset()
    {
        passedTime = 0;
        taskStarted = false;
    }
}
