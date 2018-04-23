using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SatIntroSceneManager : MonoBehaviour {

    private int sceneTimeStep;
    public GameObject infoDialog;
    public GameObject childModel;

    private static int NUM_TEXT_FIRST_STAGE = 2;

	void Start () {
        sceneTimeStep = 0;
	}

    void OpenStartPageScene()
    {
        sceneTimeStep = 0;
        Application.LoadLevel("StartPage");
    }

    void AdvanceScene() {
        if (sceneTimeStep == NUM_TEXT_FIRST_STAGE)
        {
            OpenStartPageScene();
        }

        infoDialog.SendMessage("AdvanceText");

        sceneTimeStep++;

    }
}
