using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StayingMythical.ExplorerTask;
using StayingMythical.Environment;
using StayingMythical.Reference;

public class ExplorerManager : MonoBehaviour
{

    [SerializeField] ExplorerController[] Explorers;
    [SerializeField] List<Transform> GoToTargets = new List<Transform>();
    Transform ExplorerBase;

    public void addGoToTarget(Transform toAdd)
    {
        GoToTargets.Add(toAdd);
    }
    public Transform randomGoTo()
    {
        int choice = Mathf.RoundToInt(Random.Range(0, GoToTargets.Count-1));
        return GoToTargets[choice];
    }



    void Start()
    {
        Explorers = FindObjectsOfType<ExplorerController>();

        foreach (var item in Explorers)
        {

            //item.addTask(new GoTo(randomGoTo(), item));
            //item.addTask(new Wait(item, 3));
            //item.addTask(new GoToBase(item));
            //item.addTask(new GoTo(randomGoTo(), item));
            //item.addTask(new GoToBase(item));
            
            item.addTask(new Collect(closestRock(item), item));
           

            // item.addTask(new Collect(closestRock(item), item));
        }

    }

    private InterractableTree closestTree(ExplorerController explorer)
    {
        int closest = 0;

        for (int i = 0; i < GameObjects.Trees.Length; i++)
        {
            if(GameObjects.Trees[i].isFallen == false)
            {
                if (Vector3.Distance(GameObjects.Trees[i].transform.position, explorer.transform.position) < Vector3.Distance(GameObjects.Trees[closest].transform.position, explorer.transform.position))
                {
                    closest = i;
                }
            }
           
        }

        if(GameObjects.Trees[closest] != null)
        {
            return GameObjects.Trees[closest];
        }
        else
        {
            return null;
        }
    }
    private InterractableRock closestRock(ExplorerController explorer)
    {
        int closest = 0;

        for (int i = 0; i < GameObjects.Rocks.Length; i++)
        {
          
            
                if (Vector3.Distance(GameObjects.Rocks[i].transform.position, explorer.transform.position) < Vector3.Distance(GameObjects.Rocks[closest].transform.position, explorer.transform.position))
                {
                    closest = i;
                }
            

        }

        if (GameObjects.Trees[closest] != null)
        {
            return GameObjects.Rocks[closest];
        }
        else
        {
            return null;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

