using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleScore : MonoBehaviour {

    private int score;
    private string scoreName;

    public SingleScore (int val, string name)
    {
        score = val;
        scoreName = name;
    }

    public int GetScore()
    {
        return score;
    }

    public int GetScoreName()
    {
        return scoreName;
    }

}
