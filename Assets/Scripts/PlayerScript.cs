using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour {

    public bool rightSide;  //false = left side, true = right side
    private int score;
    public Text scoreText;

    // Use this for initialization
    void Start () {
        score = 0;
	}
	
	// Update is called once per frame
	void Update () {
        //print score ot the text field
        scoreText.text = score.ToString();
	}

    //increase the player's score after chopping the tree
    public void IncreaseScore(int points)
    {
        score += points;
    }
}
