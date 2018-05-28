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

    private IEnumerator phraseProtocolCoroutine;
    private bool phraseDetected = false;
    private int phrase1Id = 1;
    private int phrase2Id = 2;

    void Awake()
    {
        enabled = shouldEnable();
    }

    void Start () {
        if (!LocationManager.Instance.ChildLocationSet) return;
        StartProtocol();
    }

    private void StartProtocol() {
        if (!shouldEnable()) return;

        microphoneManager = GetComponent<MicrophoneManager>();

        AnimatedText dialogScreenAnimatedText = dialogScreen.AddComponent<AnimatedText>() as AnimatedText;
        dialogScreenAnimatedText.strings = new string[] {
            "Now you will perform exercises which will help you form and maintain a loving relationship with your inner-child.",
            "Now you feel better connected with your inner-child...",
            "It is important you love your inner-child and vow to comfort it during its times of need.",
            "To show your love for your inner-child, think of one of your favourite songs from your childhood (e.g. a love song)",
            "Nicely done!!! Your song has made the inner-child feel more loved",
            "Now let's move on to more activities to increase the bond you have with your inner-child.",
            "Giving yourself a massage is a great way to relieve stress and relax. You can massage any part of your body including your arms, neck and face.",
            "Excellent!!! Your efforts have made your inner-child feel more loved and connected with you",
            "Now let's revisit our soothing song... we can sing this song when we are in times of stress to remind us of the importance of comforting our inner-child. Slowly moving our body to the rhytm also helps with this exercise.",
            "Hence we must practice comfortably and intutively singing this song often to remind you of your connection to your inner-child and help you feel the happiness associated with the song",
            "Excellent!!!, your song has made your inner-child much happier",
            "It is very important that you remember to sing this song whenever you feel distressed. It will remind you of your loving bond with your inner-child who you want to feel the happiness associated with song.",
            "Practice singing to your inner-child until the action becomes intuitive and natural. Now to complete this stage... We must practice reciting important phrases to our inner-child.",
            "Excellent!!! continue to practice these activities continuously so you can use them to improve your mood in connection with your inner-child."

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
            activateActionScreen("Think of your song... sing it to your inner-child. Also slowly dance along to your song");
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
        else if (protocolStep < numberFirstSectionDialog + 2)
        {
            child.SendMessage("SetNeutralEmotion");
            dialogScreen.SendMessage("AdvanceText");
        }
        else if (protocolStep == numberFirstSectionDialog + 3)
        {
            activateActionScreen("Please give yourself a self-massage. Feel free to remove the headset if you need to (Tap to continue)");
        }
        else if (protocolStep == numberFirstSectionDialog + 4)
        {
            activateDialogScreen();
            child.SendMessage("SetHappyEmotion");
            dialogScreen.SendMessage("AdvanceText");
        }
        else if (protocolStep < numberFirstSectionDialog + 6)
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
        else if (protocolStep == numberFirstSectionDialog + 9)
        {
            if (!songDetected) return;

            StopCoroutine(songProtocolCoroutine);
            microphoneManager.StopRecording();
            activateDialogScreen();
            dialogScreen.SendMessage("AdvanceText");

            child.SendMessage("SetHappyEmotion");
        }
        else if (protocolStep <= numberFirstSectionDialog + 11)
        {
            dialogScreen.SendMessage("AdvanceText");
        }
        else if (protocolStep == numberFirstSectionDialog + 12)
        {
            phraseDetected = false;
            microphoneManager.StartRecording();
            child.SendMessage("SetNeutralEmotion");
            activateActionScreen("Say these words to your inner-child: 'I love you'");
            microphoneManager.detectPhrase(phrase1Id, new string[] { "I", "love", "you" });
            phraseProtocolCoroutine = phraseProtocol(phrase1Id);
            StartCoroutine(phraseProtocolCoroutine);
        }
        else if (protocolStep == numberFirstSectionDialog + 13)
        {
            if (!phraseDetected) return;
            StopCoroutine(phraseProtocolCoroutine);
            microphoneManager.deletePhrase(phrase1Id);
            phraseDetected = false;

            child.SendMessage("SetHappyEmotion");
            activateActionScreen("Great! now say these words: 'I vow to protect you'");
            microphoneManager.detectPhrase(phrase2Id, new string[] { "I", "vow", "to", "protect", "you" });
            phraseProtocolCoroutine = phraseProtocol(phrase2Id);
            StartCoroutine(phraseProtocolCoroutine);

        }
        else if (protocolStep == numberFirstSectionDialog + 14)
        {
            if (!phraseDetected) return;
            StopCoroutine(phraseProtocolCoroutine);
            microphoneManager.StopRecording();

            activateDialogScreen();
            dialogScreen.SendMessage("AdvanceText");
        }
        else if (protocolStep == numberFirstSectionDialog + 15) {
            sceneManager.SendMessage("OpenStartPageScene");
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
                child.SendMessage("SetNeutralEmotion");
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

    IEnumerator phraseProtocol(int phraseId) {
        while (!microphoneManager.isPhraseDetected(phraseId)) {
            yield return new WaitForSeconds(0);
        }

        phraseDetected = true;
        AdvanceProtocol();
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

    private bool shouldEnable()
    {
        return ScenesData.currentProtocol == ScenesData.ProtocolType.STAGE_3;
    }
}
