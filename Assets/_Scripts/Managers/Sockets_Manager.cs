using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using BestHTTP;
using BestHTTP.SocketIO;
using LitJson;

namespace HaveYouHeard
{
	public class Sockets_Manager : MonoBehaviour
	{
		public static Sockets_Manager instance;
		SocketManager manager;
		public bool connected = false;
		private void Awake()
		{
			instance = this;
		}
		void Start()
		{
			ConnectToServer();
		}
		void OnDestroy()
		{
			manager.Close();
		}
		public Image connection_feedback;
		private void Update()
		{
			if(connection_feedback != null){
				if (manager.Socket.IsOpen)
				{
					connection_feedback.color = Color.green;
				}
				else
				{
					connection_feedback.color = Color.red;
				}
			}
			if (Input.GetKeyDown(KeyCode.G))
			{
				Hashtable hash = new Hashtable();
				hash.Add("message", "Test Global Message");
				Send_Global_Message("test_global_message", hash);
			}
			if (Input.GetKeyDown(KeyCode.M))
			{
				Hashtable hash = new Hashtable();
				hash.Add("message", "Test match Message");
				hash.Add("match_data", Match_Manager.instance.current_match);
				Send_Match_Message("test_match_message", hash);
			}
		}
		public void DisconnectFromServer(){
			manager.Socket.Disconnect();
		}
		public void ConnectToServer()
		{
			SocketOptions options = new SocketOptions();
			options.AutoConnect = true;
			manager = new SocketManager(new Uri(Config_Manager.socket_uri), options);
			manager.Socket.On(SocketIOEventTypes.Connect, OnServerConnect);
			manager.Socket.On(SocketIOEventTypes.Disconnect, OnServerDisconnect);
			manager.Socket.On(SocketIOEventTypes.Error, OnError);
			manager.Socket.On("reconnect", OnReconnect);
			manager.Socket.On("reconnecting", OnReconnecting);
			manager.Socket.On("reconnect_attempt", OnReconnectAttempt);
			manager.Socket.On("reconnect_failed", OnReconnectFailed);
			manager.Socket.On("user_response", On_User_Response);
			manager.Socket.On("global_response", On_Global_Response);
			manager.Socket.On("match_response", On_Match_Response);
			manager.Open();
		}

		public void JoinRoom()
		{
			Hashtable hash = new Hashtable();
			hash.Add("user_data", User_Manager.instance.current_user);
			hash.Add("match_users_data", Match_Manager.instance.match_users);
			hash.Add("match_data", Match_Manager.instance.current_match);
			string json = JsonMapper.ToJson(hash);
			manager.Socket.Emit("join", json);
		}
		public void Send_User_Message(string action, Hashtable data_hash)
		{
			Hashtable hash = new Hashtable();
			hash.Add("action", action);
			hash.Add("data", data_hash);
			string json = JsonMapper.ToJson(hash);
			manager.Socket.Emit("user_event", json);
		}
		public void Send_Global_Message(string action, Hashtable data_hash)
		{
			Hashtable hash = new Hashtable();
			hash.Add("action", action);
			hash.Add("data", data_hash);
			string json = JsonMapper.ToJson(hash);
			manager.Socket.Emit("global_event", json);
		}
		public void Send_Match_Message(string action, Hashtable data_hash)
		{
			Hashtable hash = new Hashtable();
			hash.Add("action", action);
			hash.Add("data", data_hash);
			string json = JsonMapper.ToJson(hash);
			manager.Socket.Emit("match_event", json);
		}

		public Hashtable Parse_Response(params object[] args)
		{
			Hashtable hash = new Hashtable();
			foreach (Dictionary<string, System.Object> dic in args)
			{
				foreach (var key in dic.Keys)
				{
					hash.Add(key, dic[key]);
				}
			}
			return hash;
		}

		void On_User_Response(Socket socket, Packet packet, params object[] args)
		{
			Hashtable hash_response = Parse_Response(args);
			switch (hash_response["action"])
			{
				case "check_match_status":
					Match_Manager.instance.Get_Match_Status(JsonMapper.ToObject(JsonMapper.ToJson(hash_response["data"])));
					break;
				case "background_thread":
					//Debug.Log("Background_Thread");
					break;
				case "connection":
					Debug.Log("emit - connected");
					break;
			}
		}
		void On_Global_Response(Socket socket, Packet packet, params object[] args)
		{
			Hashtable hash_response = Parse_Response(args);
			Debug.Log(JsonMapper.ToJson(hash_response));
		}
		void On_Match_Response(Socket socket, Packet packet, params object[] args)
		{
			Hashtable hash_response = Parse_Response(args);
			switch(hash_response["action"])
			{
				case "rejoin":
					Debug.Log("USER RECONECTED");
					Debug.Log(JsonMapper.ToJson(hash_response["data"]));
					break;
				case "join_match":
					JsonData temp_data = JsonMapper.ToObject(JsonMapper.ToJson(hash_response["data"]));
					Match_Manager.instance.Update_Matchmaking_Status(temp_data);
					break;
				case "user_ready":
					Match_Manager.instance.Update_Users_Matchmaking_Status(JsonMapper.ToObject(JsonMapper.ToJson(hash_response["data"])));
					Match_Manager.instance.match_lobby_controller.populate_match_user_instances();
					Match_Manager.instance.match_lobby_controller.Update_Status();
					break;
				case "characters_voted":
					Match_Manager.instance.StartCoroutine(Match_Manager.instance.Update_Character_Selection_Routine(JsonMapper.ToObject(JsonMapper.ToJson(hash_response["data"]))));
					break;
				case "news_fulfilled":
					Game_Controller.instance.StartCoroutine(Game_Controller.instance.Answers_Recieved_Routine(JsonMapper.ToObject(JsonMapper.ToJson(hash_response["data"]))));
					break;
				case "news_voting_finished":
					//Debug.Log(JsonMapper.ToJson(hash_response["data"]));
					Game_Controller.instance.StartCoroutine(Game_Controller.instance.Voting_Processed_Routine(JsonMapper.ToObject(JsonMapper.ToJson(hash_response["data"]))));
					break;
				default:
					//Debug.Log(JsonMapper.ToJson(hash_response));
					break;
			}
		}


		void OnServerConnect(Socket socket, Packet packet, params object[] args)
		{
			Debug.Log("Connected");
			if (!connected)
			{
				Invoke("DisconnectFromServer", 2.0f);
			}else if(!Game_Manager.instance.game_started){
				Game_Manager.instance.Get_User_Data();
			}
		}

		void OnServerDisconnect(Socket socket, Packet packet, params object[] args)
		{
			Debug.Log("Disconnected");
			if (!connected)
			{
				connected = true;
				Invoke("ConnectToServer", 2.0f);
			}
		}

		void OnError(Socket socket, Packet packet, params object[] args)
		{
			Error error = args[0] as Error;

			switch (error.Code)
			{
				case SocketIOErrors.User:
					Debug.LogWarning("Exception in an event handler!");
					break;
				case SocketIOErrors.Internal:
					Debug.LogWarning("Internal error!");
					break;
				default:
					Debug.LogWarning("server error!");
					break;
			}
		}

		void OnReconnect(Socket socket, Packet packet, params object[] args)
		{
			Debug.Log("Reconnected");
			if(Match_Manager.instance.current_match.id != 0){
				Hashtable hash = new Hashtable();
				hash.Add("user_data", User_Manager.instance.current_user);
				hash.Add("match_data", Match_Manager.instance.current_match);
				string json = JsonMapper.ToJson(hash);
				manager.Socket.Emit("rejoin", json);
			}
		}

		void OnReconnecting(Socket socket, Packet packet, params object[] args)
		{
			Debug.Log("Reconnecting");
		}

		void OnReconnectAttempt(Socket socket, Packet packet, params object[] args)
		{
			Debug.Log("ReconnectAttempt");
		}

		void OnReconnectFailed(Socket socket, Packet packet, params object[] args)
		{
			Debug.Log("ReconnectFailed");
		}
	}
}
