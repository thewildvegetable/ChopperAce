using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeHiveScript : MonoBehaviour {

    private int strings;        //# of strings connecting the beehive to the branch
    public BranchScript branch;   //the branch the beehive is attached to
    private bool ground;        //stop moving downwards once at the ground

	// Use this for initialization
	void Start () {
        strings = 1;        //change to correct number when we know it
        ground = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (strings <= 0 && !ground)
        {
            this.gameObject.transform.Translate(new Vector3(0, -0.5f, 0));

            //if position is FIDN PLAYER'S POSITION then particle effect
            if (this.gameObject.transform.position.y <= GameObject.FindGameObjectWithTag("Player").transform.position.y)
            {
                ground = true;
                ParticleManager.instance.generateParticles("bee", this.gameObject.transform);
            }
        }
	}

    //remove a string holdign the beehive, separate from branch if nothing holding it
    public void SeperateFromBranch()
    {
        strings--;
    }
}
