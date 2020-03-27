using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory : MonoBehaviour
{
    playerController player;
    [SerializeField] Text text;
    void Start()
    {
        player = StayingMythical.player;
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
