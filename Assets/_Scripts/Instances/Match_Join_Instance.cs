using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HaveYouHeard;
public class Match_Join_Instance : MonoBehaviour
{
	public Join_Types join_type;
	Button join_btn;

	private void Awake()
	{
		join_btn = gameObject.GetComponent<Button>();
		join_btn.onClick.AddListener(() => {
			switch(join_type){
				case Join_Types.create_private_room:
					HTTP_Manager.instance.Create_Match(false);
					break;
				case Join_Types.join_public_room:
					HTTP_Manager.instance.Search_Matches();
					break;
			}
		});
	}
	[System.Serializable]
	public enum Join_Types
	{
		create_private_room,
		join_public_room
	}
}
