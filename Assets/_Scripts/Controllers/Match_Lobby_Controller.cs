using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HaveYouHeard;
using System.Linq;
using TMPro;
public class Match_Lobby_Controller : MonoBehaviour
{
	public Transform Users_Tab;
	public List<Match_User_Instance> match_user_instances = new List<Match_User_Instance>(); 
	public GameObject Match_Lobby_User_Prefab;
	public TextMeshProUGUI room_number;
	public Image fill_bar;
	public void clear_match_user_instances(){
		foreach(var item in match_user_instances){
			Destroy(item.gameObject);
			match_user_instances.Clear();
		}
	}
	public IEnumerator Start_Countdown(){
		float timer = 0.0f;
		while(timer<1.0f){
			timer += Time.deltaTime / 5.0f;
			fill_bar.fillAmount = timer;
			if(Match_Manager.instance.current_match.status != "starting_game"){
				fill_bar.fillAmount = 0.0f;
				timer = 0.0f;
				break;
			}
			yield return null;
		}
		if(timer >= 1.0f){
			Hashtable hash = new Hashtable();
			hash.Add("match_data", Match_Manager.instance.current_match);
			Sockets_Manager.instance.Send_User_Message("check_match_status", hash);
		}
	}
	public void populate_match_user_instances(){
		room_number.text = Match_Manager.instance.current_match.id.ToString();
		while(room_number.text.Length < 6){
			room_number.text = "0" + room_number.text;
		}
		room_number.text = "#" + room_number.text;
		foreach (var item in Match_Manager.instance.match_users){
			bool has = match_user_instances.Any(cus => cus.current_match_user.id_user == item.id_user);
			if(!has){
				GameObject go = Instantiate(Match_Lobby_User_Prefab, Users_Tab, false);
				Match_User_Instance temp = go.GetComponent<Match_User_Instance>();
				temp.current_match_user = item;
				foreach (var user in Match_Manager.instance.users)
				{
					if (temp.current_match_user.id_user == user.id)
					{
						temp.name_text.text = user.username;
						break;
					}
				}
				match_user_instances.Add(temp);
			}
		}
	}
	public GameObject Im_Ready_Text;
	public GameObject Im_Not_Ready_Text;
	public void Ready(){
		foreach(var item in Match_Manager.instance.match_users){
			if (item.id_user == User_Manager.instance.current_user.id){
				Match_User temp = item;
				temp.ready = !item.ready;
				Hashtable hash = new Hashtable();
				hash.Add("match_data", Match_Manager.instance.current_match);
				hash.Add("match_user_data", temp);
				Sockets_Manager.instance.Send_Match_Message("user_ready", hash);
			}
		}
	}

	public void Update_Status()
	{
		foreach (var item in match_user_instances)
		{
			Match_User temp_match_user = Match_Manager.instance.match_users.Where(x => x.id == item.current_match_user.id).SingleOrDefault();
			item.current_match_user.color = temp_match_user.color;
			item.current_match_user.ready = temp_match_user.ready;
			item.Check_Status();
			if(item.current_match_user.id_user == User_Manager.instance.current_user.id){
				Im_Not_Ready_Text.SetActive(temp_match_user.ready);
				Im_Ready_Text.SetActive(!temp_match_user.ready);
			}
		}
	}
}
