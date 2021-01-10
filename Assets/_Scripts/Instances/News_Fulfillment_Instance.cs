using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using HaveYouHeard;

public class News_Fulfillment_Instance : MonoBehaviour
{
	TMP_InputField news_inputfield;
	Button confirm_button;

	private void Awake()
	{
		news_inputfield = gameObject.GetComponentInChildren<TMP_InputField>();
		confirm_button = gameObject.GetComponentInChildren<Button>();
		news_inputfield.onValueChanged.AddListener(delegate {
			if (news_inputfield.text.Length < 1) {
				confirm_button.interactable = false;
			}
			else {
				confirm_button.interactable = true;
			}
		});
		confirm_button.onClick.AddListener(() => {
			StopAllCoroutines();
			Send_News_Fulfillment();
		});
	}
	private void OnEnable()
	{
		news_inputfield.text = "";
	}
	public IEnumerator Timeout_Routine(){
		yield return new WaitForSeconds(20.0f);
		Send_News_Fulfillment();
	}
	public void Send_News_Fulfillment(){
		Game_Controller.instance.fulfillment_balloon_controller.Handle_Balloon(false);
		Hashtable fulfillment_data_hash = new Hashtable();
		fulfillment_data_hash.Add("votes", 0);
		fulfillment_data_hash.Add("id_user", User_Manager.instance.current_user.id);
		fulfillment_data_hash.Add("fulfilled_news", ("<b>"+news_inputfield.text+"</b>"));
		Hashtable data_hash = new Hashtable();
		data_hash.Add("fulfillment_data", fulfillment_data_hash);
		data_hash.Add("match_data", Match_Manager.instance.current_match);
		Sockets_Manager.instance.Send_User_Message("news_fulfilled", data_hash);
		Transition_Manager.instance.Change_Panel("13_News_Processing");
	}
}
