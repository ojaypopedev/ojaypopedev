using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{

    float lookamount = 0;
    [SerializeField] Transform[] PosePoints;
    float[] lookMax = { -30, 80 };
    Transform head;
    Rigidbody rb;
    readonly float[] speeds = { 2, 5, 10 };
    readonly float[] FOVS = { 55, 60, 90 };
    float crouchSpeed = 4f;
    List<Collision>collisions = new List<Collision>();

    bool isGrounded = false;
    float ignoreCollisionFrames = 0.5f;
    float ifnoreCollisionTotal;

    float stamina = 2;
    float staminaMax = 2;
    bool staminaUsed = false;
    public bool StaminaRecharing { get { return staminaUsed; } }

    float interractionTime = 0;
    float interractionTimeTotal = 0;
    public float InterractionPercentage { get { return interractionTime / interractionTimeTotal; } }

    [SerializeField] float interractionDistance = 3f;
    public float StaminaPercentage { get { return stamina / staminaMax; } }

    private Interractable currentInterractableInView;
    public Interractable CurrentInteractable { get { return currentInterractableInView; } }
    public enum MovementType { Crouch, Walk, Run};
    MovementType moveState = MovementType.Walk;
    public MovementType getMoveState() { return moveState; }

    private InventoryObject inventory;
    public void SetInventory(InventoryObject inventoryObject) { inventory = inventoryObject; }
    public InventoryObject GetInventory() { return inventory; }

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
        Interract();
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

        if(ignoreCollisionFrames > 0)
        {
            ignoreCollisionFrames -= Time.deltaTime;
        }

        Vector3 movement = Vector3.zero;

        bool wDown = Input.GetKey(KeyCode.W);
        bool sDown = Input.GetKey(KeyCode.S);
        bool dDown = Input.GetKey(KeyCode.D);
        bool aDown = Input.GetKey(KeyCode.A);
        bool shiftDown = Input.GetKey(KeyCode.LeftShift);
        bool spaceDown = Input.GetKeyDown(KeyCode.Space);
        bool controlDown = Input.GetKey(KeyCode.LeftControl);

        if(controlDown)
        {
            moveState = MovementType.Crouch;
        }
        else if(shiftDown)
        {

            if(staminaUsed == false)
            {
                moveState = MovementType.Run;
            }
            else
            {
                moveState = MovementType.Walk;
            }
           
        }
        else
        {
            moveState = MovementType.Walk;
        }

        float speed = speeds[(int)moveState];

        //lateral movement
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


        //horizontal movement
        if(spaceDown)
        {
            if(isGrounded)
            {
                movement += Vector3.up * 10;
                isGrounded = false;

            }
            else
            {
                Debug.Log("Not Grounded");
            }
        }

        if(movement.magnitude < 0.1f)
        {

            if (staminaUsed)
            {
                stamina += Time.deltaTime / 2f;
                if (stamina >= staminaMax)
                {
                    stamina = staminaMax;
                    staminaUsed = false;
                }

            }
            else
            {
                stamina += Time.deltaTime;
                if (stamina >= staminaMax)
                {
                    stamina = staminaMax;

                }
            }

            if (moveState == MovementType.Run) { moveState = MovementType.Walk; }
           
        }
        else
        {
            
        
            

            if (moveState == MovementType.Run)
            {
                stamina -= Time.deltaTime;
                if(stamina <= 0)
                {
                    staminaUsed = true;
                }
            }
            else
            {
                if (staminaUsed)
                {
                    stamina += Time.deltaTime / 3f;
                    if (stamina >= staminaMax)
                    {
                        stamina = staminaMax;
                        staminaUsed = false;
                    }
                }
                else
                {
                    stamina += Time.deltaTime / 2f;
                    if (stamina >= staminaMax)
                    {
                        stamina = staminaMax;
                        staminaUsed = false;
                    }
                }
            }
        }

        head.transform.position = Vector3.MoveTowards(head.transform.position, PosePoints[(int)moveState].position, crouchSpeed*Time.deltaTime);
        head.GetComponent<Camera>().fieldOfView = Mathf.Lerp(head.GetComponent<Camera>().fieldOfView, FOVS[(int)moveState], 4 * Time.deltaTime);
        
        movement.y += rb.velocity.y;
       
        rb.velocity = movement;

    }
    void Collide()
    {
        foreach (var col in collisions)
        {
            if(col != null)
            {
                if(col.gameObject)
                {
                    if(col.gameObject.GetComponent<MeshCollider>())
                    {
                        if (col.gameObject.tag == "Ground")
                        {
                            if (ignoreCollisionFrames <= 0)
                            {
                                isGrounded = true;
                            }

                        }
                    }
                 
                }
               
            }
            
        }
    }

    void Interract()
    {

        bool EDown = Input.GetKey(KeyCode.E);

        Ray ray = Camera.main.ScreenPointToRay(Camera.main.pixelRect.size / 2);
        RaycastHit hit;
        Physics.Raycast(ray, out hit,interractionDistance);

        if(hit.collider)
        {
            if(hit.collider.gameObject.GetComponent<Interractable>())
            {

                if (hit.collider.gameObject.GetComponent<Interractable>() is InterractableGround)
                {
                    if(Vector3.Distance(hit.point, head.position) < 2f)
                    {
                        if (currentInterractableInView != null)
                        {
                            currentInterractableInView.OutlineObject(false);
                            currentInterractableInView = null;
                        }

                        currentInterractableInView = hit.collider.gameObject.GetComponent<Interractable>();
                        currentInterractableInView.OutlineObject(true);
                    }
                    else
                    {
                        if (currentInterractableInView != null)
                        {
                            currentInterractableInView.OutlineObject(false);
                            currentInterractableInView = null;
                        }
                    }

                }
                else
                {
                    if (currentInterractableInView != null)
                    {
                        currentInterractableInView.OutlineObject(false);
                        currentInterractableInView = null;
                    }

                    currentInterractableInView = hit.collider.gameObject.GetComponent<Interractable>();
                    currentInterractableInView.OutlineObject(true);
                }
             
             
            }
            else
            {
                if (currentInterractableInView != null)
                {
                    currentInterractableInView.OutlineObject(false);
                    currentInterractableInView = null;
                }
            }

        }
        else
        {
            if (currentInterractableInView != null)
            {
                currentInterractableInView.OutlineObject(false);
                currentInterractableInView = null;
            }
        }



        if(currentInterractableInView)
        {
            interractionTimeTotal = currentInterractableInView.InterractionTime;

            if(EDown)
            {
                interractionTime+= Time.deltaTime;
                print(InterractionPercentage) ;
                if(interractionTime >= interractionTimeTotal)
                {
                    currentInterractableInView.Process();
                    interractionTime = 0;
                }
            }
            else
            {
                if(interractionTime >0)
                {
                    interractionTime -= 2 * Time.deltaTime;
                }
              
            }
        }
        else
        {
            interractionTime = 0;
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

    public void DestroyAndRemoveFromCollisions(GameObject toRemove)
    {
        foreach (var item in collisions)
        {
            if(item.gameObject == toRemove)
            {
                collisions.Remove(item);
                Destroy(toRemove);
                return;
            }
        }

        Debug.LogWarning(toRemove.name + " not found in collisions, destroying anyway.");
        Destroy(toRemove);
    }
}



public class InventoryObject
{
    public enum InventoryObjectTypes { SnowBall, RockPiece, Logs, Trap};

    private InventoryObjectTypes type;

    public InventoryObject(InventoryObjectTypes type)
    {
        this.type = type;
    }

    public override string ToString()
    {
        return type.ToString();
    }

    public void Use()
    {
        switch (type)
        {
            case InventoryObjectTypes.SnowBall:
                break;
            case InventoryObjectTypes.RockPiece:
                break;
            case InventoryObjectTypes.Logs:
                break;
            case InventoryObjectTypes.Trap:
                break;
            default:
                break;
        }
    }
    
}

