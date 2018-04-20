using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class TransferScene : MonoBehaviour {

    public enum ToScene { MainMenu,Game,Credits,Options, HiScore};
    public ToScene sceneTarget;
    private string targetString;


	// Use this for initialization
	void Start () {
        switch (sceneTarget)
        {
            case ToScene.MainMenu:
                {
                    targetString = "MainMenu";
                    break;
                }
            case ToScene.Game:
                {
                    targetString = "Game";
                    break;
                }
            case ToScene.Credits:
                {
                    targetString = "Credits";
                    break;
                }
            case ToScene.Options:
                {
                    targetString = "Options";
                    break;
                }
            case ToScene.HiScore:
                {
                    targetString = "HighScore";
                    break;
                }

        }
	}
	
	public void OnClick()
    {
        SceneManager.LoadScene(targetString);
    }
}
