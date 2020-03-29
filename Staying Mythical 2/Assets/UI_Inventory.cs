using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using StayingMythical.Reference;

public class UI_Inventory : MonoBehaviour
{
    playerController player;
    [SerializeField] Text text;
    void Start()
    {
        player = GameObjects.player;
    }

    // Update is called once per frame
    void Update()
    {
        if(player.GetInventory() != null)
        {
            text.text = player.GetInventory().ToString();
        }
    }
}
