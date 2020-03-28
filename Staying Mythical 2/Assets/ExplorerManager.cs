using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplorerManager : MonoBehaviour
{

    [SerializeField] ExplorerController[] Explorers;
    [SerializeField] List<Transform> GoToTargets = new List<Transform>();
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

            item.addTask(new GoTo(randomGoTo(), item));
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
