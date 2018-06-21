using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class PhqManager : MonoBehaviour {

    public Text questionText;
    public GameObject questionnaireParent;
    //public GameObject keyInformation; 

    private string[] questions = new string[] {
            "q",
            "q",
            "q",
            "q",
            "q",
            "q",
            "q" };

    private static int numberOfQuestions = 10;

    void Start () {
        questionnaireParent.SetActive(false);
	}

    public void StartQuestionnaire() {
        questionnaireParent.SetActive(true);
    }

}
