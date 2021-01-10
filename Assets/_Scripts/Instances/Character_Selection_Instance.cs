using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using HaveYouHeard;
public class Character_Selection_Instance : MonoBehaviour
{
    public Character selection_character;
    public bool selected;
    public CanvasGroup alpha;
    public Image text_box_overlay;
    public TextMeshProUGUI character_name_text;
    public Image thumb;
    public Button description_btn;
    public Button selection_btn;

    public void Populate_Controller(Character character, Match_Character_Selection_Controller instance){
        selection_character = character;
        character_name_text.text = selection_character.name;
        thumb.sprite = selection_character.thumb;
        description_btn.onClick.AddListener(() => {
            instance.Set_Description(this);
        });
        selection_btn.onClick.AddListener(() => {
            instance.character_selected(this);
        });
    }
}
