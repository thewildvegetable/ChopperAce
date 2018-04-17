using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChopButtonScript : MonoBehaviour {

    public bool rightSide;      //what side the player goes to on click. false = left, true = right
    public GameObject player;
    public GameManagerScript gameManager;
    public GameObject node;     //side node for the player to be moved to

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnClick()
    {
        //set the player's side
        player.GetComponent<PlayerScript>().rightSide = this.rightSide;
        player.transform.SetParent(node.transform);
        player.transform.localPosition = Vector3.one;

        //change the player to attacking
        player.GetComponent<PlayerScript>().chop = true;

        //call gameManager's OnChop method
        gameManager.SwingAxe();
    }
}
