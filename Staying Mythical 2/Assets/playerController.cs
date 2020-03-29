using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StayingMythical.Environment;
using StayingMythical.Reference;
public class playerController : MonoBehaviour
{

    float lookamount = 0;
    [SerializeField] Transform[] PosePoints;
    float[] lookMax = { -30, 80 };
    Transform head;
    public Transform Head { get { return head; } }
    Rigidbody rb;
    readonly float[] speeds = { 2, 5, 10 };
    readonly float[] FOVS = { 55, 60, 90 };
    float crouchSpeed = 4f;
    List<Collision>collisions = new List<Collision>();

    bool isGrounded = false;
    float ignoreCollisionFrames = 0.5f;
    float ifnoreCollisionTotal;

     float stamina = 4;
   [SerializeField] float staminaMax = 5;
    bool staminaUsed = false;
    public bool StaminaRecharing { get { return staminaUsed; } }

    float interractionTime = 0;
    float interractionTimeTotal = 0;
    public float InterractionPercentage { get { return interractionTime / interractionTimeTotal; } }

    [SerializeField] float interractionDistance = 3f;
    public float StaminaPercentage { get { return stamina / staminaMax; } }

    private Interractable currentInterractableInView;
    private ExplorerController currentExplorerInView;
    public Interractable CurrentInteractable { get { return currentInterractableInView; } }
    public Interractable CurrentExplorer { get { return currentInterractableInView; } }
    public enum MovementType { Crouch, Walk, Run};
    MovementType moveState = MovementType.Walk;
    public MovementType getMoveState() { return moveState; }

    private InventoryObject inventoryObject;
    public void SetInventory(InventoryObject inventoryObject) { this.inventoryObject = inventoryObject; }
    public InventoryObject GetInventory() { return inventoryObject; }

    public LayerMask raycastMask;
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
      
        Move();
        Interract();
        UseItems();
    }

    private void FixedUpdate()
    {
        Collide();
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
        if (stamina <= 0)
        {
            staminaUsed = true;
        }


        if (ignoreCollisionFrames > 0)
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
                if(!StaminaRecharing)
                {
                    movement += Vector3.up * 10;
                    isGrounded = false;
                    stamina -= 2;
                }
                else
                {
                    FindObjectOfType<UI_Cursor>().ShowBan(0.5f);
                }
               
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
                stamina += Time.deltaTime;
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
               
            }
            else
            {
                if (staminaUsed)
                {
                    stamina += Time.deltaTime / 1.5f;
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
            if (col != null)
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

    void Interract()
    {

        bool EDown = Input.GetKey(KeyCode.E);

        Ray ray = Camera.main.ScreenPointToRay(Camera.main.pixelRect.size / 2);
        RaycastHit hit;

        Physics.Raycast(ray, out hit,interractionDistance,raycastMask);
        if (hit.collider)
        {
            if (hit.collider.gameObject.GetComponent<Interractable>())
            {

                if (hit.collider.gameObject.GetComponent<Interractable>() is InterractableGround)
                {
                    if (Vector3.Distance(hit.point, head.position) < 2f)
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


        Physics.Raycast(ray, out hit, 20, raycastMask, QueryTriggerInteraction.Collide);
        if (hit.collider)
        {
            if (hit.collider.gameObject.GetComponent<ExplorerController>())
            {
                if (currentExplorerInView != null)
                {
                    currentExplorerInView.OutlineObject(false);
                    currentExplorerInView = null;
                }

                currentExplorerInView = hit.collider.gameObject.GetComponent<ExplorerController>();
                currentExplorerInView.OutlineObject(true);
            }
            else
            {
                if (currentExplorerInView != null)
                {
                    currentExplorerInView.OutlineObject(false);
                    currentExplorerInView = null;
                }
            }
        }
        else
        {
            if (currentExplorerInView != null)
            {
                currentExplorerInView.OutlineObject(false);
                currentExplorerInView = null;
            }
        }


        if (currentInterractableInView)
        {
            interractionTimeTotal = currentInterractableInView.InterractionTime;

            if(EDown  && !StaminaRecharing)
            {
                interractionTime+= Time.deltaTime;
              
                if(interractionTime >= interractionTimeTotal)
                {
                    currentInterractableInView.Process();

                    switch (currentInterractableInView.Type)
                    {
                        case Environment.Obstacles.Rock:
                            stamina -= 4;
                            break;
                        case Environment.Obstacles.Tree:
                            stamina -= 3;
                            break;
                        case Environment.Obstacles.Base:
                            stamina = 0;
                            break;
                        case Environment.Obstacles.Explorer:
                            stamina = 0;
                            break;
                        case Environment.Obstacles.Ground:
                            stamina -= 1;
                            break;
                        case Environment.Obstacles.Yeti:
                            stamina = 0;
                            break;
                        default:
                            break;
                    }
                   
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

        if(currentExplorerInView)
        {
            //do stuff here
        }


    }
 
    void UseItems()
    {

        if (inventoryObject != null)
        {
            if (inventoryObject.Type == InventoryObject.InventoryObjectType.Logs)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                   
                    if(!StaminaRecharing)
                    {
                        GameObject usedObject = inventoryObject.Create();
                        usedObject.GetComponent<Logs>().SetLogUse(InventoryObject.LogUse.Fire);
                        inventoryObject = null;
                        stamina -= 2;

                    }
                    else
                    {
                        FindObjectOfType<UI_Cursor>().ShowBan(0.5f);
                    }
                   
                }

                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    if(!StaminaRecharing)
                    {
                        GameObject usedObject = inventoryObject.Create();
                        usedObject.GetComponent<Logs>().SetLogUse(InventoryObject.LogUse.Trap);
                        inventoryObject = null;
                        stamina -= 2;
                    }
                    else
                    {
                        FindObjectOfType<UI_Cursor>().ShowBan(0.5f);
                    }
                   
                }

            }

            if (Input.GetMouseButtonDown(0))
            {

                if(!StaminaRecharing)
                {
                    if (inventoryObject.Type == InventoryObject.InventoryObjectType.Snow || inventoryObject.Type == InventoryObject.InventoryObjectType.RockPiece)
                    {
                        GameObject usedObject = inventoryObject.Create();
                        usedObject.GetComponent<Rigidbody>().AddForce(head.forward * 2000 + Vector3.up * 200);
                        usedObject.GetComponent<Rigidbody>().AddTorque(Random.onUnitSphere);

                        if(inventoryObject.Type == InventoryObject.InventoryObjectType.Snow)
                        {
                            stamina -= 1;
                        }
                        else
                        {
                            stamina -= 2;
                        }
                    }

                    inventoryObject = null;
                }
                else
                {
                    FindObjectOfType<UI_Cursor>().ShowBan(0.5f);
                }
               

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

        for (int i = 0; i < collisions.Count; i++)
        {
            if(collision.gameObject == collisions[i].gameObject)
            {
                collisions.Remove(collisions[i]);
            }
        }    
        
    }
    public void DestroyAndRemoveFromCollisions(GameObject toRemove)
    {

        int indexFound = -1;
        for (int i = 0; i < collisions.Count; i++)
        {
            if(collisions[i].gameObject == toRemove)
            {
                indexFound = i;
                break;
            }

        }

        if(indexFound != -1)
        {
            collisions.RemoveAt(indexFound);
            Debug.Log("Removed " + toRemove.name);
        }


        Debug.LogWarning(toRemove.name + " not found in collisions, destroying anyway.");
        Destroy(toRemove);
    }
}



public class InventoryObject
{
    public enum InventoryObjectType { Snow, RockPiece, Logs, Trap};
    public enum LogUse { Fire, Trap};

    private InventoryObjectType type;



    public InventoryObject(InventoryObjectType type)
    {
        this.type = type;
    }

    public InventoryObjectType Type

    {
        get { return type; }
    }

    public override string ToString()
    {
        return type.ToString();
    }

    public GameObject Create()
    {
        GameObject spawnedObject = null;

        switch (type)
        {
            
            case InventoryObjectType.Snow:
            {
                
                spawnedObject = Object.Instantiate(GameResources.SnowBall, GameObjects.player.Head.position + GameObjects.player.Head.transform.forward, Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)), null);
                break;
         
            }


            case InventoryObjectType.RockPiece:
            {
                spawnedObject = Object.Instantiate(GameResources.RockPiece, GameObjects.player.Head.position + GameObjects.player.Head.transform.forward, Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)), null);
                break;
            }

            case InventoryObjectType.Logs:
            {

                spawnedObject = Object.Instantiate(GameResources.Logs, GameObjects.player.transform.position + GameObjects.player.transform.forward*2, Quaternion.identity, null);   
                break;
            }


            default:
                Debug.LogWarning(type.ToString() + " does not have a Use() assigned.");
                break;
        }

        return spawnedObject;
    }
    
}

