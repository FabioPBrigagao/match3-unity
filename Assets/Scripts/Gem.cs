using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour {

    Board board;
    GameManager manager;
    AudioManager audioManager;

    public bool selected = false;
    private Dictionary<string, int> coordinate;

    void Awake(){
        board = GameObject.FindGameObjectWithTag("Board").GetComponent<Board>();
        manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    /* 
    *   Instantiate the gem object
    */
    public static GameObject Start(GameObject gem, Vector2 position, int x, int y, float TILE_SIZE, Transform parent){
        return Instantiate(gem, new Vector2(position.x + (x * TILE_SIZE), position.y + (y * TILE_SIZE)), Quaternion.identity, parent);
    }

    void OnMouseDown(){
        //First gem selected
        if (!selected && !manager.isSelected){
            selected = true;
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
            manager.SetIsSelected(selected);
            manager.SetSelectedCoordinates(true, coordinate["x"], coordinate["y"]);
        //Second gem selected & make move if possible
        }else if (!selected && manager.isSelected){
            manager.SetSelectedCoordinates(false, coordinate["x"], coordinate["y"]);
            manager.CheckIfPossibleMove();
        //Deselect a selected gem
        }else if (selected){
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            selected = false;
            manager.SetIsSelected(selected);
        }
    }

    public void ResetSelection(){
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        selected = false;
    }

    public void SetCoordinates(Dictionary<string,int> dic){
        coordinate = dic;
    }

}
