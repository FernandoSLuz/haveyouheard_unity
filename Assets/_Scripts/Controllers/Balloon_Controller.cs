using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Balloon_Controller : MonoBehaviour
{
    public Image overlay;
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;

    public void Handle_Balloon(bool activate, string new_description = "", string new_title = "", string color = ""){ 
        if(activate){
            title.text = new_title;
            description.text = new_description;
            Color my_color;
            if (ColorUtility.TryParseHtmlString("#" + color, out my_color))
            { overlay.color = my_color; }
            this.gameObject.SetActive(activate);
        }
        else{
            StartCoroutine(Deactivate_Routine());
        }
    }
    public IEnumerator Deactivate_Routine(){
        GetComponent<Animator>().Play("out_fade");
        yield return new WaitForSeconds(0.5f);
        this.gameObject.SetActive(false);
    }
}
