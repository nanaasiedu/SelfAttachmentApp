using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using HoloToolkit.Unity;

public class AnimatedText : MonoBehaviour {
    private Text textArea;
    public string[] strings;
    public float speed = ScenesData.dialogTextSpeed;
    public TextToSpeech textToSpeech;

    public int NumOfText {
        get { return strings.Length; }
    }

    public bool TextFinished {
       get { return stringIndex == strings.Length - 1; }
    }

    private bool started = false;
    private int stringIndex = 0;
    private int charIndex = 0;

	// Use this for initialization
	void Start () {
        if (stringIndex < strings.Length) playTextAudio(strings[stringIndex]);
        textArea = GetComponentInChildren<Text>();
        StartCoroutine(OutputTextSegment());

    }

    void OnEnable()
    {
        textArea = GetComponentInChildren<Text>();
        StartCoroutine(OutputTextSegment());
    }

    IEnumerator OutputTextSegment() {
        while (stringIndex < strings.Length) {
            yield return new WaitForSeconds(speed);

            if (stringIndex < strings.Length && charIndex > strings[stringIndex].Length) {
                continue;
            }

            textArea.text = strings[stringIndex].Substring(0, charIndex);
            charIndex++;
        }
    }

    void AdvanceText() {
        if (stringIndex >= strings.Length - 1) {
            return;
        }

        stringIndex++;
        charIndex = 0;

        if (stringIndex < strings.Length) playTextAudio(strings[stringIndex]);
    }

    void playTextAudio(string text) {
        textToSpeech.StartSpeaking(text);
    }
}
