using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.Buttons;

public class ButtonInteractor : MonoBehaviour, Interactible
{

    public GameObject targetObject;
    public string targetMethod;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void GazeEntered()
    {
        Button buttonComponent = gameObject.GetComponent<Button>();

        if (buttonComponent != null) {
            buttonComponent.OnStateChange(ButtonStateEnum.ObservationTargeted);
        }
        
    }

    public void GazeExited()
    {
        Button buttonComponent = gameObject.GetComponent<Button>();

        if (buttonComponent != null)
        {
            buttonComponent.OnStateChange(ButtonStateEnum.Observation);
        }
    }

    public void OnSelect()
    {
        Button buttonComponent = gameObject.GetComponent<Button>();

        if (buttonComponent != null)
        {
            buttonComponent.OnStateChange(ButtonStateEnum.Pressed);
        }
    }

    public void OffSelect()
    {
        Button buttonComponent = gameObject.GetComponent<Button>();

        if (buttonComponent != null)
        {
            buttonComponent.OnStateChange(ButtonStateEnum.Observation);
        }

        if (targetObject != null) {
            targetObject.SendMessage(targetMethod);
        }
    }

    public void Deselect()
    {
        Button buttonComponent = gameObject.GetComponent<Button>();

        if (buttonComponent != null)
        {
            buttonComponent.OnStateChange(ButtonStateEnum.Observation);
        }
    }
}
