using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterractableGround : Interractable
{
    // Start is called before the first frame update
    void Start()
    {
        setupInterractable(StayingMythical.Environment.Environment.Obstacles.Ground);
    }


    public override void Process()
    {

        base.Process(false);
    }
}
