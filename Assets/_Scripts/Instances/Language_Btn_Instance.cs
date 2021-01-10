using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HaveYouHeard;
public class Language_Btn_Instance : MonoBehaviour
{
	public string country;
	Button btn;
	private void Awake()
	{
		btn = gameObject.GetComponent<Button>();
		btn.onClick.AddListener(() => {
			Language_Manager.instance.Set_Language(country, Proceed);
		});
	}
	public void Proceed(){
		Transition_Manager.instance.Change_Panel("03_Intro");
	}
}
