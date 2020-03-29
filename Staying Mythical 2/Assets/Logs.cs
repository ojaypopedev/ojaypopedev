using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StayingMythical.Reference;
public class Logs : MonoBehaviour
{

    public InventoryObject.LogUse use;
    public void Craft()
    {
        switch (use)
        {
            case InventoryObject.LogUse.Fire:
                Instantiate(GameResources.Fire, transform.position, Quaternion.identity, null);               
                break;
            case InventoryObject.LogUse.Trap:
                Instantiate(GameResources.Trap, transform.position, Quaternion.Euler(-90,0,0), null);
                break;
            default:
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            Craft();
            Destroy(gameObject);
        }
    }

    public void SetLogUse(InventoryObject.LogUse use)
    {
        Debug.Log("Log Set Up");
        this.use = use;
    }
}
