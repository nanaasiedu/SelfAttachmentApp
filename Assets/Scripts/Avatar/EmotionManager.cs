﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class EmotionManager : MonoBehaviour {

    public Animator animator;

    KeywordRecognizer keywordRecognizer = null;
    Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        
        setAnimateOnSpeech("sad", "SAD");
        setAnimateOnSpeech("happy", "HAPPY");
        setAnimateOnSpeech("angry", "ANGRY");

        // Tell the KeywordRecognizer about our keywords.
        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());

        // Register a callback for the KeywordRecognizer and start recognizing!
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void setAnimateOnSpeech(string speechCommand, string animationName) {
        keywords.Add(speechCommand, () =>
        {
            Debug.Log("sound: " + speechCommand + " recognised");
            animator.Play(animationName, -1, 0f);
        });
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
}
