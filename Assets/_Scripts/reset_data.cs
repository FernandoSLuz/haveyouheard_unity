using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class reset_data : MonoBehaviour
{
    public void clear_data(){
		PlayerPrefs.DeleteAll();
	}
}
