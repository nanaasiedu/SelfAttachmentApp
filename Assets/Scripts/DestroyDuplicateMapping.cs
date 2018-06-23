using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyDuplicateMapping : MonoBehaviour {
    public void Awake()
    {
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }
    }
}
