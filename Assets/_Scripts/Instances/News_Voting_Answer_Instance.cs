using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HaveYouHeard;
using TMPro;
public class News_Voting_Answer_Instance : MonoBehaviour
{
	public string fulfilled_news;
	public int id_user;
	public Button vote_btn;
	public TextMeshProUGUI fulfilled_news_text;

	public void populate_answer(string fulfillment_json){
		JsonUtility.FromJsonOverwrite(fulfillment_json, this);
		fulfilled_news_text.text = fulfilled_news;
		if(fulfilled_news == ""){
			fulfilled_news_text.text = "<i>não respondeu em tempo</i>";
		}
		vote_btn.onClick.AddListener(() => {
			Answer_Selected();
		});
	}
	public void Answer_Selected(){
		Game_Controller.instance.StopAllCoroutines();
		Game_Controller.instance.Process_Votes(id_user);
	}
}
