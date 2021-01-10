using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HaveYouHeard;
using TMPro;
public class Player_Settings_Instance : MonoBehaviour
{
	TextMeshProUGUI username_text;
	public Button profile_btn;
	private void Awake()
	{
		username_text = gameObject.GetComponentInChildren<TextMeshProUGUI>();
		profile_btn = gameObject.GetComponentInChildren<Button>();
		profile_btn.onClick.AddListener(() => {
			Debug.Log("btn clicked");
			Transition_Manager.instance.StartCoroutine(Transition_Manager.instance.Change_Panel_Routine("05.1_Player_Settings", Transitions.none));
		});
	}
	private void OnEnable()
	{
		username_text.text = User_Manager.instance.current_user.username;
	}
}
