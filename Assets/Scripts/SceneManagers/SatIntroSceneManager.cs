using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HoloToolkit.Unity;

public class SatIntroSceneManager : MonoBehaviour {

    private int sceneTimeStep;
    public GameObject infoDialog;
    public GameObject infoDialog2;
    public GameObject childModel;
    public GameObject alertDialog;
    public GameObject sceneManager;
    public HeadsUpDirectionIndicator directionIndicator;

    void Start () {
        if (!LocationManager.Instance.ChildLocationSet) return;
        StartProtocol();
	}

    void Awake()
    {
        enabled = shouldEnable();
    }

    void StartProtocol() {
        if (!shouldEnable()) return;
        infoDialog.SetActive(true);
        sceneTimeStep = 0;
        childModel.SetActive(false);
        childModel.SendMessage("DeactiveEmoScreen");
        directionIndicator.HideIndicator();
    }

    void AdvanceScene() {
        if (!enabled) return;
        AnimatedText infoDialogAnimatedText = infoDialog.GetComponent<AnimatedText>();
        AnimatedText infoDialog2AnimatedText = infoDialog2.GetComponent<AnimatedText>();

        if (infoDialogAnimatedText == null) return;
        if (infoDialog2AnimatedText == null) return;

        if (infoDialogAnimatedText.TextFinished)
        {
            if (sceneTimeStep == infoDialogAnimatedText.NumOfText + 0)
            {
                infoDialog.SetActive(false);
                alertDialog.SetActive(true);
                infoDialog2.SetActive(true);
                childModel.SetActive(true);
                childModel.SendMessage("DeactiveEmoScreen");
                directionIndicator.ShowIndicator();

                sceneTimeStep++;
                return;
            }
            else if (sceneTimeStep == infoDialogAnimatedText.NumOfText + 2)
            {
                alertDialog.SetActive(false);
                childModel.SendMessage("SetScaredEmotion");
            }
            else if (sceneTimeStep == infoDialogAnimatedText.NumOfText + 3)
            {
                childModel.SendMessage("SetHappyEmotion");
            }
            else if (sceneTimeStep == infoDialogAnimatedText.NumOfText + 4) {
                sceneManager.SendMessage("OpenStartPageScene");
            }

            infoDialog2AnimatedText.SendMessage("AdvanceText");
        }
        else {
            infoDialogAnimatedText.SendMessage("AdvanceText");
        }

        sceneTimeStep++;

    }

    private bool shouldEnable()
    {
        return ScenesData.currentProtocol == ScenesData.ProtocolType.STAGE_1;
    }
}
