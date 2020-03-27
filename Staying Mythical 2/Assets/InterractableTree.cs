using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mythical;
public class InterractableTree : Interractable
{

    bool fallen = false;
    // Start is called before the first frame update
    void Start()
    {
        setupInterractable(Environment.Obsctacles.Tree);
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
            Instantiate(GameObjectReference.Stump, transform.position, Quaternion.identity, null);
            fallen = true;
        }
        else
        {
             base.Process();
        }


    }

}
