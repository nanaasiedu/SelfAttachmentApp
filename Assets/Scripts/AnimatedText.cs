using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class AnimatedText : MonoBehaviour {
    public Text textArea;
    public string[] strings;
    public float speed = ScenesData.dialogTextSpeed;

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
        StartCoroutine(OutputTextSegment());
	}

    void OnEnable()
    {
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
    }
}
