﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChopButtonScript : MonoBehaviour {

    public bool rightSide;      //what side the player goes to on click. false = left, true = right
    public GameObject player;

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

        //call gameManager's OnChop method TODO
    }
}
