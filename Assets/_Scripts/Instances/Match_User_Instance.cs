using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using HaveYouHeard;
public class Match_User_Instance : MonoBehaviour
{
	public GameObject confirm_overlay;
	public TextMeshProUGUI name_text;
	public Match_User current_match_user;
	public Color neutralColor;
	public Color user_color;
	public void Check_Status(){ 
		if(current_match_user.ready){
			confirm_overlay.SetActive(true);
			if (ColorUtility.TryParseHtmlString("#" + current_match_user.color, out user_color))
			{ confirm_overlay.GetComponent<Image>().color = user_color; }
			name_text.color = neutralColor;
		}else{
			confirm_overlay.SetActive(false);
			if (ColorUtility.TryParseHtmlString("#"+current_match_user.color, out user_color))
			{ name_text.color = user_color; }
			confirm_overlay.GetComponent<Image>().color = Color.white;
		}
	}
}
