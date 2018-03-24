using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour {

    public Transform target;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        faceTheObject(target);
    }

    private void faceTheObject(Transform target) {
        Vector3 objectToTargetDir = target.position - transform.position;
        objectToTargetDir.y = 0;

        Vector3 objectForwardAroundY = transform.forward;
        objectForwardAroundY.y = 0;

        float angleToTarget = Vector3.SignedAngle(objectToTargetDir, transform.forward, Vector3.up);

        Vector3 oldRotation = transform.rotation.eulerAngles;
        transform.localRotation = Quaternion.Euler(oldRotation.x, oldRotation.y - angleToTarget, oldRotation.z);
    }
}
