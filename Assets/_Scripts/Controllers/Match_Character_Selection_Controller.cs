using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HaveYouHeard;
using TMPro;
public class Match_Character_Selection_Controller : MonoBehaviour
{
    public List<Transform> character_rows = new List<Transform>();
    public GameObject character_selection_prefab;
    public GameObject empty_character_prefab;
    public List<Character_Selection_Instance> character_selection_instances = new List<Character_Selection_Instance>();
    public Button ok_btn;
    public GameObject character_description_box;
    public TextMeshProUGUI character_description_name;
    public TextMeshProUGUI character_description_description;
    public Image fill_bar;

    private void OnEnable()
    {
        foreach(var item in character_selection_instances){
            Destroy(item.gameObject);
        }
        character_selection_instances.Clear();
        foreach (var item in Match_Manager.instance.characters){
            foreach(var row in character_rows){ 
                if(row.childCount < 3){
                    GameObject go = Instantiate(character_selection_prefab, row, false);
                    Character_Selection_Instance temp = go.GetComponent<Character_Selection_Instance>();
                    temp.Populate_Controller(item, this);
                    character_selection_instances.Add(temp);
                    break;
                }
            }
        }
        foreach (var row in character_rows)
        {
            while(row.childCount < 3){
                GameObject go = Instantiate(empty_character_prefab, row, false);
            }
        }
        StartCoroutine(Character_Selection_Routine());
    }
    public IEnumerator Character_Selection_Routine(){
        yield return StartCoroutine(Game_Controller.instance.Fill_Bar_Routine(fill_bar, 20.0f));
        Start_Game_Request();
    }
    public void Set_Description(Character_Selection_Instance highlited_character){
        character_description_description.text = highlited_character.selection_character.description;
        character_description_name.text = highlited_character.selection_character.name;
        character_description_box.SetActive(true);
    }
    public void character_selected(Character_Selection_Instance highlited_character){
        highlited_character.selected = !highlited_character.selected;
        ok_btn.interactable = highlited_character.selected;
        highlited_character.alpha.alpha = 1.0f;
        if (highlited_character.selected){
            ok_btn.GetComponent<CanvasGroup>().alpha = 1.0f;
        }
        else{
            ok_btn.GetComponent<CanvasGroup>().alpha = 0.5f;
        }
        foreach (var item in character_selection_instances){ 
            if(item != highlited_character){
                item.selected = false;
                if(highlited_character.selected){
                    item.alpha.alpha = 0.5f;
                }
                else{
                    item.alpha.alpha = 1.0f;
                }
            }
        }
    }
    public void Start_Game_Request(){
        Character selected_character = null;
        foreach(var item in character_selection_instances){ 
            if(item.selected){ 
                selected_character = item.selection_character;
                break;
            }
        }
        Transition_Manager.instance.Change_Panel("08_Game_Loader");
        Hashtable hash = new Hashtable();
        Hashtable character_hash = new Hashtable();
        if(selected_character == null){
            character_hash.Add("id", -1);
        }else{
            character_hash.Add("id", selected_character.id);
            character_hash.Add("name", selected_character.name);
            character_hash.Add("description", selected_character.description);
            character_hash.Add("country", selected_character.country);
        }
        hash.Add("character_data", character_hash);
        hash.Add("match_data", Match_Manager.instance.current_match);
        Sockets_Manager.instance.Send_User_Message("send_character_selection", hash);
    }

    public IEnumerator Character_Selected_Routine(){
        yield return new WaitForSeconds(1.0f);
    }
}
