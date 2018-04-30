using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SatIntroSceneManager : MonoBehaviour {

    private int sceneTimeStep;
    public GameObject infoDialog;
    public GameObject infoDialog2;
    public GameObject childModel;

	void Start () {
        sceneTimeStep = 0;
        childModel.SetActive(false);
	}

    void OpenStartPageScene()
    {
        sceneTimeStep = 0;
        Application.LoadLevel("StartPage");
    }

    void AdvanceScene() {
        AnimatedText infoDialogAnimatedText = infoDialog.GetComponent<AnimatedText>();
        AnimatedText infoDialog2AnimatedText = infoDialog2.GetComponent<AnimatedText>();

        if (infoDialogAnimatedText == null) return;
        if (infoDialog2AnimatedText == null) return;

        if (infoDialogAnimatedText.TextFinished)
        {
            if (sceneTimeStep == infoDialogAnimatedText.NumOfText + 0)
            {
                infoDialog.SetActive(false);
                childModel.SetActive(true);

                sceneTimeStep++;
                return;
            }
            else if (sceneTimeStep == infoDialogAnimatedText.NumOfText + 2)
            {
                childModel.SendMessage("SetScaredEmotion");
            }
            else if (sceneTimeStep == infoDialogAnimatedText.NumOfText + 3)
            {
                childModel.SendMessage("SetHappyEmotion");
            }
            else if (sceneTimeStep == infoDialogAnimatedText.NumOfText + 4) {
                OpenStartPageScene();
            }

            infoDialog2AnimatedText.SendMessage("AdvanceText");
        }
        else {
            infoDialogAnimatedText.SendMessage("AdvanceText");
        }

        sceneTimeStep++;

    }
}
