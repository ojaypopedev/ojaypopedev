using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mythical;

public class InterractableRock : Interractable
{
    // Start is called before the first frame update
    void Start()
    {
        setupInterractable(Environment.Obstacles.Rock);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Process()
    {
        base.Process(false);
    }
}
