using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StayingMythical.Environment;

public class InterractableRock : Interractable
{
    // Start is called before the first frame update
    void Start()
    {
        setupInterractable(Environment.Obstacles.Rock);
        chooseRandomModel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }  

     public override void Process(bool Destroy, bool player)
     {
       base.Process(Destroy, player);
     }

    public override void Process()
    {
        base.Process(false,true);
    }
}
