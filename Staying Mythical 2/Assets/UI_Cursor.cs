using UnityEngine;
using UnityEngine.UI;

public class UI_Cursor : MonoBehaviour
{

    playerController player;
    Animator anim;
   [SerializeField] Image fillMeter;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        player = StayingMythical.player;
       
    }

    // Update is called once per frame
    void Update()
    {    
        anim.SetBool("Highlight", player.CurrentInteractable != null);
        fillMeter.fillAmount = player.InterractionPercentage;
       
    }
}
