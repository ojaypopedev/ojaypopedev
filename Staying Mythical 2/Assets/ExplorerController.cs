using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mythical;
using UnityEngine.AI;

public class ExplorerController : MonoBehaviour
{

    [SerializeField] Task currentTask;
    [SerializeField] List<Task> taskList = new List<Task>();
    public Transform testTransform;

    NavMeshAgent explorer;
    public NavMeshAgent agent { get { return explorer; } }

    private void Awake()
    {
       explorer = GetComponent<NavMeshAgent>();

    }
    void Update()
    {       

       if(currentTask.Complete)
       {
           
            if (taskList.Count > 0)
            {
                currentTask = taskList[0];
                taskList.RemoveAt(0);
             

            }
            else
            {
                return;
            }
       
       }
       else
       {
            if(currentTask.location != null && currentTask.owner != null)
            {
                currentTask.UpdateTask();
            }
            else
            {
                currentTask.CompleteTask();
            }
           

       }

     
      

    }

    public void addTask(Task task)
    {
        taskList.Add(task);
    }
}



[System.Serializable]
public class Task
{
    private readonly float reachedDistance = 3;

    public Transform location;
    public ExplorerController owner;

    bool complete = false;
    public bool Complete { get { return complete; } }
    public void CompleteTask()
    {
        currentState = TaskState.Complete;
        complete = true;
    }

    public enum TaskState { Travel, Action, Complete, Special}
    
    [SerializeField] private TaskState currentState = TaskState.Travel;
    public TaskState State { get { return currentState; } }

    public virtual void UpdateTask()
    {

      
       if(owner.agent)
        {
            if (currentState == TaskState.Travel)
            {

                owner.agent.isStopped = false;
                owner.agent.SetDestination(location.position);

                if (Vector3.Distance(location.position, owner.transform.position) < reachedDistance)
                {
                    currentState = TaskState.Action;
                }

            }

            if (currentState == TaskState.Complete)
            {
                owner.agent.isStopped = true;

                complete = true;
            }

            if (currentState == TaskState.Action)
            {
                owner.agent.isStopped = true;
            }

        }

    }

}
[System.Serializable]
public class GoTo : Task
{  
    public GoTo(Transform Target, ExplorerController owner)
    {
        this.owner = owner;
        location = Target;
    }


    public override void UpdateTask()
    {
        if (State == TaskState.Action)
        {
            CompleteTask();

        }  

        base.UpdateTask();

    }
}
[System.Serializable]
public class Process: Task
{
    private Environment.Obstacles targetType;

    public Process(Environment.Obstacles Target, ExplorerController owner)
    {
        this.owner = owner;
        targetType = Target;
        FindObjectToProcess();
    }

    void FindObjectToProcess()
    {

        foreach (var item in Object.FindObjectsOfType<Interractable>())
        {
            if (item.Type == targetType)
            {
                location = item.transform;
                break;
            }
        }
    }

    public override void UpdateTask()
    {
        if (State == TaskState.Action)
        {

        }

        if (State == TaskState.Special)
        {

        }

        base.UpdateTask();

    }

}





