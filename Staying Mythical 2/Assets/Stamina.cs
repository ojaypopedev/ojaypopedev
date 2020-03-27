using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamina : MonoBehaviour
{

    playerController player;
    UI_Meter meter;
    void Start()
    {
        meter = GetComponent<UI_Meter>();
        player = StayingMythical.player;
    }

    // Update is called once per frame
    void Update()
    {
        meter.fillValue = player.StaminaPercentage;
    }
}
