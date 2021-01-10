using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HaveYouHeard;
using TMPro;

public class Character_Info_Controller : MonoBehaviour
{
    public float time_to_change;
    public Image character_image;
    public Image character_overlay;
    public Image fill_bar;
    public TextMeshProUGUI character_name;
    public TextMeshProUGUI character_description;
    public TextMeshProUGUI character_specs;
    public IEnumerator Start()
    {
        character_image.sprite = Match_Manager.instance.current_character.pic;
        character_overlay.sprite = Match_Manager.instance.current_character.pic_overlay;
        character_name.text = Match_Manager.instance.current_character.name;
        character_description.text = Match_Manager.instance.current_character.description;
        Color my_color;
        foreach(var match_user in Match_Manager.instance.match_users){ 
            if(User_Manager.instance.current_user.id == match_user.id_user){
                if (ColorUtility.TryParseHtmlString("#" + match_user.color, out my_color))
                { character_overlay.color = my_color; }
            }
        }
        float timer = 0.0f;
        while(timer <1.0f){
            fill_bar.fillAmount = timer;
            timer += Time.deltaTime / time_to_change;
            yield return null;
        }
        Game_Controller.instance.StartCoroutine(Game_Controller.instance.Round_Routine_1());
    }
}
