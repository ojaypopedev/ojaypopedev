using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Pose : MonoBehaviour
{
    playerController controller;
    playerController.MovementType currentState;
    float timer = 1;
    float alpha =1;
    Image image;
    [SerializeField] Sprite[] sprites;
    Color col;

    void Start()
    {
        controller = StayingMythical.player;
        image = GetComponent<Image>();
        col = image.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.StaminaRecharing)
        {
            image.sprite = sprites[3];
        }

        if (currentState != controller.getMoveState())
        {
            currentState = controller.getMoveState();

            if(controller.StaminaRecharing)
            {
                image.sprite = sprites[3];
            }
            else
            {
                image.sprite = sprites[(int)currentState];
            }
         
            timer = 2;
        }

        //if(timer >0)
        //{
        //    if(alpha < 1)
        //    {
        //        alpha += Time.deltaTime;
        //    }

        //}
        //else
        //{
        //    if(alpha > 0)
        //    {
        //        alpha -= Time.deltaTime;
        //    }
        //}

        timer -= Time.deltaTime;

        col.a = alpha;
        image.color = col;

    }
}
