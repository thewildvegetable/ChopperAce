using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BranchScript : MonoBehaviour {

    public bool rotten;
    public bool rightSide;  //false = left side, true = right side
    public BeeHiveScript hive;

	// Use this for initialization
	void Start () {
        if (!rightSide)
        {
            this.gameObject.transform.localScale += new Vector3(-2 * this.gameObject.transform.localScale.x, 0, 0);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
