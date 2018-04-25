using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassageManager : MonoBehaviour {

    public GameObject child;

    void Awake() {
        enabled = ScenesData.currentProtocol == ScenesData.ProtocolType.MASSAGE;
    }
	
	void Start () {
        child.SendMessage("DeactiveEmoScreen");
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
