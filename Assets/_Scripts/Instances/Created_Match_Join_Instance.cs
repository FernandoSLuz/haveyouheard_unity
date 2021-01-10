using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using HaveYouHeard;

public class Created_Match_Join_Instance : MonoBehaviour
{
	public TextMeshProUGUI virtual_text;
	TMP_InputField room_name_inputfield;
	Button confirm_button;

	public void Update_Text(string actual_text){
		string updated_text = actual_text;
		while(updated_text.Length != 6){
			updated_text = "0" + updated_text;
		}
		updated_text = "#" + updated_text;
		virtual_text.text = updated_text;
	}
	private void Awake()
	{
		room_name_inputfield = gameObject.GetComponentInChildren<TMP_InputField>();
		confirm_button = gameObject.GetComponentInChildren<Button>();
		room_name_inputfield.onValueChanged.AddListener(delegate {
			Update_Text(room_name_inputfield.text);
			if (room_name_inputfield.text.Length < 3) {
				confirm_button.interactable = false;
			}
			else {
				confirm_button.interactable = true;
			}
		});
		confirm_button.onClick.AddListener(() => {
			HTTP_Manager.instance.Search_Match(int.Parse(virtual_text.text.Replace("#","")));
		});
	}
	private void OnEnable()
	{
		room_name_inputfield.text = "";
		virtual_text.text = "";
	}
}
