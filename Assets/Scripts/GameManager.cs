using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour {

    Board board;
    UI ui;
    AudioManager audioManager;
    public GameObject[] gameplayUIElements, gameOverUIElements;
    public GameObject additionUI, shuffleUI, nextRoundUI;
    [HideInInspector] public int score, totalScore, round, scoreRoundGoal;
    [HideInInspector] public float timer;
    [HideInInspector] public bool isSelected;
    private int selectedXCor_1, selectedYCor_1, selectedXCor_2, selectedYCor_2;
    private bool isSwitching = false;
    private const int ROUND_GOAL_INCREASE = 35;
    private const int RESET_TIMER = 120;
    private const int ROUND_ONE_GOAL = 10;

    void Awake() => Setup();
    void Update() => Timer();

    void Setup(){
        //Initial conditions
        round = 1;
        timer = RESET_TIMER;
        scoreRoundGoal = ROUND_ONE_GOAL;
        isSelected = false;

        //Reference objects
        board = GameObject.FindGameObjectWithTag("Board").GetComponent<Board>();
        ui = GameObject.FindGameObjectWithTag("UI").GetComponent<UI>();
        audioManager = GameObject.FindGameObjectWithTag("Audio Manager").GetComponent<AudioManager>();
    }

    /*
    *  Handles timer
    *  When timer reaches 0, the game is over
    */
    void Timer(){
        if (timer <= 0){
            Invoke("GameOver", 0);
        }else{
            timer -= Time.deltaTime;
        }
    }

    /*
    *  Called by a gem object to apply a move if possible
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

    /*
    *  Check if selected gems are a legal movement. 
    *  A swap can only happen up, down, right and left 
    *  No diagonal swap
    */
    bool PerpendicularMove(){
        if ((selectedXCor_1 + 1 == selectedXCor_2 && selectedYCor_1 == selectedYCor_2) ||           //Right
                   (selectedXCor_1 - 1 == selectedXCor_2 && selectedYCor_1 == selectedYCor_2) ||    //Left
                   (selectedXCor_1 == selectedXCor_2 && selectedYCor_1 + 1 == selectedYCor_2) ||    //Up
                   (selectedXCor_1 == selectedXCor_2 && selectedYCor_1 - 1 == selectedYCor_2)){     //Down
            return true;
        }else{
            return false;
        }
    }

    /*
    *  Calculate score
    *  Call UI effects 
    *  Manages round transition
    */
    public void AddScore(int sequenceCount, int combo = 1){
        score += (sequenceCount * combo);
        totalScore += (sequenceCount * combo);
        ui.AdditionPopUp(additionUI, 1.0f, (sequenceCount * combo));
        if (score >= scoreRoundGoal){
            StartCoroutine(ui.PopUpFadeAway(nextRoundUI, 3.0f));
            scoreRoundGoal += ROUND_GOAL_INCREASE;
            round += 1;
            score = 0;
            timer = RESET_TIMER;
        }
    }

    public void SetIsSelected(bool status){
        audioManager.Play("Select");
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

    void GameOver(){
        board.DeactivateBoard();
        ui.DisplayGameOverText();
        foreach (var item in gameplayUIElements) item.SetActive(false);
        foreach (var item in gameOverUIElements) item.SetActive(true);
    }

    public void ButtonRestartGame(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ButtonReturnMenu(){
        SceneManager.LoadScene(0);
    }
}

