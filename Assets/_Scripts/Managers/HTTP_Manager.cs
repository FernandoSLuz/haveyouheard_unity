using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using BestHTTP;
using LitJson;

namespace HaveYouHeard
{
	public class HTTP_Manager : MonoBehaviour
	{
		public static HTTP_Manager instance;
		public GameObject Request_Panel;

		private void Awake()
		{
			instance = this;
		}

		//***ADD USER***
		public void Add_User()
		{
			Request_Panel.SetActive(true);
			string uri = Config_Manager.http_uri + Config_Manager.add_user_request;
			HTTPRequest request = new HTTPRequest(new Uri(uri), HTTPMethods.Post, Add_User_Callback);
			request.SetHeader("Content-Type", "application/json");
			Hashtable body_hash = new Hashtable();
			body_hash.Add("username", User_Manager.instance.current_user.username);
			body_hash.Add("country", User_Manager.instance.current_user.country);
			request.RawData = Encoding.UTF8.GetBytes(JsonMapper.ToJson(body_hash));
			request.Send();
		}
		public void Add_User_Callback(HTTPRequest request, HTTPResponse response){
			Request_Panel.SetActive(false);
			if(response.StatusCode == 200){
				//Debug.Log("Success - " + response.DataAsText);
				JsonUtility.FromJsonOverwrite(JsonMapper.ToJson((JsonMapper.ToObject(response.DataAsText)["data"])), User_Manager.instance.current_user);
				User_Manager.instance.Save_User();
				Transition_Manager.instance.Change_Panel("05_Lobby");
			}else{
				Debug_Results(response.StatusCode, response.DataAsText);
			}
		}

		//***JOIN MATCH***
		public void Join_Match()
		{
			Request_Panel.SetActive(true);
			string uri = Config_Manager.http_uri + Config_Manager.join_match_request;
			HTTPRequest request = new HTTPRequest(new Uri(uri), HTTPMethods.Post, Join_Match_Callback);
			request.SetHeader("Content-Type", "application/json");
			Hashtable body_hash = new Hashtable();
			body_hash.Add("user_data", User_Manager.instance.current_user);
			body_hash.Add("match_data", Match_Manager.instance.current_match);
			body_hash.Add("is_player", true);
			string json = JsonMapper.ToJson(body_hash);
			request.RawData = Encoding.UTF8.GetBytes(json);
			request.Send();
		}
		public void Join_Match_Callback(HTTPRequest request, HTTPResponse response)
		{
			Request_Panel.SetActive(false);
			if (response.StatusCode == 200)
			{
				Match_User match_user = new Match_User();
				JsonUtility.FromJsonOverwrite(JsonMapper.ToJson((JsonMapper.ToObject(response.DataAsText)["data"])), match_user);
				Match_Manager.instance.match_users.Add(match_user);
				Match_Manager.instance.match_lobby_controller.clear_match_user_instances();
				Sockets_Manager.instance.JoinRoom();

			}
			else
			{
				Debug_Results(response.StatusCode, response.DataAsText);
			}
		}

		//***CREATE MATCH***
		public void Create_Match(bool is_public)
		{
			Request_Panel.SetActive(true);
			string uri = Config_Manager.http_uri + Config_Manager.create_match_request;
			HTTPRequest request = new HTTPRequest(new Uri(uri), HTTPMethods.Post, Create_Room_Callback);
			request.SetHeader("Content-Type", "application/json");
			Hashtable body_hash = new Hashtable();
			body_hash.Add("user_data", User_Manager.instance.current_user);
			body_hash.Add("is_public", is_public);
			string json = JsonMapper.ToJson(body_hash);
			request.RawData = Encoding.UTF8.GetBytes(json);
			request.Send();
		}
		public void Create_Room_Callback(HTTPRequest request, HTTPResponse response)
		{
			Request_Panel.SetActive(false);
			if(response.StatusCode == 200){
				JsonUtility.FromJsonOverwrite(JsonMapper.ToJson((JsonMapper.ToObject(response.DataAsText)["data"])), Match_Manager.instance.current_match);
				Join_Match();
			}else{
				Debug_Results(response.StatusCode, response.DataAsText);
			}
		}

		//***SEARCH MATCH***
		public void Search_Match(int match_id)
		{
			Request_Panel.SetActive(true);
			string uri = Config_Manager.http_uri + Config_Manager.get_match_request;
			HTTPRequest request = new HTTPRequest(new Uri(uri), HTTPMethods.Get, Search_Match_Callback);
			request.SetHeader("Content-Type", "application/json");
			Hashtable body_hash = new Hashtable();
			body_hash.Add("id", match_id);
			string json = JsonMapper.ToJson(body_hash);
			request.RawData = Encoding.UTF8.GetBytes(json);
			request.Send();
		}
		public void Search_Match_Callback(HTTPRequest request, HTTPResponse response)
		{
			Request_Panel.SetActive(false);
			if (response.StatusCode == 200)
			{
				JsonUtility.FromJsonOverwrite(JsonMapper.ToJson((JsonMapper.ToObject(response.DataAsText)["data"])), Match_Manager.instance.current_match);
				Join_Match();
			}
			else
			{
				Debug_Results(response.StatusCode, response.DataAsText);
			}
		}


		//***SEARCH MATCHES***
		public void Search_Matches()
		{

		}
		public void Search_Matches_Callback(HTTPRequest request, HTTPResponse response)
		{

		}


		public void Debug_Results(int status_code, string response_data){
			switch (status_code)
			{
				case 406:
					//MISSING POST DATA
					Debug.Log("Missing POST data - " + response_data);
					break;
				case 400:
					//BAD REQUEST
					Debug.Log("Bad request - " + response_data);
					break;
				default:
					//OTHER ERRORS
					Debug.Log("unknown error - " + response_data);
					break;
			}
		}
	}
}
