using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] float time;

    void Start()
    {
        Invoke(_Kill, time);
    }

   string _Kill = "Kill";
   void Kill()
    {
        Destroy(gameObject);
    }
}
