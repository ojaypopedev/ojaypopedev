using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StayingMythical;
using UnityEngine.AI;
using cakeslice;
using StayingMythical.ExplorerTask;
using StayingMythical.Reference;

public class ExplorerController : MonoBehaviour
{

    [SerializeField] Task currentTask;
    [SerializeField] List<Task> taskList = new List<Task>();
    //  public Transform testTransform;
    bool alive = true;


    NavMeshAgent explorer;
    Animator anim;
    public NavMeshAgent agent { get { return explorer; } }

    public GameObject FOV;
    public Outline outline;
    Outline FOVOutline;
        
    private void Awake()
    {
       
        FOVOutline = FOV.GetComponent<Outline>();
       anim = GetComponent<Animator>();
       explorer = GetComponent<NavMeshAgent>();
        anim.SetFloat("MoveSpeed", 0);
        
    }
    private void Start()
    {
        OutlineObject(false);
    }
    void Update()
    {

        
        if (alive)
        {
           
            if (!explorer.isStopped)
            {
                anim.SetFloat("MoveSpeed", explorer.speed);
            }
            else
            {
                anim.SetFloat("MoveSpeed", 0);
            }

            if (currentTask != null)
            {
                
                if (currentTask.Complete)
                {
                    if (taskList.Count > 0)
                    {
                        currentTask = taskList[0];
                        taskList.RemoveAt(0);
                       
                    }

                }
                else
                {
                    currentTask.UpdateTask();
                }
            }
            else
            {
                if (taskList.Count > 0)
                {
                    currentTask = taskList[0];
                    taskList.RemoveAt(0);

                }
            }

        }
        else
        {

        }
      

    }

    public void SetAnimation(string state)
    {
        anim.SetTrigger(state);
    }

    public void addTask(Task task)
    {
        taskList.Add(task);
    }

    public void KillExplorer()
    {
        transform.LookAt(GameObjects.player.transform);

        alive = false;
        Destroy(explorer); 
        anim.SetTrigger("Die");
        OutlineObject(false);
        Destroy(FOV);
    }

    private void OnTriggerEnter (Collider collision)
    {
       
        if(collision.gameObject.GetComponent<InventoryRock>())
        {
            Destroy(collision.gameObject);
            KillExplorer();
        }
    }
   
    public void OutlineObject(bool active)
    {
        
        if(FOVOutline)
        {
            FOVOutline.GetComponent<MeshRenderer>().enabled = active;
            FOVOutline.enabled = active;

        }
        outline.enabled = active;
    }
    
}






