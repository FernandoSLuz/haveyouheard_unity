using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using HaveYouHeard;

public class Name_Subscription_Instance : MonoBehaviour
{
	TMP_InputField name_inputfield;
	Button confirm_button;

	private void Awake()
	{
		name_inputfield = gameObject.GetComponentInChildren<TMP_InputField>();
		confirm_button = gameObject.GetComponentInChildren<Button>();
		name_inputfield.onValueChanged.AddListener(delegate {
			if (name_inputfield.text.Length < 4) {
				confirm_button.interactable = false;
			}
			else {
				confirm_button.interactable = true;
			}
		});
		confirm_button.onClick.AddListener(() => {
			User_Manager.instance.current_user.username = name_inputfield.text;
			HTTP_Manager.instance.Add_User();
		});
	}
	private void OnEnable()
	{
		name_inputfield.text = "";
	}
}
