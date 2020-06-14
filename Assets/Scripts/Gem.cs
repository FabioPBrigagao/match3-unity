using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour{

    Board board;
    SceneManager manager;

    public bool selected = false;
    private Dictionary<string, int> coordinate;

    void Awake(){
        board = GameObject.FindGameObjectWithTag("Board").GetComponent<Board>();
        manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<SceneManager>();
    }

    void OnMouseDown(){
        Select(); 
    }

    void Select(){
        //First gem selected
        if (!selected && !manager.GetIsSelected()){
            selected = true;
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
            manager.SetIsSelected(selected);
            manager.SetSelectedCoordinates(true, coordinate["x"], coordinate["y"]);
        //Second gem selected & make move if possible
        }else if (!selected && manager.GetIsSelected()){
            manager.SetSelectedCoordinates(false, coordinate["x"], coordinate["y"]);
            manager.CheckIfPossibleMove();
        //Deselect a selected gem
        }else if(selected){ 
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            selected = false;
            manager.SetIsSelected(selected);
        } 
    }


    public void ResetSelection(){
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        selected = false;
    }


    public void SetCoordinates(Dictionary<string,int> arg){
        coordinate = arg;
    }

}
