using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using HoloToolkit.Unity;

public class SatIntroSceneManager : MonoBehaviour {

    private int sceneTimeStep;
    public GameObject infoDialog;
    public GameObject infoDialog2;
    public GameObject childModel;
    public GameObject alertDialog;
    public GameObject sceneManager;
    public GameObject attachmentTherapyImage;
    public HeadsUpDirectionIndicator directionIndicator;
    public Text tutorialText;

    private string[] strings = new string[] {
            "Welcome to Self-attachment Therapy with Augmented Reality. You will experience a new and immersive way to self-regulate your emotions using technology!",
            "Depression and anxiety generally stems from emotional problems from your childhood due to emotionally distant parents.",
            "Attachment therapy, detailed by the renowned psychoanalyst John Bowlby, outlines the effects various levels of parenting have on a child",
            "This image outlines the 4 key types of attachment associated with Attachment therapy.",
            "Poor parenting can lead to a child forming negative insecure attachments with their care-givers.",
            "These insecure attachments follow the child to adulthood and can develop into depression that can hinder their social interactions.",
            "Depression has negative neurological effects on your brain. The brain has an arrangement of neural pathways that contain neurons that allow electrical signals to be sent to different parts of the body.",
            "However, what is interesting is that our brains change when we latch on to certain emotions for an extended period of time.",
            "Depression has the effect of reducing a sufferers hippocampus, which is a segment of the brain that effect the user's spatial awareness, personality and emotions.",
            "This damage can make it difficult for suffers to overcome their depression.",
            "However this damage is reversible due to the brains neuroplasticity. The brain has the ability to rearrange its neural pathways based on new experiences and emotions.",
            "In other words, if you continuously practice positive activities that calm your fears you will eventually be able to rearrange the neural pathways in a way that will improve your mood.",
            "This process is known as long term potentiation, and it is an effective way of training the brain to use positive exercises repeatedly during times of distress to allow the user to self-regulate their emotions.",
            "Self-attachment therapy consist of several exercises you will perform to help with this. The aim of the therapy is to provide you with a secure attachment you can rely on during times of stress.",
            "This secure attachment will be between you and your 'inner-child', an embodiment of your fears, weaknesses and vulnerabilities..."
        };

    private string[] strings2 = new string[] {
        "Here is your inner-child. Throughout this therapy you will learn to form a meaningful attachment with them that will strengthen your ability to improve your mood.",
        "During times of distress, your inner-child becomes sad and fearful...",
        "It is up to you, the strong adult-self, to perform a series of exercises to comfort and reassure the inner-child of the loving attachment you share with them.",
        "repeatedly performing the subsequent stages will allow you to become habituated to remember your loving relationship with your inner-child.",
        "you will become so used to this that you will be able to rewire your neural pathways to alleviate the effects depression had on your brain.",
        "Now let's get started! Try selecting one of the other stages to begin your therapy"
    };

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

        tutorialText.text = "Place the cursor on the box by gazing at it. Then Air-Tap to continue";

        AnimatedText dialogScreenAnimatedText = infoDialog.AddComponent<AnimatedText>() as AnimatedText;
        dialogScreenAnimatedText.textToSpeech = sceneManager.GetComponent<TextToSpeech>();
        dialogScreenAnimatedText.strings = strings;

        AnimatedText dialogScreenAnimatedText2 = infoDialog2.AddComponent<AnimatedText>() as AnimatedText;
        dialogScreenAnimatedText2.textToSpeech = sceneManager.GetComponent<TextToSpeech>();
        dialogScreenAnimatedText2.strings = strings2;

        infoDialog.SetActive(true);
        sceneTimeStep = 0;
        childModel.SendMessage("DeactiveEmoScreen");
        childModel.SetActive(false);
        directionIndicator.HideIndicator();
    }

    void AdvanceProtocol() {
        if (!enabled) return;

        if (sceneTimeStep >= strings.Length)
        {
            if (sceneTimeStep == strings.Length + 0)
            {
                infoDialog.SetActive(false);
                alertDialog.SetActive(true);
                infoDialog2.SetActive(true);
                childModel.SetActive(true);

                directionIndicator.ShowIndicator();
                childModel.SendMessage("SetNeutralEmotion");

                sceneTimeStep++;
                return;
            }
            else if (sceneTimeStep == strings.Length + 1)
            {
                alertDialog.SetActive(false);
                childModel.SendMessage("SetSadEmotion");
            }
            else if (sceneTimeStep == strings.Length + 2)
            {
                childModel.SendMessage("SetScaredEmotion");
            }
            else if (sceneTimeStep == strings.Length + 3) {
                childModel.SendMessage("SetHappyEmotion");
            } 
            else if (sceneTimeStep == strings.Length + strings2.Length)
            {
                sceneManager.SendMessage("OpenStartPageScene");
            }

            infoDialog2.SendMessage("AdvanceText");
        }
        else {
            if (sceneTimeStep == 1) tutorialText.text = "You can also use special keywords to interact with the app";
            if (sceneTimeStep == 2) tutorialText.text = "Say 'next' to advance the text";
            if (sceneTimeStep == 3) tutorialText.text = "Say 'select' instead of using Air tap to select an object";
            if (sceneTimeStep == 4) tutorialText.text = "Say 'restart' to go back to the main menu";

            if (sceneTimeStep == 3) attachmentTherapyImage.SetActive(true);
            if (sceneTimeStep == 7) attachmentTherapyImage.SetActive(false);

            infoDialog.SendMessage("AdvanceText");
        }

        sceneTimeStep++;

    }

    private bool shouldEnable()
    {
        return ScenesData.currentProtocol == ScenesData.ProtocolType.STAGE_1;
    }
}
