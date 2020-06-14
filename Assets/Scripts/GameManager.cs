using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour {

    Board board;
    UI ui;

    [HideInInspector] public int score = 0;
    [HideInInspector] public int totalScore;
    [HideInInspector] public int round = 1;
    [HideInInspector] public int scoreRoundGoal = 10;
    public float timer = 120f;

    private bool isSelected = false;
    private bool isSwitching = false;

    public GameObject[] uiDisplay;

    private int selectedXCor_1, selectedYCor_1, selectedXCor_2, selectedYCor_2;

    void Awake(){
        board = GameObject.FindGameObjectWithTag("Board").GetComponent<Board>();
        ui = GameObject.FindGameObjectWithTag("UI").GetComponent<UI>();
    }

    void Update(){     
        if(timer <= 0){
            Invoke("GameOver",0);
        }else{
            timer -= Time.deltaTime;
        }
    }


    public void CheckIfPossibleMove(){
        if (PerpendicularMove() && !isSwitching){ 
            if(board.CheckIfSwitchIsPossible(selectedXCor_1, selectedYCor_1, selectedXCor_2, selectedYCor_2,true)){
                board.MakeSpriteSwitch(selectedXCor_1, selectedYCor_1, selectedXCor_2, selectedYCor_2);
                isSwitching = true;
                ResetCoordinates();
                isSelected = false;
            }
        }
    }

    bool PerpendicularMove(){
        if ((selectedXCor_1 + 1 == selectedXCor_2 && selectedYCor_1 == selectedYCor_2) || //Right
                   (selectedXCor_1 - 1 == selectedXCor_2 && selectedYCor_1 == selectedYCor_2) || //Left
                   (selectedXCor_1 == selectedXCor_2 && selectedYCor_1 + 1 == selectedYCor_2) || //Up
                   (selectedXCor_1 == selectedXCor_2 && selectedYCor_1 - 1 == selectedYCor_2)){ //Down
            return true;
        }else{
            return false;
        }
    }

    public bool GetIsSelected(){
        return isSelected;
    }

    public void SetIsSelected(bool status){
        isSelected = status;
    }
    public void SetIsSwitching(bool status){
        isSwitching = status;
    }

    public void SetSelectedCoordinates(bool firstSelection, int x, int y){
        if(firstSelection){
            selectedXCor_1 = x;
            selectedYCor_1 = y;
        }else{
            selectedXCor_2 = x;
            selectedYCor_2 = y;
        }
    }
    void ResetCoordinates(){
        selectedXCor_1 = 0;
        selectedYCor_1 = 0;
        selectedXCor_2 = 0;
        selectedYCor_2 = 0;
    }


    public void AddScore(int sequenceCount, int combo = 1){
        score += (sequenceCount * combo);
        totalScore += (sequenceCount * combo);
        StartCoroutine(ui.DisplayFade(uiDisplay[1], (sequenceCount * combo), 2.0f));
        if(score >= scoreRoundGoal){
            StartCoroutine(ui.DisplayFade(uiDisplay[0],0, 3.0f));
            scoreRoundGoal *= 2;
            round += 1;
            score = 0;
            timer = 120;
        }
    }

    void GameOver(){
        board.DeactivateBoard();
        ui.DisplayGameOverText();
        uiDisplay[3].SetActive(false);
        uiDisplay[2].SetActive(true);
    }

    //Buttons Method: Restart
    public void RestartGame(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    //Buttons Method: Restart
    public void ReturnMenu(){
        SceneManager.LoadScene(1);
    }

}

