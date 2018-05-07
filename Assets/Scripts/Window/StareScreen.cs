using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StareScreen : MonoBehaviour {

    public GameObject userCamera;
    public int screenMovementSpeed = 2;
    public bool hasDistanceLimit = false;

    private Vector3 originalPosition;
    private static float extra_proximity = 0.2f;

	// Use this for initialization
	void Start () {
        originalPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        faceUser();
        enforceDistanceLimit();
    }

    private void faceUser() {
        Vector3 targetDir = userCamera.transform.position - transform.position;

        // The step size is equal to speed times frame time.
        float step = screenMovementSpeed * Time.deltaTime;

        Vector3 newDir = Vector3.RotateTowards(transform.forward, -targetDir, step, 0.0f);

        // Move our position a step closer to the target.
        transform.rotation = Quaternion.LookRotation(newDir);
    }

    private void enforceDistanceLimit() {
        if (!hasDistanceLimit) return;

        Vector3 userPosXZ = userCamera.transform.position;
        userPosXZ.y = 0;
        Vector3 screenPosXZ = transform.position;
        screenPosXZ.y = 0;

        float distance = Vector3.Distance(userPosXZ, screenPosXZ);
        float step = ScenesData.screenSpeed * Time.deltaTime;

        if (distance < ScenesData.minimumScreenDistance)
        {
            Vector3 newScreenPosXZ = Vector3.MoveTowards(screenPosXZ, userPosXZ, -step);
            newScreenPosXZ.y = transform.position.y;
            transform.position = newScreenPosXZ;
        }
        else if (distance > ScenesData.minimumScreenDistance + extra_proximity && screenOffOriginal())
        {
            Vector3 newScreenPosXZ = Vector3.MoveTowards(screenPosXZ, userPosXZ, step);
            newScreenPosXZ.y = transform.position.y;
            transform.position = newScreenPosXZ;
        }
    }

    private bool screenOffOriginal() {
        Vector3 userPosXZ = userCamera.transform.position;
        userPosXZ.y = 0;
        Vector3 screenPosXZ = transform.position;
        screenPosXZ.y = 0;
        Vector3 origPosXZ = originalPosition;
        origPosXZ.y = 0;

        return Vector3.Distance(screenPosXZ, userPosXZ) > Vector3.Distance(origPosXZ, userPosXZ);
    }
}
