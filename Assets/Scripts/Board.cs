using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board: MonoBehaviour {

    GameManager manager;

    public GameObject gem;
    public Sprite[] gemsSprites;

    private int[,] gridObj;
    private Vector3 initialPos;
    private bool reset = true;
    private GameObject[,] gems;
    private List<Dictionary<string, int>> matchCoordinates;
    public bool combo = false;
    private int comboCount = 1;

    private const int GRID_WIDTH = 5;
    private const int GRID_HEIGHT = 7;
    private const float TILE_SIZE = 8;
    private const float WAIT_TIME_MOVE = 0.5f;

    void Awake(){
        manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        initialPos = gameObject.transform.position;
        gridObj = new int[GRID_WIDTH, GRID_HEIGHT];
        gems = new GameObject[GRID_WIDTH,GRID_HEIGHT];
        matchCoordinates = new List<Dictionary<string, int>>();
    }

    void Start(){
        Setup(false);
    }

    // Create 2D randomize gem grid with no matches
    void Setup(bool shuffle){
        while(reset){
            for (int x = 0; x < gridObj.GetLength(0); x++){
                for (int y = 0; y < gridObj.GetLength(1); y++){
                    gridObj[x, y] = Random.Range(0, gemsSprites.GetLength(0));
                }
            }
            if(!CheckMatches(false) && CheckIfThereArePossibleMoves()){
                reset = false;
            }
        }
        for (int x = 0; x < gridObj.GetLength(0); x++){
            for (int y = 0; y < gridObj.GetLength(1); y++){
                if(!shuffle){
                    GameObject tempGemObj = Instantiate(gem, new Vector2(initialPos.x + (x * TILE_SIZE), initialPos.y + (y * TILE_SIZE)), Quaternion.identity, gameObject.transform);
                    gems[x, y] = tempGemObj;
                }
                gems[x, y].GetComponent<SpriteRenderer>().sprite = gemsSprites[gridObj[x, y]];
                Gem tempGem = gems[x, y].GetComponent<Gem>();
                tempGem.SetCoordinates(new Dictionary<string, int>() { { "x", x }, { "y", y } });
            }
        }
    }

    /* Check for any horizontal or vertical match
    * 
    * Return: true if found match; otherwise, false
    */
    bool CheckMatches(bool isPlayerMove){
        bool ifMatch = false;
        //Horizontal Check
        for (int x = 0; x < gridObj.GetLength(0) - 2; x++){
            for (int y = 0; y < gridObj.GetLength(1); y++){
                int current = gridObj[x, y];
                if (current == gridObj[x + 1, y] && current == gridObj[x + 2, y]){
                    if (isPlayerMove){
                        SaveMatchCoordinates(x,y, true);
                    }
                    ifMatch = true;
                }
            }
        }
        //Vertical check
        for (int x = 0; x < gridObj.GetLength(0); x++){
            for (int y = 0; y < gridObj.GetLength(1) - 2; y++){
                int current = gridObj[x, y];
                if (current == gridObj[x, y + 1] && current == gridObj[x, y + 2]){
                    if (isPlayerMove){
                        SaveMatchCoordinates(x,y, false);
                    }
                    ifMatch = true;
                }
            }
        }
        return ifMatch;
    }


    public void MakeSpriteSwitch(int x_1, int y_1, int x_2, int y_2){
        Sprite spriteTemp_1 = gems[x_1, y_1].GetComponent<SpriteRenderer>().sprite;
        Sprite spriteTemp_2 = gems[x_2, y_2].GetComponent<SpriteRenderer>().sprite;
        gems[x_1, y_1].GetComponent<SpriteRenderer>().sprite = spriteTemp_2;
        gems[x_2, y_2].GetComponent<SpriteRenderer>().sprite = spriteTemp_1;
        gems[x_1, y_1].GetComponent<Gem>().ResetSelection();
        StartCoroutine(TriggerMatch());
    }

    public bool CheckIfSwitchIsPossible(int x_1, int y_1, int x_2, int y_2, bool isPlayerMove){
        int temp_1 = gridObj[x_1, y_1];
        int temp_2 = gridObj[x_2, y_2];
        gridObj[x_1, y_1] = temp_2;
        gridObj[x_2, y_2] = temp_1;
        if (CheckMatches(isPlayerMove)){
            if(isPlayerMove){
                return true;
            }else{
                gridObj[x_1, y_1] = temp_1;
                gridObj[x_2, y_2] = temp_2;
                return true;
            }
        }else{
            gridObj[x_1, y_1] = temp_1;
            gridObj[x_2, y_2] = temp_2;
        }
        return false;
    }


    void SaveMatchCoordinates(int x, int y, bool isHorizontal){
        matchCoordinates.Add(new Dictionary<string, int>() { { "x", x }, { "y", y } });
        if (isHorizontal){
            for (int i = 1; i < (GRID_WIDTH - x); i++){
                if (gridObj[x, y] == gridObj[x + i, y]){
                    matchCoordinates.Add(new Dictionary<string, int>() { { "x", x + i }, { "y", y } });
                }else{
                    break;
                }
            }
        }
        else{
            for (int i = 1; i < (GRID_HEIGHT - y); i++){
                if (gridObj[x, y] == gridObj[x, y + i]){
                    matchCoordinates.Add(new Dictionary<string, int>() { { "x", x }, { "y", y + i } });
                }else{
                    break;
                }
            }
        }
    }


    IEnumerator TriggerMatch(){
        foreach (Dictionary<string, int> unit in matchCoordinates){
            gems[unit["x"], unit["y"]].GetComponent<SpriteRenderer>().sprite = null;
        }
        manager.AddScore(matchCoordinates.Count, comboCount);
        matchCoordinates.Clear();
        yield return new WaitForSeconds(WAIT_TIME_MOVE);
        MoveGemsDownwards();
        yield return new WaitForSeconds(WAIT_TIME_MOVE);
        combo = CheckMatches(true); //Check if there was more than one match in a single move
        if(combo){
            comboCount += 1;
            Debug.Log("combo");
            StartCoroutine(TriggerMatch());
        }else{
            CheckIfThereArePossibleMoves();
            manager.SetIsSwitching(false);
            combo = false;
            comboCount = 1;
        }
    }

    void MoveGemsDownwards(){
        bool hasNull = true;
        while(hasNull){
            hasNull = false;
            for (int y = 0; y < GRID_HEIGHT; y++){
                for (int x = 0; x < GRID_WIDTH; x++){
                    if(gems[x, y].GetComponent<SpriteRenderer>().sprite == null){
                        hasNull = true;
                        if (y != GRID_HEIGHT - 1){
                            gridObj[x, y] = gridObj[x, y + 1];
                            gridObj[x, y + 1] = 7;
                            gems[x, y].GetComponent<SpriteRenderer>().sprite = gems[x, y + 1].GetComponent<SpriteRenderer>().sprite;
                            gems[x, y + 1].GetComponent<SpriteRenderer>().sprite = null;

                        } else{
                            gridObj[x, y] = Random.Range(0, gemsSprites.GetLength(0));
                            gems[x, y].GetComponent<SpriteRenderer>().sprite = gemsSprites[gridObj[x, y]];
                        }
                    }
                }
            }
        }
    }


    bool CheckIfThereArePossibleMoves(){
        bool ifPossibleMove = false;
        for (int x = 0; x < GRID_WIDTH; x++){
            for (int y = 0; y < GRID_HEIGHT; y++){
                try{
                    ifPossibleMove =  CheckIfSwitchIsPossible(x, y, x, y + 1, false); //UP
                    if(ifPossibleMove){
                        Debug.Log("x:" + x + " y: " + y);
                        break;
                    }
                }catch{ }
                try{
                    ifPossibleMove = CheckIfSwitchIsPossible(x, y, x, y - 1, false); //DOWN
                    if (ifPossibleMove){
                        Debug.Log("x:" + x + " y: " + y);
                        break;
                    }
                }catch { }
                try{
                    ifPossibleMove = CheckIfSwitchIsPossible(x, y, x + 1, y, false); //RIGHT
                    if (ifPossibleMove){
                        Debug.Log("x:" + x + " y: " + y);
                        break;
                    }
                }catch { }
                try{
                    ifPossibleMove = CheckIfSwitchIsPossible(x, y, x - 1, y, false); //LEFT
                    if (ifPossibleMove){
                        Debug.Log("x:" + x + " y: " + y);
                        break;
                    }
                }catch { }
            }
            if (ifPossibleMove){
                break;
            }
        }
        if (!ifPossibleMove){
            Debug.Log("No Possible Move");
            reset = true;
            Setup(true);
        }
        return ifPossibleMove;
    }





    bool CheckIfThereArePossibleMovesOLD(){
        bool ifPossibleMove = false;
        for (int x = 0; x < GRID_WIDTH; x++){
            for (int y = 0; y < GRID_HEIGHT; y++){
                if(y == 0 && x == 0)
                {
                    if (CheckIfSwitchIsPossible(x, y, x + 1, y, false) || //RIGHT
                        CheckIfSwitchIsPossible(x, y, x, y + 1, false)){  //UP
                        Debug.Log("x:" + x + " y: " + y);
                        ifPossibleMove = true;
                        break;
                    }
                }
                else if (y == (GRID_HEIGHT - 1) && x == (GRID_WIDTH - 1))
                {
                    if (CheckIfSwitchIsPossible(x, y, x, y - 1, false) || //DOWN
                        CheckIfSwitchIsPossible(x, y, x - 1, y, false)){  //LEFT
                        Debug.Log("x:" + x + " y: " + y);
                        ifPossibleMove = true;
                        break;
                    }
                }
                else if (y == 0 && x == (GRID_WIDTH - 1))
                {
                    if (CheckIfSwitchIsPossible(x, y, x, y + 1, false) || //UP
                        CheckIfSwitchIsPossible(x, y, x - 1, y, false)){  //LEFT
                        Debug.Log("x:" + x + " y: " + y);
                        ifPossibleMove = true;
                        break;
                    }
                }
                else if (y == (GRID_HEIGHT - 1) && x == 0)
                {
                    if (CheckIfSwitchIsPossible(x, y, x, y - 1, false) || //DOWN
                        CheckIfSwitchIsPossible(x, y, x + 1, y, false)){  //RIGHT
                        Debug.Log("x:" + x + " y: " + y);
                        ifPossibleMove = true;
                        break;
                    }
                }
                else if(y == 0)
                {
                    if (CheckIfSwitchIsPossible(x, y, x, y + 1, false) || //UP
                        CheckIfSwitchIsPossible(x, y, x + 1, y, false) || //RIGHT
                        CheckIfSwitchIsPossible(x, y, x - 1, y, false)){  //LEFT
                        Debug.Log("x:" + x + " y: " + y);
                        ifPossibleMove = true;
                        break;
                    }
                }
                else if (x == 0)
                {
                    if (CheckIfSwitchIsPossible(x, y, x, y + 1, false) || //UP
                        CheckIfSwitchIsPossible(x, y, x + 1, y, false) || //RIGHT
                        CheckIfSwitchIsPossible(x, y, x, y - 1, false)){  //DOWN
                        Debug.Log("x:" + x + " y: " + y);
                        ifPossibleMove = true;
                        break;
                    }
                }
                else if (y == (GRID_HEIGHT - 1))
                {
                    if (CheckIfSwitchIsPossible(x, y, x - 1, y, false) || //LEFT
                        CheckIfSwitchIsPossible(x, y, x + 1, y, false) || //RIGHT
                        CheckIfSwitchIsPossible(x, y, x, y - 1, false)){  //DOWN
                        Debug.Log("x:" + x + " y: " + y);
                        ifPossibleMove = true;
                        break;
                    }
                }
                else if (x == (GRID_WIDTH - 1))
                {
                    if (CheckIfSwitchIsPossible(x, y, x - 1, y, false) || //LEFT
                        CheckIfSwitchIsPossible(x, y, x, y + 1, false) || //UP
                        CheckIfSwitchIsPossible(x, y, x, y - 1, false)){  //DOWN
                        Debug.Log("x:" + x + " y: " + y);
                        ifPossibleMove = true;
                        break;
                    }
                }
                else{
                    if (CheckIfSwitchIsPossible(x, y, x, y + 1, false) || //UP
                        CheckIfSwitchIsPossible(x, y, x + 1, y, false) || //RIGHT
                        CheckIfSwitchIsPossible(x, y, x - 1, y, false) || //LEFT
                        CheckIfSwitchIsPossible(x, y, x, y - 1, false)){  //DOWN
                        Debug.Log("x:" + x + " y: " + y);
                        ifPossibleMove = true;
                        break;
                    }
                }
            }
            if(ifPossibleMove){
                break;
            }
        }
        if(!ifPossibleMove){
            Debug.Log("No Possible Move");
        }
        return ifPossibleMove;
    }


    public void DeactivateBoard(){
        gameObject.SetActive(false);
    }

}
