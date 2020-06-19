using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Menu : MonoBehaviour{

    public GameObject[] menuUI;
    public GameObject[] instructionsUI;

    public void ButtonStartGame(){
        SceneManager.LoadScene(1);
    }

    public void ButtonInstuctions(){
        foreach (var item in menuUI) item.SetActive(false);
        foreach (var item in instructionsUI) item.SetActive(true);
    }

    public void ButtonReturnMenu(){
        SceneManager.LoadScene(0);
    }

}
