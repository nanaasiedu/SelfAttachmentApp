using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage2Manager : MonoBehaviour {

    public GameObject child;
    public GameObject dialogScreen;
    public GameObject actionScreen;
    public GameObject sceneManager;

    private int protocolStep = 0;
    private IEnumerator hugProtocolCoroutine;
    private bool hugProtocolComplete = false;

    void Awake() {
        enabled = ScenesData.currentProtocol == ScenesData.ProtocolType.STAGE_2;
    }
	
	void Start () {
        child.SendMessage("DeactiveEmoScreen");
        child.SendMessage("SetSadEmotion");
        dialogScreen.SetActive(true);
    }

    public void AdvanceProtocol() {

        if (protocolStep == 0)
        {
            dialogScreen.SendMessage("AdvanceText");
            child.SendMessage("SetScaredEmotion");
        }
        else if (protocolStep == 1)
        {
            activateActionScreen("Think about a time in your past when you were distressed. (Tap once complete)");
        }
        else if (protocolStep == 2)
        {
            activateDialogScreen();
            dialogScreen.SendMessage("AdvanceText");
            child.SendMessage("SetNeutralEmotion");
        }
        else if (protocolStep == 3)
        {
            activateActionScreen("Think about a time in your past when you were happy. (Tap once complete)");
            child.SendMessage("SetHappyEmotion");
        }
        else if (protocolStep == 4)
        {
            activateDialogScreen();
            dialogScreen.SendMessage("AdvanceText");
            child.SendMessage("SetNeutralEmotion");
        }
        else if (protocolStep == 5)
        {
            dialogScreen.SendMessage("AdvanceText");
        }
        else if (protocolStep == 6)
        {
            child.SendMessage("showZones");
            activateActionScreen("---");
            child.SendMessage("SetHappyEmotion");

            hugProtocolCoroutine = hugProtocol();
            StartCoroutine(hugProtocolCoroutine);
        }
        else if (protocolStep == 7)
        {
            if (!hugProtocolComplete)
            {
                return;
            }

            StopCoroutine(hugProtocolCoroutine);
            activateDialogScreen();
            dialogScreen.SendMessage("AdvanceText");
            child.SendMessage("SetNeutralEmotion");
        }
        else if (protocolStep == 8) {
            sceneManager.SendMessage("OpenStartPageScene");
        }

        protocolStep++;
    }

    IEnumerator hugProtocol() {
        while (true) {
            AreaManager areaManager = child.GetComponent<AreaManager>();

            if (areaManager.InDeadZone)
            {
                setActionScreenText("You are too close to the inner-child. Please step back");
                hugProtocolComplete = false;
            }
            else if (areaManager.InHugZone)
            {
                areaManager.hideZones();
                setActionScreenText("Give yourself a hug while looking at the inner-child and imagine they are in your arms. (Tap to continue)");
                hugProtocolComplete = true;
            }
            else
            {
                setActionScreenText("Step into the green ring closer to the inner-child");
                hugProtocolComplete = false;
            }

            yield return new WaitForSeconds(0);
        }
    }

    private void activateActionScreen(string message) {
        dialogScreen.SetActive(false);
        setActionScreenText(message);
        actionScreen.SetActive(true);
    }

    private void setActionScreenText(string message) {
        ScreenText screenText = actionScreen.GetComponent<ScreenText>();

        if (screenText == null) return;
        screenText.SetText(message);
    }

    private void activateDialogScreen() {
        dialogScreen.SetActive(true);
        actionScreen.SetActive(false);
    }
}
