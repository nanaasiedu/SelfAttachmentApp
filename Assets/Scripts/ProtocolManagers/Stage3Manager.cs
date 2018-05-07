using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage3Manager : MonoBehaviour {

    public GameObject child;
    public GameObject dialogScreen;
    public GameObject actionScreen;
    public GameObject sceneManager;
    private MicrophoneManager microphoneManager;

    private int protocolStep = 0;
    private IEnumerator songProtocolCoroutine;
    private static int numberFirstSectionDialog = 4;
    private bool songDetected = false;

    void Awake()
    {
        enabled = ScenesData.currentProtocol == ScenesData.ProtocolType.STAGE_3;
    }

    void Start () {
        microphoneManager = GetComponent<MicrophoneManager>();

        AnimatedText dialogScreenAnimatedText = dialogScreen.AddComponent<AnimatedText>() as AnimatedText;
        dialogScreenAnimatedText.strings = new string[] {
            "Now you will perform exercises which will help you form and maintain a loving relationship with your inner-child.",
            "Now you feel better connected with the inner-child...",
            "it is important you love the inner-child and vow to comfort it during its time of need.",
            "To show your love for your inner-child, think of one of your favourite songs from your childhood (e.g. a love song)",
            "Nicely done!!! Your song has made the inner-child feel more loved",
            "Now let's move on to more activities to increase the bond you have with your inner-child.",
            "Giving yourself a massage is a great way to relieve stress and relax. You can give yourself a neck massage by firmly and slowly rubbing your neck.",
            "Excellent!!! Your efforts have made your inner-child feel more loved and connected with you",
            "Now let's revisit our soothing song... we can sing this song when we are in times of stress to remind us of the importance of comforting our inner-child",
            "Hence we must practice comfortably and intutively singing this song often to remind you of your connection to your inner-child and help you feel the happiness associated with the song",
            "Excellent!!!, your song has made your inner-child much happier"

        };
        dialogScreenAnimatedText.enabled = true;

        child.SendMessage("DeactiveEmoScreen");
        child.SendMessage("SetNeutralEmotion");
        dialogScreen.SetActive(true);
    }

    public void AdvanceProtocol() {
        if (!enabled) return;

        if (protocolStep < numberFirstSectionDialog - 1)
        {
            dialogScreen.SendMessage("AdvanceText");
        }
        else if (protocolStep == numberFirstSectionDialog - 1)
        {
            activateActionScreen("Think of your song... sing it to your inner-child");
            microphoneManager.StartRecording();
            songProtocolCoroutine = songProtocol(false);
            StartCoroutine(songProtocolCoroutine);
        }
        else if (protocolStep == numberFirstSectionDialog)
        {
            if (!songDetected) return;

            StopCoroutine(songProtocolCoroutine);
            microphoneManager.StopRecording();
            activateDialogScreen();
            dialogScreen.SendMessage("AdvanceText");

            child.SendMessage("SetHappyEmotion");
        }
        else if (protocolStep == numberFirstSectionDialog + 1 || protocolStep == numberFirstSectionDialog + 2)
        {
            child.SendMessage("SetNeutralEmotion");
            dialogScreen.SendMessage("AdvanceText");
        }
        else if (protocolStep == numberFirstSectionDialog + 3)
        {
            activateActionScreen("Please give yourself a self-massage (Tap to continue)");
        }
        else if (protocolStep == numberFirstSectionDialog + 4)
        {
            activateDialogScreen();
            child.SendMessage("SetHappyEmotion");
            dialogScreen.SendMessage("AdvanceText");
        }
        else if (protocolStep == numberFirstSectionDialog + 5 || protocolStep == numberFirstSectionDialog + 6)
        {
            child.SendMessage("SetNeutralEmotion");
            dialogScreen.SendMessage("AdvanceText");
        }
        else if (protocolStep == numberFirstSectionDialog + 7)
        {
            child.SendMessage("SetSadEmotion");
            activateActionScreen("Recall a distressing memory from your past... (Tap to continue)");
        }
        else if (protocolStep == numberFirstSectionDialog + 8)
        {
            songDetected = false;
            child.SendMessage("SetScaredEmotion");
            activateActionScreen("Now sing the song you associate with happy memories and feelings to make your inner-child feel loved");

            microphoneManager.StartRecording();
            songProtocolCoroutine = songProtocol(true);
            StartCoroutine(songProtocolCoroutine);
        }
        else if (protocolStep == numberFirstSectionDialog + 9) {
            if (!songDetected) return;

            StopCoroutine(songProtocolCoroutine);
            microphoneManager.StopRecording();
            activateDialogScreen();
            dialogScreen.SendMessage("AdvanceText");

            child.SendMessage("SetHappyEmotion");
        }

        protocolStep++;
    }

    IEnumerator songProtocol(bool sadProtocol)
    {
        int numberOfIterations = 0;

        while (true)
        {
            yield return new WaitForSeconds(ScenesData.songRate_perSecond);

            int numberOfNewWords = microphoneManager.numberOfRecentWords();
            Debug.Log("New words: " + numberOfNewWords);

            if (numberOfNewWords < ScenesData.songRate_numberOfWords) continue;

            numberOfIterations++;
            float elapsedTime = numberOfIterations * ScenesData.songRate_perSecond;
            float percentageComplete = elapsedTime / (sadProtocol ? ScenesData.songDuration*2 : ScenesData.songDuration);

            if (sadProtocol && percentageComplete < 0.2) {
                child.SendMessage("SetNeutralProtocl");
            }

            if (percentageComplete < 0.4)
            {
                setActionScreenText("Great!!! Keep going...");
                child.SendMessage("SetHappyEmotion");
            }
            else if (percentageComplete < 0.7)
            {
                setActionScreenText("Good job, the inner-child is really getting into it!");
                child.SendMessage("SetDanceEmotion");
            }
            else if (percentageComplete >= 1.0)
            {
                songDetected = true;
                AdvanceProtocol();
                break;
            }
        }
    }

    public void ResetAfterTimeout() {
        microphoneManager.StartRecording();
    }

    private void activateActionScreen(string message)
    {
        dialogScreen.SetActive(false);
        setActionScreenText(message);
        actionScreen.SetActive(true);
    }

    private void setActionScreenText(string message)
    {
        ScreenText screenText = actionScreen.GetComponent<ScreenText>();

        if (screenText == null) return;
        screenText.SetText(message);
    }

    private void activateDialogScreen()
    {
        dialogScreen.SetActive(true);
        actionScreen.SetActive(false);
    }

}
