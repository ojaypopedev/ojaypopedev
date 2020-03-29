using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StayingMythical.Environment;
using StayingMythical.Reference;
using UnityEngine.AI;
public class InterractableTree : Interractable
{

    bool fallen = false;
    public bool isFallen { get { return fallen; } }
    // Start is called before the first frame update
    void Start()
    {
        setupInterractable(Environment.Obstacles.Tree);
        chooseRandomModel();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Process()
    {
        if(!fallen)
        {
            GetComponent<Rigidbody>().isKinematic = false;
            Instantiate(GameResources.Stump, transform.position, Quaternion.identity, null);
            GameObject.Destroy(GetComponent<NavMeshObstacle>());
            fallen = true;
        }
        else
        {        
               GameObjects.player.SetInventory(InventoryObject);          
               GameObjects.player.DestroyAndRemoveFromCollisions(gameObject);      
        }


    }

    public override void Process(bool destory, bool player)
    {
        if (!fallen)
        {
            GetComponent<Rigidbody>().isKinematic = false;
            Instantiate(GameResources.Stump, transform.position, Quaternion.identity, null);
            fallen = true;
            GameObject.Destroy(GetComponent<NavMeshObstacle>());
        }      
    }

}
