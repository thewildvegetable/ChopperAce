using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreManager : MonoBehaviour {

    //gameObjects on screen
    public Button submit;
    public InputField sessionName;
    public Text textScores;

    public List<SingleScore> scores;
    public List<SingleScore> sortedScores;

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
                    scores.Add(new SingleScore(newHS, newName));
                    validSlot = true;
                    PlayerPrefs.SetInt("score" + iterator.ToString(), newHS);
                    PlayerPrefs.SetString("name" + iterator.ToString(), newName);
                    sessionScored = true;
                    PlayerPrefs.Save();
                }

                else
                {
                    scores.Add(new SingleScore(PlayerPrefs.GetInt("score" + iterator.ToString()), PlayerPrefs.GetString("name" + iterator.ToString())));
                    iterator++;
                }
            }

            SortScores();
            ScoreBoard();
        }
    }

    private void SortScores()
    {
        int initCount = scores.Count;

        for (int i = 0; i < initCount; i++)
        {
            int highInd = 0;

            for (int c = 0; c < scores.Count; c++)
            {
                if (scores[c].GetScore() > scores[highInd].GetScore())
                {
                    highInd = c;
                }
            }

            sortedScores.Add(scores[highInd]);
            scores.RemoveAt(highInd);
        }
    }

    //loads playerPrefs to fill the scoreboard
    private void ScoreBoard()
    {
        //loop through playerprefs, adding a new line for each pref stored, until you run out of new ones.
        foreach(SingleScore score in sortedScores)
        {
            string newLine = "";

            newLine += score.GetScoreName();
            newLine += ": ";
            newLine += score.GetScore();
            newLine += "\n";

            textScores.text += newLine;
            
        }
    }

}
