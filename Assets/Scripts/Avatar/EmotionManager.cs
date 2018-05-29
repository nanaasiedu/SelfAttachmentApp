using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class EmotionManager : MonoBehaviour {

    private Animator animator;

    KeywordRecognizer keywordRecognizer = null;
    Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        
        setAnimateOnSpeech("sad", "SAD");
        setAnimateOnSpeech("happy", "HAPPY");
        setAnimateOnSpeech("angry", "ANGRY");

        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        //startKeywordRecognizer();
    }

    public void startKeywordRecognizer() {
        keywordRecognizer.Start();
    }

    public void SetHappyEmotion() {
        setEmotion("HAPPY");
    }

    public void SetSadEmotion() {
        setEmotion("SAD");
    }

    public void SetNeutralEmotion() {
        setEmotion("NEUTRAL");
    }

    public void SetScaredEmotion() {
        setEmotion("SCARED");
    }

    public void SetDanceEmotion() {
        setEmotion("DANCE");
    }

    private void setAnimateOnSpeech(string speechCommand, string animationName) {
        keywords.Add(speechCommand, () =>
        {
            Debug.Log("sound: " + speechCommand + " recognised");
            setEmotion(animationName);
        });
    }

    private void setEmotion(string animationName) {
        if (animator == null) animator = GetComponent<Animator>();
        animator.Play(animationName, -1, 0f);
    }

    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        Debug.Log("command recognised, about to invoke");
        System.Action keywordAction;
        if (keywords.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke();
        }
    }

    private void setEmoScreen(bool value) {
        GameObject EmoScreen = GameObject.Find("EmoScreen");

        if (EmoScreen == null) return;

        EmoScreen.SetActive(value);
    }

    public void DeactiveEmoScreen() {
        setEmoScreen(false);
    }
}
