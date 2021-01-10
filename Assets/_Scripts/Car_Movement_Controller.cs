using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HaveYouHeard
{
    public class Car_Movement_Controller : MonoBehaviour
    {
        public static Car_Movement_Controller instance;
        public Car_Character_instance driver;
        public List<Car_Character_instance> players = new List<Car_Character_instance>();
        public RectTransform car;

        private void Awake()
        {
            if(instance == null){
                instance = this;
            }
        }
        public void Clear_Players(){
            players.Clear();
        }

        public void ResetPosition()
        {
            car.anchoredPosition = new Vector2(0, car.anchoredPosition.y);
        }
        public IEnumerator Move_Car_Routine(Car_Character_instance target_instance, float transition_duration){
            float timer = 0.0f;
            float origin_pos = car.anchoredPosition.x;
            float target_pos = target_instance.origin_pos.x*(-1) - 20.0f;
            while (timer < 1.0f){
                timer += Time.deltaTime / transition_duration;
                float actual_pos = Mathf.Lerp(origin_pos, target_pos, timer);
                Debug.Log(actual_pos);
                car.anchoredPosition = new Vector2(actual_pos, car.anchoredPosition.y);
                yield return null;
            }
        }
    }
}

