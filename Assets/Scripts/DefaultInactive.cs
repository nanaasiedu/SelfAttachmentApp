using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultInactive : MonoBehaviour {

    void Awake() {
        gameObject.SetActive(false);
    }
}
