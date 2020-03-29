using UnityEngine;
using UnityEngine.UI;
using StayingMythical.Reference;
public class UI_Cursor : MonoBehaviour
{

    playerController player;
    Animator anim;
   [SerializeField] Image fillMeter;
    [SerializeField] CanvasGroup StaminaBan;
    float staminaBanTime;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        player = GameObjects.player;
       
    }

    // Update is called once per frame
    void Update()
    {    
        anim.SetBool("Highlight", player.CurrentInteractable != null);
        fillMeter.fillAmount = player.InterractionPercentage;

        staminaBanTime -= Time.deltaTime;

        StaminaBan.alpha = (player.CurrentInteractable && player.StaminaRecharing)||staminaBanTime > 0 ? 1 : 0;
    }

    public void ShowBan(float time)
    {
        staminaBanTime = time;
    }
}
