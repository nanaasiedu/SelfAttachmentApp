using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HoloToolkit.Unity.SpatialMapping;

public class DestroyDuplicateProcessing : MonoBehaviour {
    public void Awake()
    {
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }
    }
}
