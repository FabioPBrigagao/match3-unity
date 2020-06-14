using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour{

    Board board;

    private bool isSelected = false;

    public int selectedXCor_1, selectedYCor_1, selectedXCor_2, selectedYCor_2;

    void Awake(){
        board = GameObject.FindGameObjectWithTag("Board").GetComponent<Board>();
    }

    public void CheckIfPossibleMove(){
        if ((selectedXCor_1 + 1 == selectedXCor_2 && selectedYCor_1 == selectedYCor_2) || //Right
           (selectedXCor_1 - 1 == selectedXCor_2 && selectedYCor_1 == selectedYCor_2) || //Left
           (selectedXCor_1 == selectedXCor_2 && selectedYCor_1 + 1 == selectedYCor_2) || //Up
           (selectedXCor_1 == selectedXCor_2 && selectedYCor_1 - 1 == selectedYCor_2)){  //Down
            if(board.CheckIfSwitchIsPossible(selectedXCor_1, selectedYCor_1, selectedXCor_2, selectedYCor_2,true)){
                board.MakeSpriteSwitch(selectedXCor_1, selectedYCor_1, selectedXCor_2, selectedYCor_2);
                ResetCoordinates();
                isSelected = false;
            }
        }
    }

    public bool GetIsSelected(){
        return isSelected;
    }

    public void SetIsSelected(bool status){
        isSelected = status;
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

}

