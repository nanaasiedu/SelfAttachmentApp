using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StareScreen : MonoBehaviour {

    public GameObject userCamera;
    public int screenMovementSpeed = 2; 

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 targetDir = userCamera.transform.position - transform.position;

        // The step size is equal to speed times frame time.
        float step = screenMovementSpeed * Time.deltaTime;

        Vector3 newDir = Vector3.RotateTowards(transform.forward, -targetDir, step, 0.0f);

        // Move our position a step closer to the target.
        transform.rotation = Quaternion.LookRotation(newDir);
    }
}
