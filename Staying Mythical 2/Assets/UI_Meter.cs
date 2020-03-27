using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UI_Meter : MonoBehaviour
{
    [SerializeField] bool background = true;
    [SerializeField] Color color;
    [SerializeField] Image backgroundColor;
    [SerializeField] Image meter;
    [SerializeField] Image icon;
    [SerializeField] public Sprite iconSprite;
    public enum FillDirection { LR, RL, DU, UD};
    [SerializeField] private FillDirection direction;
    [Range(0, 1)] public float fillValue;
    [HideInInspector] private Vector2 rectSize;

    private bool initialzed
    {
        get { return (backgroundColor && meter && icon); }

    }

    private void Awake()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if(initialzed)
        {
            rectSize = GetComponent<RectTransform>().sizeDelta;
            setColor(color);
            setShape(iconSprite);
            setPivot(direction);
            setMeter(fillValue);
        }

        

      
    }

    void setMeter(float value)
    {
        RectTransform rect = meter.GetComponent<RectTransform>();
        Vector2 size = Vector2.one;
        switch (direction)
        {
            case FillDirection.LR:
               size = new Vector2((value), 1);
                break;
            case FillDirection.RL:
                size = new Vector2((value), 1);
                break;
            case FillDirection.DU:
                size = new Vector2(1, (value));
                break;
            case FillDirection.UD:
                size = new Vector2(1, (value));
                break;
            default:
                break;
        }

        //print(size);
        rect.sizeDelta = size*rectSize;
    }

    void setPivot(FillDirection dir)
    {
        RectTransform rect = meter.GetComponent<RectTransform>();
        Vector2 anchorPoint = Vector2.zero;

        switch (dir)
        {
            case FillDirection.LR:
                anchorPoint = new Vector2(0f, 0.5f);
                break;
            case FillDirection.RL:
                anchorPoint = new Vector2(1f, 0.5f);
                break;
            case FillDirection.DU:
                anchorPoint = new Vector2(0.5f, 0f);
                break;
            case FillDirection.UD:
                anchorPoint = new Vector2(0.5f,1f);
                break;
        }

        rect.anchorMax = anchorPoint;
        rect.anchorMin = anchorPoint;
        rect.pivot = anchorPoint;
    }
    void setColor(Color col)
    {
        meter.color = col;
        Color bgCol = meter.color;// (col - Color.white / 3);
        bgCol.a = meter.color.a - 0.5f;

       // bgCol.a = 1;
        if(background)
        {
            backgroundColor.color = bgCol;
        }
        else
        {
            backgroundColor.color = Color.clear;
        }
       
       
        
    }
    void setShape(Sprite shape)
    {
        icon.sprite = iconSprite;
    }
    
     


   
}
