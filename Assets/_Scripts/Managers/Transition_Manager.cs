using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition_Manager : MonoBehaviour
{
	public static Transition_Manager instance;
	public GameObject wait_panel;
	public GameObject actual_panel;
	public List<GameObject> panels = new List<GameObject>();
	private void Awake()
	{
		instance = this;
	}
	public void Change_Panel(string panel_name){
		StartCoroutine(Change_Panel_Routine(panel_name));
	}
	public IEnumerator Change_Panel_Routine(string new_panel_str,
		Transitions panels_transition_type = Transitions.scroll, bool play_together = true,
		float actual_panel_speed = 1.0f, float new_panel_speed = 1.0f)
	{
		wait_panel.SetActive(true);
		GameObject new_panel = null;
		foreach(var item in panels){
			if (item.name == new_panel_str) {
				new_panel = item;
				break;
			}
		}
		string transition = "";
		switch (panels_transition_type)
		{
			case Transitions.scroll:
				transition = "_scroll";
				break;
			case Transitions.fade:
				transition = "_fade";
				break;
		}
		if(panels_transition_type != Transitions.none){
			Animator actual_panel_anim = null;
			Animator new_panel_anim = new_panel.GetComponent<Animator>();
			new_panel_anim.enabled = true;
			if (actual_panel != null)
			{
				actual_panel_anim = actual_panel.GetComponent<Animator>();
				actual_panel_anim.enabled = true;
				float wait_time = Play_Anim(actual_panel_anim, "out" + transition, actual_panel_speed);
				if (!play_together)
				{
					yield return new WaitForSeconds(wait_time);
				}
			}
			yield return new WaitForSeconds(Play_Anim(new_panel_anim, "in" + transition, new_panel_speed));
			if (actual_panel != null)
			{
				actual_panel_anim.enabled = false;
				actual_panel.SetActive(false);
			}
			actual_panel = new_panel;
			actual_panel_anim.enabled = false;
			wait_panel.SetActive(false);
		}
		else{
			actual_panel.SetActive(false);
			actual_panel = new_panel;
			actual_panel.SetActive(true);
		}

	}
	float Play_Anim(Animator anim, string anim_name, float speed){
		speed = 1 / speed;
		anim.gameObject.SetActive(true);
		anim.speed = speed;
		anim.Play(anim_name);
		float wait_time = anim.GetCurrentAnimatorStateInfo(0).length / speed;
		
		return wait_time;
	}
}
[System.Serializable]
public enum Transitions
{
	scroll,
	fade,
	none
}