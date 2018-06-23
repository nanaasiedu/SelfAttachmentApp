using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HoloToolkit.Unity;
using HoloToolkit.Unity.SpatialMapping;

public class Stage4Manager : MonoBehaviour {

    public GameObject child;
    public GameObject dialogScreen;
    public GameObject actionScreen;
    public GameObject sceneManager;

    public GameObject gestaltscreen;
    public Sprite gestaltVaseImage;
    public Sprite gestaltFaceImage;

    public GameObject carpetPrefab;
    public GameObject plantPrefab;
    public GameObject plantPotPrefab;
    public GameObject chandelierPrefab;
    public GameObject portraitPrefab;
    public GameObject portrait2Prefab;

    private IEnumerator hugProtocolCoroutine;
    private bool hugProtocolComplete = false;

    private MicrophoneManager microphoneManager;
    private IEnumerator phraseProtocolCoroutine;
    private bool phraseDetected = false;
    private int phrase1Id = 1;

    private IEnumerator songProtocolCoroutine;
    private bool songDetected = false;

    private bool eyesClocsedComplete = false;
    private IEnumerator eyeClosedProtocolCoroutine;
    private static float eyesCloseDuration = 5;

    private int protocolStep = 0;
    private int typeAProtocolStart = 0;
    private int typeDProtocolStart = 10;
    private int typeFProtocolStart = 20;

    

    void Awake()
    {
        enabled = shouldEnable();
    }

    void Start()
    {
        if (!LocationManager.Instance.ChildLocationSet || ScenesData.autoRoomScan) return;
        StartProtocol();
    }

    private void StartProtocol()
    {
        if (!shouldEnable()) return;

        microphoneManager = GetComponent<MicrophoneManager>();

        AnimatedText dialogScreenAnimatedText = dialogScreen.AddComponent<AnimatedText>() as AnimatedText;
        dialogScreenAnimatedText.textToSpeech = sceneManager.GetComponent<TextToSpeech>();
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
            "Once this is achieved, we can disassociate the dark emotions from the image and start to see new possibilities...",
            "Look at the new red outline, you should be able to make out two white faces!",
            "These white faces represent our adult-self and inner-child looking at each other in a loving relationship",
            "We tend to see things from different perspectives depending on our emotions",

            "Finally, through performing these protocols to increase positive affects and contain the inner-child's negative thoughts and emotions...",
            "we start to realise these feelings stem from our negative past experiences e.g. toxic parental relationships.",
            "But as you continue to reduce these negative emotions, you and your inner-child can sense the growth in your emotional maturity and you no longer...",
            "have to see yourself as a scared prisoner of your past. You can now rely on your loving relationship with your inner-child to escape your struggles.",
            "Your cooperation with your inner child can be represented as a new and beautiful house with beautiful scenery...",
            "We can use the HoloLens to make this scenery a reality...",
            "Now look around your enhanced augmented environment!",
            "The chandelier has brighten up your room! The light and the beautiful new scenery is based on your new secure attachment with your inner child",
            "As your bond with your inner-child increases... think of this bright scenery to help remind you of your attachment."
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
        if (!typeFProtocol()) return;

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

    private bool typeFProtocol() {
        if (protocolStep < typeFProtocolStart + 0) return true;

        if (protocolStep <= typeFProtocolStart + 5)
        {
            child.SendMessage("SetNeutralEmotion");
            dialogScreen.SendMessage("AdvanceText");
        }
        else if (protocolStep <= typeFProtocolStart + 6)
        {
            activateActionScreen("Close your eyes for 5 seconds...");
            eyeClosedProtocolCoroutine = eyeCloseProtocol();
            StartCoroutine(eyeClosedProtocolCoroutine);
        }
        else if (protocolStep <= typeFProtocolStart + 7)
        {
            if (!eyesClocsedComplete) return false;
            StopCoroutine(eyeClosedProtocolCoroutine);

            child.SendMessage("SetHappyEmotion");
            activateDialogScreen();
            dialogScreen.SetActive(false);
            dialogScreen.SendMessage("AdvanceText");

            transformRoom();
        }
        else if (protocolStep <= typeFProtocolStart + 8) {
            sceneManager.SendMessage("OpenStartPageScene");
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

    IEnumerator eyeCloseProtocol() {
        yield return new WaitForSeconds(eyesCloseDuration);
        eyesClocsedComplete = true;
        AdvanceProtocol();

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

    private void transformRoom() {
        placeCarpet();
        placePlants();
        placeChandelier();
        placePlantPot();
        placePortraits();
    }

    private void placeCarpet() {
        GameObject carpet = (GameObject)Instantiate(carpetPrefab);
        carpet.transform.position = LocationManager.Instance.CarpetLocation;
        carpet.transform.localScale = new Vector3(ScenesData.carpetScale, ScenesData.carpetScale, ScenesData.carpetScale);
    }

    private void placePlants() {
        for (int i = 0; i < ScenesData.numberOfFloorObjects; i++)
        {
            GameObject plantObj = ((GameObject)Instantiate(plantPrefab));
            plantObj.transform.position = LocationManager.Instance.generateFloorPosition;
            plantObj.transform.localScale = new Vector3(ScenesData.plantScale, ScenesData.plantScale, ScenesData.plantScale);
        }
    }

    private void placeChandelier() {
        GameObject chandelier = (GameObject)Instantiate(chandelierPrefab);
        chandelier.transform.position = LocationManager.Instance.ChandelierPosition;
        chandelier.transform.localScale = new Vector3(ScenesData.chandelierScale, ScenesData.chandelierScale, ScenesData.chandelierScale);
        Light lightComponent = chandelier.AddComponent<Light>();
        lightComponent.type = LightType.Directional;
    }

    private void placePlantPot() {
        GameObject plantPot = (GameObject)Instantiate(plantPotPrefab);
        plantPot.transform.position = LocationManager.Instance.PlantPotPosition;
    }

    private void placePortraits() {
        GameObject portrait = portraitPrefab;
        portrait.SetActive(true);
        portrait.transform.position = LocationManager.Instance.wallPortraitPosition;
        portrait.transform.forward = LocationManager.Instance.wallPortraitNormal;

        GameObject portrait2 = portrait2Prefab;
        portrait2.SetActive(true);
        portrait2.transform.position = LocationManager.Instance.wallPortraitPosition;
        portrait2.transform.forward = LocationManager.Instance.wallPortraitNormal;

    }
}
