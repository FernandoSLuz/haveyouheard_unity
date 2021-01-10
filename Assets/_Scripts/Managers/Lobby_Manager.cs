using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HaveYouHeard
{
	public class Lobby_Manager : MonoBehaviour
	{
		public static Lobby_Manager instance;
		private void Awake()
		{
			instance = this;
		}
	}
}

