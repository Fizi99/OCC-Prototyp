using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcBehaviourManager : MonoBehaviour
{

  


    public Task currentTask = new Task("wait", 1f, "neutral_idle");
    private NpcAnimationManager animManager;
    private Transform player;

    private Rigidbody rigid;
    private Vector3 targetLocation;
    private Vector3 direction;

    private List<Task> taskList = new List<Task>();
    private int taskIndex = 0;

    private void Start()
    {
        animManager = transform.GetComponent<NpcAnimationManager>();
        player = GameObject.Find("Player").transform;

        rigid = transform.GetComponent<Rigidbody>();

        AddToTaskList(new Task("wait", 1f, "neutral_idle"));
        AddToTaskList(new Task("wander", 3f, "neutral_walk"));

        currentTask = taskList[0];

    }

    private void Update()
    {

        if (currentTask.PassTime())
        {
            HandleTasks();
        }
        else
        {
            NextTask();
        }
        
    }

    public void StartTaskList()
    {
        taskIndex = 0;
        currentTask = taskList[0];
        animManager.ChangeAnimation(taskList[0].animation);
    }

    public void StartTask(Task task)
    {
        currentTask = task;
        animManager.ChangeAnimation(task.animation);

    }

    public void NextTask()
    {
        currentTask.Reset();
        taskIndex++;
        if (taskIndex >= taskList.Count)
        {
            taskIndex = 0;
        }

        StartTask(taskList[taskIndex]);
    }

    private void HandleTasks()
    {
        if (currentTask.name == "wait")
        {
            Wait();
        }else if (currentTask.name == "wander")
        {
            Wander();
        }else if (currentTask.name == "flee")
        {
            Flee();
        }
        else if (currentTask.name == "waitFlee")
        {
            WaitFlee();
        }
        else if (currentTask.name == "follow")
        {
            Follow();
        }
        else if (currentTask.name == "waitFollow")
        {
            WaitFollow();
        }
        else if (currentTask.name == "followNpc")
        {
            FollowNpc();
        }
        else if (currentTask.name == "waitFollowNpc")
        {
            WaitFollowNpc();
        }
        else if (currentTask.name == "fleeNpc")
        {
            FleeNpc();
        }
        else if (currentTask.name == "waitFleeNpc")
        {
            WaitFleeNpc();
        }
    }

    public void AddToTaskList(Task task)
    {
        taskList.Add(task);
    }

    public void ResetTaskList()
    {
        taskList.Clear();
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////TASKHANDLING////////////////////////////////////////////////////
    private void Wait()
    {
        if (!currentTask.taskStarted)
        {
            currentTask.taskStarted = true;
            currentTask.passedTime = 0;
        }
        Move(0);
    }

    private void Wander()
    {
        if (!currentTask.taskStarted)
        {
            currentTask.taskStarted = true;
            currentTask.passedTime = 0;
            targetLocation = PickRandTarget(new Vector3(-20, 0, -20), new Vector3(20, 0, 20));
        }

        if((targetLocation - rigid.transform.position).magnitude < 0.1f)
        {
            NextTask();
        }
        else
        {
            Move(3);
        }
        
    }

    private void Flee()
    {
        if (!currentTask.taskStarted)
        {
            currentTask.taskStarted = true;
            currentTask.passedTime = 0;
        }

        targetLocation = new Vector3(player.position.x, 0, player.position.z) + ((new Vector3(transform.position.x, 0, transform.position.z) - new Vector3(player.position.x, 0, player.position.z)).normalized * 10f);

        if ((targetLocation - rigid.transform.position).magnitude < 0.1f || (player.position - rigid.transform.position).magnitude > 9f)
        {
            NextTask();
        }
        else
        {
            Move(5);
        }
    }

    private void WaitFlee()
    {
        if (!currentTask.taskStarted)
        {
            currentTask.taskStarted = true;
            currentTask.passedTime = 0;
        }

        targetLocation = new Vector3(player.position.x, 0, player.position.z);

        if ((player.position - rigid.transform.position).magnitude < 9f)
        {
            NextTask();
        }
        else
        {
            Move(0);
        }
    }

    private void Follow()
    {
        if (!currentTask.taskStarted)
        {
            currentTask.taskStarted = true;
            currentTask.passedTime = 0;
        }

        targetLocation = new Vector3(player.position.x, 0, player.position.z);

        if ((targetLocation - rigid.transform.position).magnitude < 2f)
        {
            NextTask();
        }
        else
        {
            Move(5);
        }
    }

    private void WaitFollow()
    {
        if (!currentTask.taskStarted)
        {
            currentTask.taskStarted = true;
            currentTask.passedTime = 0;
        }

        targetLocation = new Vector3(player.position.x, 0, player.position.z);

        if ((player.position - rigid.transform.position).magnitude > 2.5f)
        {
            NextTask();
        }
        else
        {
            Move(0);
        }
    }

    private void FollowNpc()
    {
        if (!currentTask.taskStarted)
        {
            currentTask.taskStarted = true;
            currentTask.passedTime = 0;
        }

        targetLocation = new Vector3(currentTask.targetNpc.position.x, 0, currentTask.targetNpc.position.z);

        if ((targetLocation - rigid.transform.position).magnitude < 4.5f)
        {
            NextTask();
        }
        else
        {
            Move(5);
        }
    }

    private void WaitFollowNpc()
    {
        if (!currentTask.taskStarted)
        {
            currentTask.taskStarted = true;
            currentTask.passedTime = 0;
        }

        targetLocation = new Vector3(currentTask.targetNpc.position.x, 0, currentTask.targetNpc.position.z);

        if ((targetLocation - rigid.transform.position).magnitude > 5f)
        {
            NextTask();
        }
        else
        {
            Move(0);
        }
    }

    private void FleeNpc()
    {
        if (!currentTask.taskStarted)
        {
            currentTask.taskStarted = true;
            currentTask.passedTime = 0;
        }
        targetLocation = new Vector3(currentTask.targetNpc.position.x, 0, currentTask.targetNpc.position.z) + ((new Vector3(transform.position.x, 0, transform.position.z) - new Vector3(currentTask.targetNpc.position.x, 0, currentTask.targetNpc.position.z).normalized) * 10f);

        if ((targetLocation - rigid.transform.position).magnitude < 0.1f || (new Vector3(currentTask.targetNpc.position.x, 0, currentTask.targetNpc.position.z) - rigid.transform.position).magnitude > 9f)
        {
            NextTask();
        }
        else
        {
            Move(5);
        }
    }

    private void WaitFleeNpc()
    {
        if (!currentTask.taskStarted)
        {
            currentTask.taskStarted = true;
            currentTask.passedTime = 0;
        }

        targetLocation = new Vector3(currentTask.targetNpc.position.x, 0, currentTask.targetNpc.position.z);

        if ((new Vector3(currentTask.targetNpc.position.x, 0, currentTask.targetNpc.position.z) - rigid.transform.position).magnitude < 9f)
        {
            NextTask();
        }
        else
        {
            Move(0);
        }
    }





    private void Move(float speed)
    {
        direction = (targetLocation - rigid.transform.position).normalized;
        rigid.velocity = direction * speed;
        transform.LookAt(targetLocation);
    }

    private Vector3 PickRandTarget(Vector3 min, Vector3 max)
    {
        return new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));

    }

}
