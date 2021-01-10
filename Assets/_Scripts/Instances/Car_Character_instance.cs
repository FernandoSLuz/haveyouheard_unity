using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HaveYouHeard;
public class Car_Character_instance : MonoBehaviour
{
	public Match_User match_user;
	public Vector2 origin_pos;
	public Image character_img;
	public Image character_overlay;
	public CanvasGroup alpha;
	RectTransform my_transform;
	public bool is_driver;

	private void Start()
	{
		if(is_driver){
			my_transform = GetComponent<RectTransform>();
			origin_pos = my_transform.anchoredPosition;
			Car_Movement_Controller.instance.driver = this;
		}
	}
	public void Clear_Character(){
		character_img.sprite = null;
		character_overlay.sprite = null;
		match_user = new Match_User();
		origin_pos = Vector2.zero;
		my_transform = null;
	}
	public void Populate_Character(Match_User new_match_user){
		my_transform = GetComponent<RectTransform>();
		match_user = new_match_user;
		character_img.sprite = Match_Manager.instance.current_character.pic;
		character_overlay.sprite = Match_Manager.instance.current_character.pic_overlay;
		origin_pos = my_transform.anchoredPosition;
		Color my_color;
		if (ColorUtility.TryParseHtmlString("#" + match_user.color, out my_color))
		{ character_overlay.color = my_color; }
		this.gameObject.SetActive(true);
		Car_Movement_Controller.instance.players.Add(this);
	}
}
