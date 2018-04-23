using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeHiveScript : MonoBehaviour {

    private int strings;        //# of strings connecting the beehive to the branch
    public BranchScript branch;   //the branch the beehive is attached to

	// Use this for initialization
	void Start () {
        strings = 1;        //change to correct number when we know it
	}
	
	// Update is called once per frame
	void Update () {
		if (strings <= 0)
        {
            this.gameObject.transform.Translate(new Vector3(0, 1, 0));

            //if position is FIDN PLAYER'S POSITION then particle effect
        }
	}

    //remove a string holdign the beehive, separate from branch if nothing holding it
    public void SeperateFromBranch()
    {
        strings--;
    }
}
