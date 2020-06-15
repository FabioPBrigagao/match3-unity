using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour {

    Board board;
    UI ui;
    AudioManager audioManager;

    public GameObject[] uiElements;

    [HideInInspector] public int score = 0;
    [HideInInspector] public int totalScore;
    [HideInInspector] public int round = 1;
    [HideInInspector] public int scoreRoundGoal = 10;
    [HideInInspector] public float timer = 120f;
    [HideInInspector] public bool isSelected = false;

    private int selectedXCor_1, selectedYCor_1, selectedXCor_2, selectedYCor_2;
    private bool isSwitching = false;

    void Awake(){
        board = GameObject.FindGameObjectWithTag("Board").GetComponent<Board>();
        ui = GameObject.FindGameObjectWithTag("UI").GetComponent<UI>();
        audioManager = GameObject.FindGameObjectWithTag("Audio Manager").GetComponent<AudioManager>();
    }

    void Update(){     
        if(timer <= 0){ 
            Invoke("GameOver",0);
        }else{
            timer -= Time.deltaTime;
        }
    }

    /* Called by a gem object to apply a move if possible
                */
    public void CheckIfPossibleMove(){
        if (PerpendicularMove() && !isSwitching){ 
            if(board.CheckIfSwitchIsPossible(selectedXCor_1, selectedYCor_1, selectedXCor_2, selectedYCor_2,true)){
                audioManager.Play("Swap");
                board.MakeSpriteSwitch(selectedXCor_1, selectedYCor_1, selectedXCor_2, selectedYCor_2);
                isSwitching = true;
                ResetCoordinates();
                isSelected = false;
            }
        }
    }

    /* Check if selected gems are a legal movement. 
                * A swap can only happen up, down, right and left 
                * No diagonal swap
                */
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

    /* Gem object updates game manager if it has been selected or deselected
                */
    public void SetIsSelected(bool status){
        audioManager.Play("Select");
        isSelected = status;
    }

    /* Keeps track if a switch is happening. 
                * No movements are allowed during a swap
                */
    public void SetIsSwitching(bool status){
        isSwitching = status;
    }

    /* Sets first and second selected coordinates
                */
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

    /* Calculate score
                * call UI effects 
                * manages rounds
                 */
    public void AddScore(int sequenceCount, int combo = 1){
        score += (sequenceCount * combo);
        totalScore += (sequenceCount * combo);
        StartCoroutine(ui.DisplayFade(uiElements[1], (sequenceCount * combo), 2.0f));
        if(score >= scoreRoundGoal){
            StartCoroutine(ui.DisplayFade(uiElements[0],0, 3.0f));
            scoreRoundGoal *= 2;
            round += 1;
            score = 0;
            timer = 120;
        }
    }
    /* Set game over screen
                     */
    void GameOver(){
        board.DeactivateBoard();
        ui.DisplayGameOverText();
        uiElements[3].SetActive(false);
        uiElements[2].SetActive(true);
    }

    /* Buttons Method: Restart
                        */
    public void RestartGame(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /* Buttons Method: Menu
                        */
    public void ReturnMenu(){
        SceneManager.LoadScene(0);
    }

}

