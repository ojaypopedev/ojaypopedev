using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{

    [SerializeField] float lookamount = 0;
    float[] lookMax = { -30, 80 };
    Transform head;
    Rigidbody rb;
    float walkSpeed = 4;
    float runSpeed = 7;
    List<Collision>collisions = new List<Collision>();
    bool isGrounded = false;
  

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        head = GetComponentInChildren<Camera>().transform;
        rb = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update()
    {
        Look();
        Collide();
        Move();
       
       

    }

    void Look()
    {
        transform.Rotate(0, Input.GetAxis("Mouse X"), 0);


        lookamount -= Input.GetAxis("Mouse Y");

        if (lookamount > lookMax[1] || lookamount < lookMax[0])
        {
            lookamount -= Input.GetAxis("Mouse Y");
        }
        else
        {
            head.Rotate(-Input.GetAxis("Mouse Y"), 0, 0);
        }

    }
    void Move()
    {

        Vector3 movement = Vector3.zero;

        bool wDown = Input.GetKey(KeyCode.W);
        bool sDown = Input.GetKey(KeyCode.S);
        bool dDown = Input.GetKey(KeyCode.D);
        bool aDown = Input.GetKey(KeyCode.A);
        bool shiftDown = Input.GetKey(KeyCode.LeftShift);
        bool spaceDown = Input.GetKeyDown(KeyCode.Space);

        float speed = shiftDown ? runSpeed : walkSpeed;

        if(wDown)
        {
            movement += transform.forward * speed;
        }

        if (sDown)
        {
            movement -= transform.forward * speed;
        }

        if(dDown)
        {
            movement += transform.right * speed;
        }

        if(aDown)
        {
            movement -= transform.right * speed;
        }

        if(spaceDown)
        {
            if(isGrounded)
            {
                movement += Vector3.up * 10;
            }
            else
            {
                Debug.Log("Not Grounded");
            }
        }

        
        movement.y += rb.velocity.y;
       
        rb.velocity = movement;

    }

    void Collide()
    {
        foreach (var col in collisions)
        {
            if(col.gameObject.tag == "Ground")
            {
                isGrounded = true;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
      if(!collisions.Contains(collision))
        {
            collisions.Add(collision);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collisions.Contains(collision))
        {
            collisions.Remove(collision);
        }
    }
}
