using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour {

    public bool rightSide;  //false = left side, true = right side
    private int score;
    public Text scoreText;
    public Sprite leftSprite;
    public Sprite leftAttackSprite;
    public Sprite rightSprite;
    public Sprite rightAttackSprite;
    public Image player;
    public bool chop;

    // Use this for initialization
    void Start () {
        score = 0;
        chop = false;
	}
	
	// Update is called once per frame
	void Update () {
        //print score ot the text field
        scoreText.text = score.ToString();

        if (rightSide && !chop)
        {
            player.sprite = rightSprite;
        }
        else if (rightSide)
        {
            player.sprite = rightAttackSprite;
        }
        else if (chop)
        {
            player.sprite = leftAttackSprite;
        }
        else
        {
            player.sprite = leftSprite;
        }
	}

    //increase the player's score after chopping the tree
    public void IncreaseScore(int points)
    {
        score += points;
    }
}
