using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class ScreenText : MonoBehaviour
{

    public Text textComponent;

    public void SetText(string text) {
        textComponent.text = text;
    }
}
