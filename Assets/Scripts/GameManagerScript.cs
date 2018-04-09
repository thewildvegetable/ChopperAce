using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour {
    //constants
    //how many logs the manager queues up
    public const int LOG_CAPACITY = 8;
    //how many logs are being displayed on the screen
    public const int SHOWN_LOGS = 5;
    //how long we pause to move logs down
    public const float PAUSE_DUR = 0.2f;
    //base score of a non-strong log
    public const int BASE_SCORE = 100;

    //the player
    public PlayerScript player;

    //score a log starts off as (separate from base score because strong logs should be worth more points)
    private int initLogScore;
    //score lost from time
    private int deltaScore;
    //initLogScore - deltaScore, the score added to the player
    private int curLogScore;
    //indicator for score of current log
    public Text logScoreText;
    //time last log was created
    private float lastScoreTime;

    //is the game over?
    private bool gameOver;

    //pause time counting while the tree is being moved
    private bool movingTree = false;

    //the logs
    private List<GameObject> Logs;

    //log resources
    private Object[] logPrefabs;
    //panel for logs on screen
    public GameObject curTree;
    //panel for logs off screen
    public GameObject waitingTree;

    //panel for game over screen.
    public GameObject gameOverPanel;

	// Use this for initialization
	void Start () {
        //create logs
        Logs = new List<GameObject>()
        {
            Capacity = LOG_CAPACITY
        };

        //load prefabs
        logPrefabs = Resources.LoadAll("Prefabs");
        initLogScore = BASE_SCORE;
        lastScoreTime = Time.time;

        gameOver = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (!movingTree && !gameOver)
        {
            float timeDif = Time.time - (lastScoreTime + 0.25f);

            if (timeDif > 0)
            {
                deltaScore = (int)Mathf.Floor(timeDif * 100);
                curLogScore = initLogScore - deltaScore;
            }

            if(curLogScore < 0)
            {
                curLogScore = 0;
            }
        }

        logScoreText.text = curLogScore.ToString();

        if(gameOver)
        {
            gameOverPanel.SetActive(true);
        }

	}

    //called when player swings axe
    public void SwingAxe()
    {
        StartCoroutine(TakeBottom());
    }

    //fill up tree and stored memory
    void Refill()
    {
        int initCount = Logs.Count;

        for (int i = 0; i < Logs.Capacity - initCount; i++)
        {
            bool logFine = true;

            //TODO: currently all are equally likely, should implement some kind of scaling later
            int PrefabVal = (int)Mathf.Ceil(Random.value * 18 - 1);

            GameObject newLog = (GameObject)Instantiate(logPrefabs[PrefabVal]);

            if(Logs.Count > 0)
            {
                //validate beehive
                BeeHiveScript ifHive = newLog.GetComponentInChildren<BeeHiveScript>();
                if(ifHive != null)
                {
                    //dont' bother if we have less than 2 logs total
                    if (Logs.Count < 2)
                    {
                        logFine = false;
                    }
                    
                    else
                    {
                        //check each branch in the log that would be directly below our beehive
                        foreach (GameObject branch in Logs[Logs.Count - 2].GetComponent<LogScript>().branches)
                        {
                            if (ifHive.branch.GetComponent<BranchScript>().rightSide == branch.GetComponent<BranchScript>().rightSide)
                            {
                                logFine = false;
                            }
                        }

                        //if the log below our beehive is rotten, check the one below that too
                        if (Logs[Logs.Count - 2].GetComponent<LogScript>().rotten || Logs[Logs.Count - 1].GetComponent<LogScript>().rotten)
                        {
                            foreach (GameObject branch in Logs[Logs.Count - 3].GetComponent<LogScript>().branches)
                            {
                                if (ifHive.branch.GetComponent<BranchScript>().rightSide == branch.GetComponent<BranchScript>().rightSide)
                                {
                                    logFine = false;
                                }
                            }
                        }
                    }
                }

                //no more than 1 rotten log in a row
                if(newLog.GetComponent<LogScript>().rotten && Logs[Logs.Count-1].GetComponent<LogScript>().rotten)
                {
                    logFine = false;
                }
            }

            //if the new log passes validation
            if(logFine)
            {
                //add new log
                Logs.Add(newLog);
                //for the first bunch add to displayed panel
                if(i + initCount < SHOWN_LOGS)
                {
                    Logs[i + initCount].transform.SetParent(curTree.transform);
                }

                //after append to waiting panel.
                else
                {
                    Logs[i + initCount].transform.SetParent(waitingTree.transform);
                }

                //set local transform to preempt scaling issues
                Logs[i + initCount].transform.localScale = Vector3.one;
            }

            //decrement i to do the loop again
            else
            {
                i--;
            }
        }
    }

    //pop bottom log from tree
    private IEnumerator TakeBottom()
    {
        movingTree = true;

        //check for branches
        foreach (GameObject branch in Logs[0].GetComponent<LogScript>().branches)
        {
            //if branches are on the same side, kill
            if (branch.GetComponent<BranchScript>().rightSide == player.GetComponent<PlayerScript>().rightSide && !branch.GetComponent<BranchScript>().rotten)
            {
                gameOver = true;
            }
        }

        BeeHiveScript ifHive = Logs[1].GetComponentInChildren<BeeHiveScript>();

        //check for hive, then validate for hive
        if (ifHive != null)
        {
            if (ifHive.branch.GetComponent<BranchScript>().rightSide == player.GetComponent<PlayerScript>().rightSide)
            {
                gameOver = true;
            }
        }

        player.IncreaseScore(curLogScore);
        
        Destroy(Logs[0]);
        Logs.RemoveAt(0);

        waitingTree.transform.GetChild(0).transform.SetParent(curTree.transform);

        yield return new WaitForSeconds(PAUSE_DUR);

        //repeat if log is rotten
        if(Logs[0].GetComponent<LogScript>().rotten)
        {
            StartCoroutine(TakeBottom());
        }
        //if not rotten, reset scores and refill the queue
        else
        {
            if (Logs[0].GetComponent<LogScript>().strong)
            {
                initLogScore = (int)Mathf.Round(BASE_SCORE * 1.5f);
            }
            else
            {
                initLogScore = BASE_SCORE;
            }

            lastScoreTime = Time.time;
            curLogScore = initLogScore;

            movingTree = false;

            Refill();
        }
    }

    
}