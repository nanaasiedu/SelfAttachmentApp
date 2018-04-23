using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class AnimatedText : MonoBehaviour {
    public Text textArea;
    public string[] strings;
    public float speed = 0.1f;

    int stringIndex = 0;
    int charIndex = 0;

	// Use this for initialization
	void Start () {
        StartCoroutine(DisplayTimer());
	}

    IEnumerator DisplayTimer() {
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
