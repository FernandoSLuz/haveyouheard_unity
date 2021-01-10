using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
namespace HaveYouHeard
{
	public class Match_Manager : MonoBehaviour
	{
		public static Match_Manager instance;
		public List<Match_User> match_users = new List<Match_User>();
		public List<User> users = new List<User>();
		public Match current_match = new Match();
		public Character current_character = new Character();
		public List<Character> characters = new List<Character>();
		public List<Round> rounds = new List<Round>();
		public Match_Lobby_Controller match_lobby_controller;
		private void Awake()
		{
			instance = this;
		}
		public IEnumerator Update_Character_Selection_Routine(JsonData data){
			Character temp = new Character();
			//print(JsonMapper.ToJson(data["character_data"]));
			JsonUtility.FromJsonOverwrite(JsonMapper.ToJson(data["character_data"]), temp);
			foreach (var loop_character in characters){ 
				if(loop_character.id == temp.id)
				{
					current_character = loop_character;
					break;
				}
			}
			yield return new WaitForSeconds(2.0f);
			Transition_Manager.instance.Change_Panel("09_Character_Info");
		}
		public void Get_Match_Status(JsonData data)
		{
			string status = (string)data["status"];
			switch(status){
				case "starting_game":
					Transition_Manager.instance.Change_Panel("07_Character_Selection");
					break;
			}
		}
		public void Update_Users_Matchmaking_Status(JsonData data){
			JsonUtility.FromJsonOverwrite(JsonMapper.ToJson(data["match_data"]), current_match);
			Match_User temp = new Match_User();
			JsonUtility.FromJsonOverwrite(JsonMapper.ToJson(data["match_user_data"]), temp);
			foreach (var item in match_users){
				if (item.id == temp.id){
					item.ready = temp.ready;
				}
			}
			if(current_match.status == "starting_game"){
				match_lobby_controller.StartCoroutine(match_lobby_controller.Start_Countdown());
				foreach (JsonData round_data in data["rounds_data"])
				{
					Round temp_round = new Round();
					JsonUtility.FromJsonOverwrite(JsonMapper.ToJson(round_data), temp_round);
					rounds.Add(temp_round);
				}
				foreach (JsonData character_data in data["characters_data"])
				{
					Character temp_character = new Character();
					JsonUtility.FromJsonOverwrite(JsonMapper.ToJson(character_data), temp_character);
					temp_character.Load_Sprites();
					characters.Add(temp_character);
				}
			}
		}
		public void Update_Matchmaking_Status(JsonData data)
		{
			if (Transition_Manager.instance.actual_panel.name == "05_Lobby")
			{
				StartCoroutine(Transition_Manager.instance.Change_Panel_Routine("06_Match_Lobby"));
			}
			JsonUtility.FromJsonOverwrite(JsonMapper.ToJson(data["match_data"]), current_match);
			foreach (JsonData user_data in data["users_data"])
			{
				bool can_add = true;
				User temp_user = new User();
				JsonUtility.FromJsonOverwrite(JsonMapper.ToJson(user_data), temp_user);
				foreach (var user in users)
				{
					if (user.id == temp_user.id)
					{
						can_add = false;
						break;
					}
				}
				if (can_add)
				{
					users.Add(temp_user);
				}

			}
			foreach (JsonData match_user_data in data["match_users_data"])
			{
				bool can_add = true;
				Match_User temp_matach_user = new Match_User();
				JsonUtility.FromJsonOverwrite(JsonMapper.ToJson(match_user_data), temp_matach_user);
				foreach (var match_user in match_users)
				{
					if (match_user.id == temp_matach_user.id)
					{
						match_user.color = temp_matach_user.color;
						match_user.ready = temp_matach_user.ready;
						can_add = false;
						break;
					}
				}
				if (can_add)
				{
					match_users.Add(temp_matach_user);
				}
			}
			current_match.status = (string)data["match_data"]["status"];
			match_lobby_controller.populate_match_user_instances();
			match_lobby_controller.Update_Status();
		}
	}
	[System.Serializable]
	public class Confirmed_Players{

	}
	[System.Serializable]
	public class Match_User{
		public int id;
		public int id_match;
		public int id_user;
		public bool is_player;
		public bool won;
		public int points;
		public bool ready;
		public string color;
	}
	[System.Serializable]
	public class Match
	{
		public int id;
		public bool is_public;
		public string status;
		public string country;
	}
	[System.Serializable]
	public class Character
	{
		public int id;
		public string name;
		public string description;
		public string country;
		public Sprite thumb;
		public Sprite pic;
		public Sprite pic_overlay;

		public void Load_Sprites(){
			pic = Resources.Load<Sprite>("Characters/" + name);
			pic_overlay = Resources.Load<Sprite>("Characters/" + name + "_overlay");
			thumb = Resources.Load<Sprite>("Characters/" + name + "_thumb");
		}
	}
	[System.Serializable]
	public class Round
	{
		public int id;
		public bool id_topic;
		public string url;
		public string complete_text;
		public string incomplete_text;
		public int round;
		public bool on_going;
		public List<FulFillment> fulfillments = new List<FulFillment>();
		public Round_Winner round_winner;
	}
	[System.Serializable]
	public class FulFillment{
		public int id_user;
		public string fulfilled_news;
	}
	[System.Serializable]
	public class Round_Winner{
		public int id_user;
		public string fulfilled_news;
		public int votes;
	}
}
