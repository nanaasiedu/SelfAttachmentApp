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
    public static float zoneBoundary = 0.2f;

    public static float minimumScreenDistance = 2.0f;
    public static float screenSpeed = 1.0f;

    public static int songRate_numberOfWords = 3;
    public static float songRate_perSecond = 2.5f;
    public static float songDuration = 10.0f;

    public static int pledgeRate_numberOfWords = 4;
    public static float pledgeRate_perSecond = 3.0f;
    public static float pledgeDuration = 10.0f;

    public static int phraseDetectionLookAhead = 2;
    public static float phraseDetectionPercentage = 0.6f;

    private static int numberOfScans = 2;
    public static float scanningDuration = 3.5f * numberOfScans + 1.0f;
    public static int minimumHorizontalSurfaces = 1;
    public static int minimumVerticalSurfaces = 1;

    public static float childStartDistance = 2.0f;
    public static float childHeightOffset = -0.1f;
    public static float childStartPositionCheckAngle = 360.0f / 16.0f;
    public static float floorHitAllowance = 0.2f;
    public static float childMidHeight = 0.6f + 0.4f;
    public static float childMinDistanceToWall = 0.8f;
}
