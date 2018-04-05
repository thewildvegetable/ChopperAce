using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeHiveScript : MonoBehaviour {

    private int strings;        //# of strings connecting the beehive to the branch
    public GameObject branch;   //the branch the beehive is attached to

	// Use this for initialization
	void Start () {
        strings = 5;        //change to correct number when we know it
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //remove a string holdign the beehive, separate from branch if nothing holding it
    public void SeperateFromBranch()
    {
        strings--;
        if (strings <= 0)
        {
            branch = null;

            //drop beehive here TODO
        }
    }
}
