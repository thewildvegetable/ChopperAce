using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreManager : MonoBehaviour {

    //gameObjects on screen
    public Button submit;
    public InputField sessionName;
    public Text textScores;

    //the new score from the current session
    private int newHS;

    //can only submit a new high score once per page load
    private bool sessionScored;

	// Use this for initialization
	void Start () {
        newHS = PlayerPrefs.GetInt("curHS");

        sessionScored = false;

        ScoreBoard();
	}
	
    //submit a new score
	public void SubmitButton()
    {
        if(!sessionScored)
        {
            string newName = sessionName.text;

            int iterator = 0;
            bool validSlot = false;

            //loop through the comparable playerPref names, until you find one that's unused.
            while(!validSlot)
            {
                if(PlayerPrefs.GetInt("score" + iterator.ToString(), -1) == -1)
                {
                    validSlot = true;
                    PlayerPrefs.SetInt("score" + iterator.ToString(), newHS);
                    PlayerPrefs.SetString("name" + iterator.ToString(), newName);
                    sessionScored = true;
                    PlayerPrefs.Save();
                }

                else
                {
                    iterator++;
                }
            }

            ScoreBoard();
        }
    }

    //loads playerPrefs to fill the scoreboard
    private void ScoreBoard()
    {
        int iterator = 0;
        bool moreScores = true;

        //loop through playerprefs, adding a new line for each pref stored, until you run out of new ones.
        while(moreScores)
        {
            string newLine = "";

            if(PlayerPrefs.GetInt("score" + iterator.ToString(), -1) != -1)
            {
                newLine += PlayerPrefs.GetString("name" + iterator.ToString());
                newLine += ": ";
                newLine += PlayerPrefs.GetInt("score" + iterator.ToString());
                newLine += "\n";

                textScores.text += newLine;
                iterator++;
            }

            else
            {
                moreScores = false;
            }
        }
    }

}
