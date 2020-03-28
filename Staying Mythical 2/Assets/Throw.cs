using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw : MonoBehaviour
{

    Rigidbody rb;
    Vector3 throwDir;
    float throwForce = 300;
    public void SetThrowDirection(Vector3 direction)
    {
        throwDir = direction;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
       
    }

    void Start()
    {
        rb.AddForce(throwDir * throwForce);
    }

   

    
}
