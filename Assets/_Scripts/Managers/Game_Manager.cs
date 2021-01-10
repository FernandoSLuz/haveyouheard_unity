using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HaveYouHeard
{
	public class Game_Manager : MonoBehaviour
	{
		public static Game_Manager instance;
		public bool game_started;
		private void Awake()
		{
			Application.targetFrameRate = 60;
			instance = this;
		}
		public void Get_User_Data()
		{
			game_started = true;
			if (User_Manager.instance.Check_If_User_Is_Registered()){
				Language_Manager.instance.Set_Language(User_Manager.instance.current_user.country, Skip_Intro);
			} else{
				Transition_Manager.instance.StartCoroutine(Transition_Manager.instance.Change_Panel_Routine("02_Language",Transitions.fade));
			}
		}
		public void Skip_Intro(){
			Transition_Manager.instance.Change_Panel("05_Lobby");
		}
	}
}
