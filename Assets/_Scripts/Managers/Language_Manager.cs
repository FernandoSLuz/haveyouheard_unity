using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HaveYouHeard
{
	public class Language_Manager : MonoBehaviour
	{
		public static Language_Manager instance;
		public delegate void Callback_Delegate();
		public Callback_Delegate callback;
		private void Awake()
		{
			instance = this;
		}
		public void Set_Language(string country, Callback_Delegate new_callback = null)
		{
			//CHANGELATER - MAKE A PROPER REQUEST, AND THEN CALL POPULATE_TEXTS
			User_Manager.instance.current_user.country = country;
			callback = new_callback;
			Populate_Texts("");
		}
		public void Populate_Texts(string texts_json)
		{
			//CHANGELATER - POPULATE TEXTS WITH THE CALLBACK
			if (callback != null){
				callback();
				callback = null;
			}
		}
	}
}
