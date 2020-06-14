using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Menu : MonoBehaviour
{
    //Buttons Method: Start
    public void StartGame(){
        SceneManager.LoadScene(0);
    }
}
