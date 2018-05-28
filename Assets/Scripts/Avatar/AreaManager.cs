﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HoloToolkit.Unity;
using System;

public class AreaManager : MonoBehaviour {

    public GameObject hugZone;
    public GameObject deadZone;
    public Transform headTransform;
    public GameObject childModel;
    public GameObject warningDialog;
    public HeadsUpDirectionIndicator directionIndicator;
    public Camera mainCamera;

    public float Distance { get; private set; }

    private bool shouldDeactivateDeadZone = false;

    public bool InDeadZone {
        get {
            return Distance < ScenesData.deadZoneRadius;
        }
    }

    public bool InHugZone {
        get
        {
            return !InDeadZone && Distance < ScenesData.hugZoneRadius;
        }
    }

    // Use this for initialization
    void Start () {
        float hugRadius = (ScenesData.hugZoneRadius + ScenesData.zoneBoundary) * 2; 
        float deadRadius = (ScenesData.deadZoneRadius + ScenesData.zoneBoundary) * 2;

        deadZone.transform.localScale = new Vector3(deadRadius, ScenesData.deadZoneHeight, deadRadius);
        hugZone.transform.localScale = new Vector3(hugRadius, ScenesData.hugZoneHeight, hugRadius);

        ResetPosition();
    }
	
	// Update is called once per frame
	void Update () {
        updateDistance();

        childModel.SetActive(!InDeadZone);
        warningDialog.SetActive(InDeadZone);
        if (InDeadZone && !deadZone.active) {
            showDeadZone();
            shouldDeactivateDeadZone = true;
        }

        if (!InDeadZone && shouldDeactivateDeadZone) {
            hideDeadZone();
            shouldDeactivateDeadZone = false;
        }

        if (Math.Abs(Vector3.Angle(transform.position, headTransform.forward)) < 40.0f)
        {
            directionIndicator.HideIndicator();
        }
        else {
            directionIndicator.ShowIndicator();
        }

	}

    private void updateDistance() {
        Vector3 userPositionXZ = headTransform.position;
        userPositionXZ.y = 0;

        Vector3 childPositionXZ = gameObject.transform.position;
        childPositionXZ.y = 0;

        Distance = Vector3.Distance(userPositionXZ, childPositionXZ);
    }

    public void showHugZone() {
        hugZone.SetActive(true);
    }

    public void hideHugZone() {
        hugZone.SetActive(false);
    }

    public void showDeadZone()
    {
        deadZone.SetActive(true);
    }

    public void hideDeadZone()
    {
        deadZone.SetActive(false);
    }

    public void showZones() {
        showDeadZone();
        showHugZone();
    }

    public void hideZones() {
        hideDeadZone();
        hideHugZone();
    }

    public void ResetPosition()
    {
        if (LocationManager.Instance.ChildLocation == null) return;
        transform.position = LocationManager.Instance.ChildLocation;

        hugZone.transform.position = LocationManager.Instance.ChildLocation;

        Vector3 temp = LocationManager.Instance.ChildLocation;
        temp.y += 0.01f;
        deadZone.transform.position = temp;
    }
}
