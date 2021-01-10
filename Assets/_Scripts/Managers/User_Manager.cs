using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HaveYouHeard
{
	public class User_Manager : MonoBehaviour
	{
		public User current_user = new User();
		public static User_Manager instance;
		public bool clear_data;
		private void Awake()
		{
			if (clear_data) Clear_Data();
			instance = this;
		}
		public void Clear_Data(){
			PlayerPrefs.DeleteAll();
		}
		public void Save_User(){
			string user_json = JsonUtility.ToJson(current_user);
			//Debug.Log(user_json);
			PlayerPrefs.SetString("user_json",user_json);
		}
		public bool Check_If_User_Is_Registered(){
			if (PlayerPrefs.HasKey("user_json")){
				JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString("user_json"), current_user);
				return true;
			}else{
				return false;
			}
		}
	}
}
[System.Serializable]
public class User{
	public string username;
	public int id;
	public string country;
}