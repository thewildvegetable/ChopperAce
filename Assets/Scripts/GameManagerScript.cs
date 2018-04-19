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

    //track how many logs the player has cut
    //after 10, start spawning logs with 2 branches
    //after 20, start spawning strong logs
    //after 30, start spawning bee hives
    private int totalLogs;

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
    //indicator for score at gameOver
    public Text finalScoreText;
    //time last log was created
    private float lastScoreTime;

    //is the game over?
    private bool gameOver;

    //pause time counting while the tree is being moved
    private bool movingTree = false;

    //the logs
    private List<GameObject> logs;

    //log resources
    private Object[] logPrefabs;
    //panel for logs on screen
    public GameObject curTree;
    //panel for logs off screen
    public GameObject waitingTree;

    //panel for game over screen.
    public GameObject gameOverPanel;

    //strike through/shatter effect for a strong log being cut
    public GameObject shatter;

    //progress bar for current log score
    public Image progressBar;
    
	// Use this for initialization
	void Start () {
        //create logs
        logs = new List<GameObject>()
        {
            Capacity = LOG_CAPACITY
        };

        //load prefabs
        logPrefabs = Resources.LoadAll("Prefabs/Logs");
        initLogScore = BASE_SCORE;
        lastScoreTime = Time.time;

        gameOver = false;

        totalLogs = 0;

        Refill();
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

        progressBar.fillAmount = curLogScore / 100f;

        if(gameOver)
        {
            finalScoreText.text = "Final Score: " + player.Score.ToString();
            gameOverPanel.SetActive(true);
        }

	}

    //called to restart the game
    public void Restart()
    {
        //empty the logs
        logs.Clear();

        //load prefabs
        lastScoreTime = Time.time;

        //disable gameover
        gameOver = false;
        gameOverPanel.SetActive(false);

        //reset player score to 0
        player.IncreaseScore(-1 * player.Score);

        totalLogs = 0;

        Refill();
    }

    //called when player swings axe
    public void SwingAxe()
    {

        Debug.Log("Swing Axe");
        if (logs[0].GetComponent<LogScript>().strong)
        {
            logs[0].GetComponent<LogScript>().strong = false;

            //spawn strikethrough effect
            Instantiate(shatter, logs[0].transform.position, Quaternion.identity);
        }

        else if (!logs[0].GetComponent<LogScript>().strong)
        {
            //delete all shatter effects
            GameObject[] shatterEffect = GameObject.FindGameObjectsWithTag("Sprite");
            if (shatterEffect.Length > 0)
            {
                for (int i = 0; i < shatterEffect.Length; i++)
                {
                    Destroy(shatterEffect[i]);
                }
            }

            StartCoroutine(TakeBottom());

            if (logs[0].GetComponent<LogScript>().rotten)
            {
                Debug.Log("Swing Axe from rotten");
                SwingAxe();
            }

        }

    }

    //fill up tree and stored memory
    void Refill()
    {
        int initCount = logs.Count;

        for (int i = 0; i < logs.Capacity - initCount; i++)
        {
            bool logFine = true;

            //TODO: currently all are equally likely, should implement some kind of scaling later
            int PrefabVal = (int)Mathf.Ceil(Random.value * logPrefabs.Length - 1);

            GameObject newLog = (GameObject)Instantiate(logPrefabs[PrefabVal]);
            
            if (logs.Count > 0)
            {
                //validate beehive
                BeeHiveScript ifHive = newLog.GetComponentInChildren<BeeHiveScript>();
                if(ifHive != null)
                {
                    //logFine = false;

                    //dont' bother if we have less than 2 logs total
                    if (logs.Count < 2)
                    {
                        logFine = false;
                    }

                    else
                    {
                        //check each branch in the log that would be directly below our beehive
                        foreach (BranchScript branch in logs[logs.Count - 1].GetComponent<LogScript>().branches)
                        {
                            if (!branch.GetComponent<BranchScript>().rotten)
                            {
                                if (ifHive.branch.GetComponent<BranchScript>().rightSide != branch.GetComponent<BranchScript>().rightSide)
                                {
                                    logFine = false;
                                }
                            }
                        }

                        //if the log below our beehive is rotten, check the one below that too
                        if (logs[logs.Count - 1].GetComponent<LogScript>().rotten)
                        {
                            foreach (BranchScript branch in logs[logs.Count - 2].GetComponent<LogScript>().branches)
                            {
                                if (!branch.GetComponent<BranchScript>().rotten)
                                {
                                    if (ifHive.branch.GetComponent<BranchScript>().rightSide != branch.GetComponent<BranchScript>().rightSide)
                                    {
                                        logFine = false;
                                    }
                                }
                            }
                        }
                    }
                }

                //no more than 1 rotten log in a row
                if(newLog.GetComponent<LogScript>().rotten && logs[logs.Count-1].GetComponent<LogScript>().rotten)
                {
                    logFine = false;
                }
            }

            //if the new log passes validation
            if(!logFine)
            {
                i--;
                Destroy(newLog);
            }

            //decrement i to do the loop again
            else
            {
                //add new log
                logs.Add(newLog);
                logs[i + initCount].transform.SetParent(curTree.transform);
                //for the first bunch add to displayed panel
                if (i + initCount < SHOWN_LOGS)
                {
                    logs[i + initCount].transform.SetParent(curTree.transform);
                }

                //after append to waiting panel.
                else
                {
                    logs[i + initCount].transform.SetParent(waitingTree.transform);
                }

                //set local transform to preempt scaling issues
                logs[i + initCount].transform.localScale = Vector3.one;
            }
        }
    }

    //pop bottom log from tree
    private IEnumerator TakeBottom()
    {
        movingTree = true;

        //check for branches
        foreach (BranchScript branch in logs[0].GetComponent<LogScript>().branches)
        {
            //if branches are on the same side, kill
            if (branch.rightSide == player.GetComponent<PlayerScript>().rightSide && !branch.rotten)
            {
                gameOver = true;
            }
        }

        foreach(BranchScript branch in logs[1].GetComponent<LogScript>().branches)
        {
            BeeHiveScript ifHive = branch.hive;

            //check for hive, then validate for hive
            if (ifHive != null)
            {
                if (ifHive.branch.rightSide == player.GetComponent<PlayerScript>().rightSide)
                {
                    gameOver = true;
                }
            }
        }

        player.IncreaseScore(curLogScore);

        Debug.Log("Deleting bottom");
        Destroy(logs[0]);
        logs.RemoveAt(0);

        waitingTree.transform.GetChild(0).transform.SetParent(curTree.transform);

        //repeat if log is rotten
        if (!logs[0].GetComponent<LogScript>().rotten)
        {
            //if not rotten, reset scores and refill the queue
            if (logs[0].GetComponent<LogScript>().strong)
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

            player.chop = false;

            Refill();
        }

        totalLogs++;

        yield return new WaitForSeconds(0.001f);
    }
}