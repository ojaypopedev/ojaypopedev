using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;
using StayingMythical.Environment;
using StayingMythical.Reference;
public class Interractable : MonoBehaviour
{

    [SerializeField] public Mesh[] Models;
    Environment.Obstacles type;
    public Environment.Obstacles Type { get { return type; } }
    playerController player;
    Outline outline;
    float interractionTime;
    public float InterractionTime { get { return interractionTime; } }
    public InventoryObject InventoryObject;

    public void setupInterractable(Environment.Obstacles type)
    {
        outline = GetComponent<Outline>();
        this.type = type;
        outline.enabled = false;
        switch (type)
        {
            case Environment.Obstacles.Rock:
                interractionTime = 3;
                break;
            case Environment.Obstacles.Tree:
                interractionTime = 5;
                break;
            case Environment.Obstacles.Base:
                interractionTime = 100;
                break;
            case Environment.Obstacles.Explorer:
                interractionTime = 100;
                break;
            case Environment.Obstacles.Ground:
                interractionTime = 1;
                break;
            case Environment.Obstacles.Yeti:
                interractionTime = 100;
                break;
            default:
                interractionTime = 1;
                break;
        }
        
        player = GameObjects.player;

        InventoryObject = getInventoryObject(type);

    }
    public void chooseRandomModel()
    {
        if (Models.Length != 0)
        {
            if(GetComponent<MeshFilter>())
            {
                int RandomModelIndex = Mathf.RoundToInt(Random.Range(0, Models.Length));
                GetComponent<MeshFilter>().mesh = Models[RandomModelIndex];
            }
          
        }
    }

    public void OutlineObject(bool active)
    {
        outline.enabled = active;
    }

    public virtual void Process()
    {
        Process(true);
               
    }

    public virtual void Process(bool Destroy)
    {
        Process(Destroy, true);
    }


    public virtual void Process(bool Destroy, bool Player)
    {
        if(player)
        {
           
           player.SetInventory(InventoryObject);
        }
     
        if (Destroy)
        {
            player.DestroyAndRemoveFromCollisions(gameObject);
        }

        Debug.Log(type.ToString() + "has been processed.");
    }


    public InventoryObject getInventoryObject(Environment.Obstacles type)
    {
        switch (type)
        {
            case Environment.Obstacles.Rock:
                return new InventoryObject(InventoryObject.InventoryObjectType.RockPiece);
               
            case Environment.Obstacles.Tree:
                return new InventoryObject(InventoryObject.InventoryObjectType.Logs);
              
            case Environment.Obstacles.Base:
                return null;
                
            case Environment.Obstacles.Explorer:
                return null;

            case Environment.Obstacles.Ground:
                return new InventoryObject(InventoryObject.InventoryObjectType.Snow);

            default:
                return null;
              
        }
    }

}
