using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HaveYouHeard;
using TMPro;
using UnityEngine.SceneManagement;
using LitJson;
public class Game_Controller : MonoBehaviour
{
    public static Game_Controller instance;
    public TextMeshProUGUI round_announcement_title_round_text;
    public TextMeshProUGUI round_announcement_round_text;
    public TextMeshProUGUI news_announcement_round_text;
    public TextMeshProUGUI votes_winner_round_text;
    public TextMeshProUGUI news_revelation_round_text;
    public TextMeshProUGUI news_results_round_text;

    public GameObject car_box;

    public Balloon_Controller center_balloon_controller;
    public Balloon_Controller fulfillment_balloon_controller;
    public Balloon_Controller answers_balloon_controller;

    public Image round_announcement_fill_bar;
    public Image news_announcement_fill_bar;
    public Image news_fulfillment_fill_bar;
    public Image news_voting_fill_bar;
    public Image votes_winner_fill_bar;
    public Image news_revelation_fill_bar;
    public Image news_results_fill_bar;

    public Image news_fulfillment_character;
    public Image news_fulfillment_character_overlay;
    public Image news_fulfillment_character_shadow;

    public Image game_winner_character;
    public Image game_winner_overlay;
    public Image game_winner_shadow;
    public TextMeshProUGUI winner_banner;

    public GameObject news_voting_answer_prefab;
    public GameObject user_statistic_prefab;
    public Transform news_voting_answers_parent;
    public Transform user_statistic_parent;

    public int actual_round = 0;

    public News_Fulfillment_Instance news_fulfillment_instance;

    public List<Car_Character_instance> car_characters = new List<Car_Character_instance>();
    public List<Car_Character_instance> actual_car_characters = new List<Car_Character_instance>();
    public List<User_Statistic_Instance> user_statistic_instances = new List<User_Statistic_Instance>();
    public List<News_Voting_Answer_Instance> news_voting_answer_instances = new List<News_Voting_Answer_Instance>();

    string player_color;
    private void Awake()
    {
        instance = this;
    }
    public IEnumerator Fill_Bar_Routine(Image actual_fill_bar, float fill_seconds){
        float timer = 0.0f;
        while(timer < 1.0f){
            actual_fill_bar.fillAmount = timer;
            timer += Time.deltaTime / fill_seconds;
            yield return null;
        }
    }
    void Populate_Car_Characters(){
        Car_Movement_Controller.instance.Clear_Players();
        for(int i = 0; i<Match_Manager.instance.match_users.Count; i++){
            actual_car_characters.Add(car_characters[i]);
            actual_car_characters[i].Populate_Character(Match_Manager.instance.match_users[i]);
        }    
    }
    void Unpopulate_Car_Characters(){
        foreach(var item in actual_car_characters){
            item.Clear_Character();
        }
    }
    public void Disable_Car(){
        car_box.SetActive(false);
        Car_Movement_Controller.instance.ResetPosition();
    }
    public void Populate_Statistics(){ 
        foreach(var item in user_statistic_instances){
            Destroy(item.gameObject);
        }
        user_statistic_instances.Clear();
        foreach(var item in Match_Manager.instance.match_users){
            GameObject go = Instantiate(user_statistic_prefab, user_statistic_parent, false);
            User_Statistic_Instance temp = go.GetComponent<User_Statistic_Instance>();
            temp.Populate(item);
            user_statistic_instances.Add(temp);
        }
    }
    public void ChangeCharacterImg(Image character_overlay, Image character, Image character_shadow, Sprite sprite_overlay, Sprite sprite, string color)
    {
        Color my_color;
        if (ColorUtility.TryParseHtmlString("#" + color, out my_color))
        { character_overlay.color = my_color; }
        character.sprite = sprite;
        character_overlay.sprite = sprite_overlay;
        character_shadow.sprite = sprite;
    }
    public IEnumerator Round_Routine_1(){
        Populate_Statistics();
        foreach (var item in Match_Manager.instance.match_users){ 
            if(item.id_user == User_Manager.instance.current_user.id){
                player_color = item.color;
                break;
            }
        }
        ChangeCharacterImg(news_fulfillment_character_overlay, news_fulfillment_character, news_fulfillment_character_shadow, Match_Manager.instance.current_character.pic_overlay, Match_Manager.instance.current_character.pic, player_color);
        Populate_Car_Characters();
        round_announcement_title_round_text.text = "RODADA " + (actual_round + 1);
        round_announcement_round_text.text = "RODADA " + (actual_round + 1);
        news_announcement_round_text.text = "RODADA " + (actual_round + 1);
        votes_winner_round_text.text = "VENCEDOR - RODADA " + (actual_round + 1);
        news_revelation_round_text.text = "FIM DA RODADA " + (actual_round + 1);
        news_results_round_text.text = "RODADA " + (actual_round + 1);
        StartCoroutine(Transition_Manager.instance.Change_Panel_Routine("10_Round_Announcement", Transitions.fade));
        car_box.SetActive(true);
        StartCoroutine(Fill_Bar_Routine(round_announcement_fill_bar, 5.0f));
        yield return new WaitForSeconds(5.0f);
        StartCoroutine(Transition_Manager.instance.Change_Panel_Routine("11_News_Announcment", Transitions.fade));
        StartCoroutine(Fill_Bar_Routine(news_announcement_fill_bar, 5.0f));
        yield return StartCoroutine(Car_Movement_Controller.instance.Move_Car_Routine(Car_Movement_Controller.instance.driver, 1.0f));
        //MOVIMENTAR CARRO PARA CENTRALIZAR MOTORISTA
        center_balloon_controller.Handle_Balloon(true, Match_Manager.instance.rounds[actual_round].incomplete_text, "Você ouviu que...", "252C4A");
        yield return new WaitForSeconds(4.0f);
        center_balloon_controller.Handle_Balloon(false);
        //MOVIMENTAR CARRO PARA CENTRALIZAR O JOGADOR
        yield return new WaitForSeconds(1.0f);
        car_box.GetComponent<Animator>().Play("out_fade");
        Invoke("Disable_Car", 1.0f);
        StartCoroutine(Transition_Manager.instance.Change_Panel_Routine("12_News_Fulfillment", Transitions.fade));
        fulfillment_balloon_controller.Handle_Balloon(true, Match_Manager.instance.rounds[actual_round].incomplete_text, "O que?", player_color);
        StartCoroutine(Fill_Bar_Routine(news_fulfillment_fill_bar, 20.0f));
        news_fulfillment_instance.StartCoroutine(news_fulfillment_instance.Timeout_Routine());
    }
    
    public IEnumerator Answers_Recieved_Routine(JsonData data)
    {
        foreach(var item in news_voting_answer_instances){
            Destroy(item.gameObject);
        }
        news_voting_answer_instances.Clear();
        foreach(JsonData answer_data in data["fulfillments"]){
            GameObject go = Instantiate(news_voting_answer_prefab, news_voting_answers_parent, false);
            News_Voting_Answer_Instance temp = go.GetComponent<News_Voting_Answer_Instance>();
            temp.populate_answer(JsonMapper.ToJson(answer_data));
            FulFillment temp_fulfillment = new FulFillment();
            JsonUtility.FromJsonOverwrite(JsonMapper.ToJson(answer_data), temp_fulfillment);
            Match_Manager.instance.rounds[actual_round].fulfillments.Add(temp_fulfillment);
            if (temp.id_user == User_Manager.instance.current_user.id){
                temp.vote_btn.interactable = false;
            }
            news_voting_answer_instances.Add(temp);
        }
        yield return new WaitForSeconds(2.0f);
        StartCoroutine(Transition_Manager.instance.Change_Panel_Routine("14_News_Voting", Transitions.fade));
        answers_balloon_controller.Handle_Balloon(true, Match_Manager.instance.rounds[actual_round].incomplete_text, "O que?", "252C4A");
        StartCoroutine(Fill_Bar_Routine(news_voting_fill_bar, 20.0f));
        yield return new WaitForSeconds(20.0f);
        Process_Votes(-1);
    }
    public void Process_Votes(int user_id){
        answers_balloon_controller.Handle_Balloon(false);
        Hashtable hash = new Hashtable();
        Hashtable vote_hash = new Hashtable();
        vote_hash.Add("id_user", user_id);  
        vote_hash.Add("round", actual_round);
        hash.Add("match_data", Match_Manager.instance.current_match);
        hash.Add("vote_data", vote_hash);
        Sockets_Manager.instance.Send_User_Message("send_vote", hash);
        StartCoroutine(Transition_Manager.instance.Change_Panel_Routine("15_Votes_Processing", Transitions.fade));
    }
    public IEnumerator Voting_Processed_Routine(JsonData data){
        JsonUtility.FromJsonOverwrite(JsonMapper.ToJson(data["round_winner"]), Match_Manager.instance.rounds[actual_round].round_winner);
        yield return new WaitForSeconds(1.0f);
        car_box.SetActive(true);
        StartCoroutine(Transition_Manager.instance.Change_Panel_Routine("16_Votes_Winner", Transitions.fade));
        yield return new WaitForSeconds(1.0f);
        foreach (var item in Match_Manager.instance.match_users){ 
            if(Match_Manager.instance.rounds[actual_round].round_winner.id_user == item.id_user){
                string user_complete_news = (Match_Manager.instance.rounds[actual_round].incomplete_text).Replace("___", Match_Manager.instance.rounds[actual_round].round_winner.fulfilled_news);
                foreach(var match_user in Car_Movement_Controller.instance.players){ 
                    if(match_user.match_user == item){
                        yield return StartCoroutine(Car_Movement_Controller.instance.Move_Car_Routine(match_user, 1.0f));
                        break;
                    }
                }
                center_balloon_controller.Handle_Balloon(true, user_complete_news, "O quê?", item.color);
            }
        }

        StartCoroutine(Fill_Bar_Routine(votes_winner_fill_bar, 8.0f));
        yield return new WaitForSeconds(6.0f);
        center_balloon_controller.Handle_Balloon(false);
        yield return new WaitForSeconds(2.0f);
        yield return StartCoroutine(Car_Movement_Controller.instance.Move_Car_Routine(Car_Movement_Controller.instance.driver, 1.0f));
        center_balloon_controller.Handle_Balloon(true, Match_Manager.instance.rounds[actual_round].complete_text, "Na verdade eu disse...", "252C4A");
        StartCoroutine(Transition_Manager.instance.Change_Panel_Routine("17_News_Revelation", Transitions.fade));
        StartCoroutine(Fill_Bar_Routine(news_revelation_fill_bar, 8.0f));
        yield return new WaitForSeconds(6.0f);
        center_balloon_controller.Handle_Balloon(false);
        yield return new WaitForSeconds(2.0f);
        car_box.GetComponent<Animator>().Play("out_fade");
        Invoke("Disable_Car", 1.0f);
        Calculate_Points();
        Activate_Statistics_Player_Character();
        StartCoroutine(Transition_Manager.instance.Change_Panel_Routine("18_News_Results", Transitions.fade));
        yield return new WaitForSeconds(0.5f);
        foreach (var item in user_statistic_instances)
        {
            item.StartCoroutine(item.Update_Routine());
        }
        StartCoroutine(Fill_Bar_Routine(news_results_fill_bar, 10.0f));
        yield return new WaitForSeconds(9.5f);
        actual_round++;
        bool hasWinner = Get_Winner();
        if (!hasWinner){
            StartCoroutine(Round_Routine_1());
        }else{
            StartCoroutine(Transition_Manager.instance.Change_Panel_Routine("19_Game_Winner", Transitions.scroll));
            yield return new WaitForSeconds(5.0f);
            Unpopulate_Car_Characters();
            //CHANGE_LATER - NÃO FAZER RESET, MAS IR PARA O LOBBY
            SceneManager.LoadScene("Main");
        }
    }
    bool  Get_Winner(){
        Match_User winner = new Match_User();
        foreach (var match_user in Match_Manager.instance.match_users){
            if (match_user.points > winner.points) winner = match_user;
        }
        ChangeCharacterImg(game_winner_overlay, game_winner_character, game_winner_shadow, Match_Manager.instance.current_character.pic_overlay, Match_Manager.instance.current_character.pic, winner.color);
        if(winner.points >= 3){
            foreach (var item in user_statistic_instances)
            {
                if (item.current_user.id_user == winner.id_user)
                {
                    winner_banner.text = "O melhor\n~Personagem~\n" + item.username.text;
                }
            }
            return true;
        }else{
            return false;
        }
    }
    public void Activate_Statistics_Player_Character(){ 
        foreach(var item in user_statistic_instances){
            if (item.current_user.id_user == Match_Manager.instance.rounds[actual_round].round_winner.id_user){
                item.character_obj.SetActive(true);
                item.non_character_obj.SetActive(false);
            }
            else{
                item.character_obj.SetActive(false);
                item.non_character_obj.SetActive(true);
            }
        }
    }
    public void Calculate_Points(){
        foreach (var match_user in Match_Manager.instance.match_users)
        {
            match_user.points = 0;
        }
        foreach (var round in Match_Manager.instance.rounds){ 
            foreach(var match_user in Match_Manager.instance.match_users){ 
                if(round.round_winner.id_user == match_user.id_user){
                    match_user.points++;
                }
            }
        }
    }
}

[System.Serializable]
public enum Match_States{ 
    round_announcement,
    news_announcement,
    news_fulfillment,
    news_voting,
    news_processing,
    news_winner,
    news_result,
    news_final_result
}