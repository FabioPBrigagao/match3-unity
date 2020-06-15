using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Menu : MonoBehaviour{

    public GameObject[] uiElements;

    /* Buttons Method: Start
                    */
    public void StartGame(){
        SceneManager.LoadScene(1);
    }
    /* Buttons Method: Instuctions
                        */
    public void Instuctions(){
        uiElements[0].SetActive(false);
        uiElements[1].SetActive(true);
    }

    /* Buttons Method: Menu
                        */
    public void ReturnMenu(){
        SceneManager.LoadScene(0);
    }

}
