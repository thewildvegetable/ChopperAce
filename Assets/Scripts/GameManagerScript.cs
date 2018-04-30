using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour {
    //constants
    //how many logs the manager queues up
    public const int LOG_CAPACITY = 8;
    //how many logs are being displayed on the screen
    public const int SHOWN_LOGS = 5;
    //how long we pause to move logs down
    public const float PAUSE_DUR = 0.001f;
    //base score of a non-strong log
    public const int BASE_SCORE = 100;
    //how many logs are chopped before a difficulty increase
    public const int DELTA_DIFF = 20;

    public ChopButtonScript leftButton;
    public ChopButtonScript rightButton;

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

    //only calls end game method once
    private bool gameEnded;

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

    private int[] diffAProg = { 25, 38, -1, -1, 54, -1, 70, -1, 86, 101, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
    private int[] diffBProg = { 25, 36, -1, -1, 41, -1, 52, 63, 68, 78, 101, -1, -1, -1, -1, -1, -1, -1, -1 };
    private int[] diffCProg = { 15, 21, 29, -1, 37, -1, 49, 54, 61, 70, 76, 80, 84, -1, 88, -1, 92, 96, 101 };
    private int[] diffDProg = { 7, 12, 19, 22, 30, 33, 45, 50, 57, 66, 72, 76, 80, 82, 86, 88, 92, 96, 101 };
    
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

        gameEnded = false;

        totalLogs = 0;

        Refill();
	}
	
	// Update is called once per frame
	void Update () {
        if (!movingTree && !gameEnded)
        {
            float timeDif = Time.time - (lastScoreTime + 0.25f);
            progressBar.fillAmount = curLogScore / 100f;

            if (timeDif > 0)
            {
                deltaScore = (int)Mathf.Floor(timeDif * 100);
                curLogScore = initLogScore - deltaScore;
                if(curLogScore <= 0)
                {
                    SwingAxe();
                }
            }

            if(curLogScore < 0)
            {
                curLogScore = 0;
            }

            if(Input.GetKeyDown("left") || Input.GetKeyDown("a"))
            {
                leftButton.OnClick();
            }

            else if(Input.GetKeyDown("right") || Input.GetKeyDown("d"))
            {
                rightButton.OnClick();
            }
        }

        logScoreText.text = curLogScore.ToString();


	}

    //called to restart the game
    public void Restart()
    {
        //load prefabs
        lastScoreTime = Time.time;

        //disable gameover
        gameOverPanel.SetActive(false);
        finalScoreText.text = "";

        gameEnded = false;

        //reset player score to 0
        player.IncreaseScore(-1 * player.Score);


        GameObject[] shatterEffect = GameObject.FindGameObjectsWithTag("Sprite");
        if (shatterEffect.Length > 0)
        {
            for (int i = 0; i < shatterEffect.Length; i++)
            {
                Destroy(shatterEffect[i]);
            }
        }

        totalLogs = 0;

        while(logs.Count > 0)
        {
            Destroy(logs[0]);
            logs.RemoveAt(0);
        }

        totalLogs = 0;

        Refill();
    }

    //called when player swings axe
    public void SwingAxe()
    {
        //destroy every particle system
        GameObject[] particles = GameObject.FindGameObjectsWithTag("Particle");
        if (particles.Length > 0)
        {
            for (int i = particles.Length - 1; i >= 0; i--)
            {
                Destroy(particles[i]);
            }
        }

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
            //do log generation here, full-tree validation in this method
            GameObject newLog = GenLog();

            bool logFine = true;
            
            if (logs.Count > 0)
            {
                //validate beehive
                BeeHiveScript ifHive = newLog.GetComponentInChildren<BeeHiveScript>();
                if(ifHive != null)
                {
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
                            logFine = false;
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

    //generate a log, handle length validation
    private GameObject GenLog()
    {
        int r = (int)Mathf.Floor(Random.value * 100);

        GameObject returnVal = new GameObject();

        //0, 1, 4, 6, 8, 9
        if (totalLogs < DELTA_DIFF)
        {
            for (int i = 0; i < diffAProg.Length; i++)
            {
                if (r < diffAProg[i])
                {
                    Destroy(returnVal);
                    returnVal = (GameObject)Instantiate(logPrefabs[i]);
                    break;
                }
            }
        }

        //0, 1, 2, 4, 6, 7, 8, 9
        else if (totalLogs < DELTA_DIFF * 2)
        {
            for (int i = 0; i < diffBProg.Length; i++)
            {
                if (r < diffBProg[i])
                {
                    Destroy(returnVal);
                    returnVal = (GameObject)Instantiate(logPrefabs[i]);
                    break;
                }
            }
        }


        //0, 1, 2, 4, 6, 7, 8, 9, 10, 11, 12, 14, 16, 17, 18
        else if (totalLogs < DELTA_DIFF * 3)
        {
            for (int i = 0; i < diffCProg.Length; i++)
            {
                if (r < diffCProg[i])
                {
                    Destroy(returnVal);
                    returnVal = (GameObject)Instantiate(logPrefabs[i]);
                    break;
                }
            }
        }

        else
        {
            for (int i = 0; i < diffDProg.Length; i++)
            {
                if (r < diffDProg[i])
                {
                    Destroy(returnVal);
                    returnVal = (GameObject)Instantiate(logPrefabs[i]);
                    break;
                }
            }
        }

        return returnVal;
    }

    //pop bottom log from tree
    private IEnumerator TakeBottom()
    {
        movingTree = true;

        //spawn particle effect
        if (logs[0].GetComponent<LogScript>().rotten)
        {
            ParticleManager.instance.generateParticles("rotten", logs[0].transform);
        }
        else
        {
            ParticleManager.instance.generateParticles("regular", logs[0].transform);
        }

        //check for branches
        foreach (BranchScript branch in logs[0].GetComponent<LogScript>().branches)
        {
            //if branches are on the same side, kill
            if (branch.rightSide == player.GetComponent<PlayerScript>().rightSide && !branch.rotten)
            {
                EndGame();
            }
        }

        foreach(BranchScript branch in logs[1].GetComponent<LogScript>().branches)
        {
            BeeHiveScript ifHive = branch.hive;

            //check for hive, then validate for hive
            if (ifHive != null)
            {
                //make beehive fall
                ifHive.SeperateFromBranch();
                yield return new WaitForSeconds(0.5f);
                if (ifHive.branch.rightSide == player.GetComponent<PlayerScript>().rightSide)
                {
                    EndGame();
                }
            }
        }

        player.IncreaseScore(curLogScore);

        Debug.Log("Deleting bottom");
        Destroy(logs[0]);
        logs.RemoveAt(0);

        waitingTree.transform.GetChild(0).transform.SetParent(curTree.transform);

        yield return new WaitForSeconds(PAUSE_DUR);

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
    }

    private void EndGame()
    {
        gameEnded = true;

        finalScoreText.text += player.Score.ToString();
        gameOverPanel.SetActive(true);

        PlayerPrefs.SetInt("curHS", player.Score);
        PlayerPrefs.Save();
    }
}