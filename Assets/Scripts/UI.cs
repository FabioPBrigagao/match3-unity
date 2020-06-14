using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour{

    GameManager manager;

    public Text[] roundText, scoreText, timerText, totalScoreText, finalRoundText;

    void Awake(){
        manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    void Update(){
        scoreText[0].text = manager.score + " / " + manager.scoreRoundGoal;
        scoreText[1].text = manager.score + " / " + manager.scoreRoundGoal;

        roundText[0].text = "Round #" + manager.round;
        roundText[1].text = "Round #" + manager.round;

        timerText[0].text = Mathf.Round(manager.timer) + "";
        timerText[1].text = Mathf.Round(manager.timer) + "";


    }


    public IEnumerator DisplayFade(GameObject obj, int scoreAdd, float time){
        if (scoreAdd > 0){
            obj.GetComponent<Text>().text = "+ " + scoreAdd;
        }
        obj.SetActive(true);
        Color originalcolor = obj.GetComponent<Text>().color;
        for (float t = 1.0f; t >= 0.0f; t -= Time.deltaTime / time){
            Color newColor = new Color(originalcolor.r, originalcolor.g, originalcolor.b, t);
            obj.GetComponent<Text>().color = newColor;
            yield return null;
        }
        obj.SetActive(false);
    }


    public void DisplayGameOverText(){
        totalScoreText[0].text = manager.totalScore + "";
        totalScoreText[1].text = manager.totalScore + "";

        finalRoundText[0].text = manager.round + "";
        finalRoundText[1].text = manager.round + "";
    }

}
