using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using HaveYouHeard;
public class User_Statistic_Instance : MonoBehaviour
{
    public Image pic;
    public Image pic_overlay;
    public TextMeshProUGUI username;
    public Image fill_bar;
    public Image non_character_bg;
    public int rounds_won;
    public Match_User current_user;
    int actual_points = 0;
    public GameObject character_obj;
    public GameObject non_character_obj;
    public void Populate(Match_User temp){
        current_user = temp;
        foreach (var item in Match_Manager.instance.users)
        {
            if (item.id == current_user.id_user){
                username.text = item.username;
            }
        }
        pic.sprite = Match_Manager.instance.current_character.pic;
        pic_overlay.sprite = Match_Manager.instance.current_character.pic_overlay;
        Color color;
        if (ColorUtility.TryParseHtmlString("#" + temp.color, out color)){ 
            pic_overlay.color = color;
            fill_bar.color = color;
            non_character_bg.color = color;
        }
    }
    public IEnumerator Update_Routine(){
        int new_points = 0;
        foreach (var item in Match_Manager.instance.match_users){
            if (item.id_user == current_user.id_user){
                new_points = item.points;
                break;
            }
        }
        float timer = 0;
        while(timer < 1.0f){
            float lerp = Mathf.Lerp((actual_points / 3.0f), (new_points / 3.0f), timer);
            timer += Time.deltaTime * 1.0f;
            fill_bar.fillAmount = lerp;
            yield return null;
        }
    }
}
