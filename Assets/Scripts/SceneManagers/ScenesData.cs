using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ScenesData {

    public enum ProtocolType {
        NONE,
        STAGE_1,
        STAGE_2,
        STAGE_3,
        STAGE_4
    }

    public static ProtocolType currentProtocol = ProtocolType.NONE;

    public static float dialogTextSpeed = 0.03f;

    public static float deadZoneRadius = 0.8f;
    public static float hugZoneRadius = 1.3f;
    public static float deadZoneHeight = 0.01f;
    public static float hugZoneHeight = 0.005f;
    public static float zoneBoundary = -0.1f;

    public static float minimumScreenDistance = 2.0f;
    public static float screenSpeed = 1.0f;

    public static int songRate_numberOfWords = 3;
    public static float songRate_perSecond = 2.5f;
    public static float songDuration = 7.0f;//10.0f;

    public static int pledgeRate_numberOfWords = 4;
    public static float pledgeRate_perSecond = 3.0f;
    public static float pledgeDuration = 10.0f;

    public static int phraseDetectionLookAhead = 2;
    public static float phraseDetectionPercentage = 0.6f;

    private static int numberOfScans = 5;
    public static float scanningDuration = 2.5f * numberOfScans;
    public static int minimumHorizontalSurfaces = 1;
    public static int minimumVerticalSurfaces = 1;

    public static float childStartDistance = 1.5f;
    public static float childHeightOffset = -0.15f;
    public static float childStartPositionCheckAngle = 360.0f / 32.0f;
    public static float floorHitAllowance = 0.2f;
    public static float childMidHeight = 0.6f - 0.3f;
    public static float childMinDistanceToWall = 0.5f;

    public static float childHeight = 0.9f;
    public static float offGroundBombStartHeight = 0.3f;
    public static float bombScanStartDistance = 0.01f;
    public static int numberOfBombScans = 12;
    public static float bombScanSeperation = 0.02f;//(bombScanCheckDistance - bombScanStartDistance) / (numberOfBombScans - 1);
    public static float bombScanCheckAngle = 360.0f / 8.0f;
    //public static float bombScanCheckDistance = 10f;

    public static float carpetHeightOffset = 0.05f;
    public static int numberOfFloorObjects = 20;
    public static float plantScale = 0.1f;
    public static float carpetScale = 0.5f;
    public static float defaultCeilingHeight = 3f;
    public static float chandelierHeight = 0.73f + 0.05f;
    public static float chandelierScale = 0.41f;

    public static float potStartDistance = 1f;
    public static float potTestSeperation = 0.5f;
    public static float potEndDistance = 4f;
    public static float minTableY = 0.5f;
    public static float potTableOffset = 0.005f;

    public static float wallScanHeight = 2.5f;
    public static float portraitScale = 0.01f;

}
