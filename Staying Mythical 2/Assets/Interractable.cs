using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;
using Mythical;
public class Interractable : MonoBehaviour
{

    Environment.Obsctacles type;
    playerController player;
    Outline outline;
    float interractionTime;
    public float InterractionTime { get { return interractionTime; } }
    public InventoryObject InventoryObject;

    public void setupInterractable(Environment.Obsctacles type)
    {
        outline = GetComponent<Outline>();
        this.type = type;
        outline.enabled = false;
        interractionTime = 3;
        player = StayingMythical.player;

        InventoryObject = getInventoryObject(type);

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
        player.SetInventory(InventoryObject);

        if(Destroy)
        {
            player.DestroyAndRemoveFromCollisions(gameObject);
        }
       
        Debug.Log(type.ToString() + "has been processed.");
    }

    public InventoryObject getInventoryObject(Environment.Obsctacles type)
    {
        switch (type)
        {
            case Environment.Obsctacles.Rock:
                return new InventoryObject(InventoryObject.InventoryObjectTypes.RockPiece);
               
            case Environment.Obsctacles.Tree:
                return new InventoryObject(InventoryObject.InventoryObjectTypes.Logs);
              
            case Environment.Obsctacles.Base:
                return null;
                
            case Environment.Obsctacles.Explorer:
                return null;

            case Environment.Obsctacles.Ground:
                return new InventoryObject(InventoryObject.InventoryObjectTypes.SnowBall);

            default:
                return null;
              
        }
    }

}
