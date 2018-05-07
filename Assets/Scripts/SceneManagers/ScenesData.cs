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
    public static float deadZoneHeight = 0.1f;
    public static float hugZoneHeight = 0.005f;
    public static float zoneBoundary = 0.2f;

    public static float minimumScreenDistance = 2.0f;
    public static float screenSpeed = 1.0f;

    public static int songRate_numberOfWords = 7;
    public static float songRate_perSecond = 4.0f;
    public static float songDuration = 20.0f;
}
