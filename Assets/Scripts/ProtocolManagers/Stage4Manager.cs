using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stage4Manager : MonoBehaviour {

    public GameObject child;
    public GameObject dialogScreen;
    public GameObject actionScreen;
    public GameObject sceneManager;

    public GameObject gestaltscreen;
    public Sprite gestaltVaseImage;
    public Sprite gestaltFaceImage;

    private IEnumerator hugProtocolCoroutine;
    private bool hugProtocolComplete = false;

    private MicrophoneManager microphoneManager;
    private IEnumerator phraseProtocolCoroutine;
    private bool phraseDetected = false;
    private int phrase1Id = 1;

    private IEnumerator songProtocolCoroutine;
    private bool songDetected = false;

    private int protocolStep = 0;
    private int typeAProtocolStart = 0;
    private int typeDProtocolStart = 10;

    void Awake()
    {
        enabled = shouldEnable();
    }

    void Start()
    {
        if (!LocationManager.Instance.ChildLocationSet) return;
        StartProtocol();
    }

    private void StartProtocol()
    {
        if (!shouldEnable()) return;

        microphoneManager = GetComponent<MicrophoneManager>();

        AnimatedText dialogScreenAnimatedText = dialogScreen.AddComponent<AnimatedText>() as AnimatedText;
        dialogScreenAnimatedText.strings = new string[] {
            "Through the previous stages we have learnt exercises to strengthen our attachment to our inner-child.",
            "Using the bond you have developed with your child, these exercises can be used to calm your inner-child's emotional state when you experience trauma due to past events.",
            "For the first protocol, we need to imagine a traumatic experience from our past that has caused us to feel emotions associated with terror, helplessness, humiliation or rage.",
            "These negative emotions make your inner-child distressed... during these times our adult-self must quickly comfort and embrace the inner-child to make them happy again.",
            "Great, Your embrace has improved the emotional state of the inner-child.",
            "Now it is also important that we tell our inner-child with a loud voice supportive and reassuring phrases to help ease the pain of the traumatic experiences.",
            "Great! By continuing to rehearse this protocol you will be better able to self-regulate your emotions when you inner-child feels distressed",

            "Now we will move on to analyse a special image called the Gesalt vase which is a black vase that is associated with powerful dark and negative emotions.",
            "Look next to the inner-child at the Gesalt vase...",
            "However, the positive loving bond you have created with the inner-child using the previous protocols can be used to help overcome the negative emotions associated witht he vase. This can be done through singing your happy song for example.",
            "Excellent! Using the exercises learnt is a great way to relieve the stress caused by these dark emotions.",
            "Once this is achieved, we can disassociate the dark emotions from the image and start to see new possibilites...",
            "Look at the new red outline, you should be able to make out two white faces!",
            "These white faces represent our adult-self and inner-child looking at each other in a loving relationship",
            "We tend to see thing from different perspectives depending on our emotions"
        };
        dialogScreenAnimatedText.enabled = true;

        child.SendMessage("DeactiveEmoScreen");
        child.SendMessage("SetSadEmotion");
        dialogScreen.SetActive(true);
    }

    public void AdvanceProtocol() {
        if (!enabled) return;

        if (!typeAProtocol()) return;
        if (!typeDProtocol()) return;

        protocolStep++;
    }

    private bool typeAProtocol() {
        if (protocolStep < 2)
        {
            dialogScreen.SendMessage("AdvanceText");
        }
        else if (protocolStep <= 2)
        {
            activateActionScreen("Think back to your traumatic experience. (Tap to Continue)");
            child.SendMessage("SetScaredEmotion");
        }
        else if (protocolStep <= 3)
        {
            activateDialogScreen();
            dialogScreen.SendMessage("AdvanceText");
        }
        else if (protocolStep <= 4)
        {
            child.SendMessage("showZones");
            activateActionScreen("---");

            hugProtocolCoroutine = hugProtocol();
            StartCoroutine(hugProtocolCoroutine);
        }
        else if (protocolStep <= 5)
        {
            if (!hugProtocolComplete) return false;

            StopCoroutine(hugProtocolCoroutine);
            activateDialogScreen();
            dialogScreen.SendMessage("AdvanceText");
            child.SendMessage("SetNeutralEmotion");
        }
        else if (protocolStep <= 6)
        {
            dialogScreen.SendMessage("AdvanceText");
        }
        else if (protocolStep <= 7)
        {
            phraseDetected = false;
            microphoneManager.StartRecording();
            activateActionScreen("Say these words to your inner-child: 'I will support you'");
            microphoneManager.detectPhrase(phrase1Id, new string[] { "I", "will", "support", "you" });
            phraseProtocolCoroutine = phraseProtocol(phrase1Id);
            StartCoroutine(phraseProtocolCoroutine);
        }
        else if (protocolStep <= 8)
        {
            if (!phraseDetected) return false;
            StopCoroutine(phraseProtocolCoroutine);
            microphoneManager.deletePhrase(phrase1Id);
            microphoneManager.StopRecording();
            phraseDetected = false;

            child.SendMessage("SetHappyEmotion");
            activateActionScreen("Great! now encourage and reassure your inner-child. (Tap to Continue)");
        }
        else if (protocolStep <= 9) {
            activateDialogScreen();
            dialogScreen.SendMessage("AdvanceText");
        }

        return true;
    }

    private bool typeDProtocol() {
        if (protocolStep < typeDProtocolStart + 0) return true;

        if (protocolStep <= typeDProtocolStart + 0)
        {
            child.SendMessage("SetNeutralEmotion");
            activateDialogScreen();
            dialogScreen.SendMessage("AdvanceText");
        }
        else if (protocolStep <= typeDProtocolStart + 1)
        {
            dialogScreen.SendMessage("AdvanceText");
            showNormalGestaltImage();
        }
        else if (protocolStep <= typeDProtocolStart + 2)
        {
            child.SendMessage("SetSadEmotion");
            activateActionScreen("Stare intensely at the black vase and think about your negative dark emotions... (Tap to continue).");
        }
        else if (protocolStep <= typeDProtocolStart + 3)
        {
            child.SendMessage("SetScaredEmotion");
            activateDialogScreen();
            dialogScreen.SendMessage("AdvanceText");
        }
        else if (protocolStep <= typeDProtocolStart + 4)
        {
            songDetected = false;
            activateActionScreen("Now sing your happy song to act against these dark emotions");

            microphoneManager.StartRecording();
            songProtocolCoroutine = songProtocol(true);
            StartCoroutine(songProtocolCoroutine);
        }
        else if (protocolStep <= typeDProtocolStart + 5)
        {
            if (!songDetected) return false;

            StopCoroutine(songProtocolCoroutine);
            microphoneManager.StopRecording();
            activateDialogScreen();
            dialogScreen.SendMessage("AdvanceText");
        }
        else if (protocolStep <= typeDProtocolStart + 6)
        {
            dialogScreen.SendMessage("AdvanceText");
        }
        else if (protocolStep <= typeDProtocolStart + 8)
        {
            showRedGestaltImage();
            dialogScreen.SendMessage("AdvanceText");
        }
        else if (protocolStep <= typeDProtocolStart + 9) {
            hideGesaltImage();
            dialogScreen.SendMessage("AdvanceText");
        }

        return true;
    }

    IEnumerator hugProtocol()
    {
        while (true)
        {
            AreaManager areaManager = child.GetComponent<AreaManager>();

            if (areaManager.InDeadZone)
            {
                child.SendMessage("SetScaredEmotion");
                areaManager.showZones();
                setActionScreenText("You are too close to the inner-child. Please step back");
                hugProtocolComplete = false;
            }
            else if (areaManager.InHugZone)
            {
                child.SendMessage("SetSadEmotion");
                areaManager.hideZones();
                setActionScreenText("Give yourself a hug while looking at the inner-child and imagine they are in your arms. (Tap to continue)");
                hugProtocolComplete = true;
            }
            else
            {
                child.SendMessage("SetScaredEmotion");
                areaManager.showZones();
                setActionScreenText("Step into the green ring closer to the inner-child");
                hugProtocolComplete = false;
            }

            yield return new WaitForSeconds(0);
        }
    }

    IEnumerator phraseProtocol(int phraseId)
    {
        while (!microphoneManager.isPhraseDetected(phraseId))
        {
            yield return new WaitForSeconds(0);
        }

        phraseDetected = true;
        AdvanceProtocol();
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
            float percentageComplete = elapsedTime / (sadProtocol ? ScenesData.songDuration * 2 : ScenesData.songDuration);

            if (percentageComplete < 0.4)
            {
                setActionScreenText("Great!!! Keep going...");
                child.SendMessage("SetNeutralEmotion");
            }
            else if (percentageComplete < 0.7)
            {
                setActionScreenText("Good job, the inner-child is really getting into it!");
                child.SendMessage("SetHappyEmotion");
            }
            else if (percentageComplete >= 1.0)
            {
                songDetected = true;
                AdvanceProtocol();
                break;
            }
        }
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
        return ScenesData.currentProtocol == ScenesData.ProtocolType.STAGE_4;
    }

    private void showNormalGestaltImage() {
        setGestaltImage(gestaltVaseImage);
    }

    private void showRedGestaltImage() {
        setGestaltImage(gestaltFaceImage);
    }

    private void setGestaltImage(Sprite image) {
        Image gestaltImage = gestaltscreen.GetComponentInChildren<Image>();

        gestaltImage.sprite = image;

        gestaltscreen.SetActive(true);
    }

    private void hideGesaltImage() {
        gestaltscreen.SetActive(false);
    }


}
