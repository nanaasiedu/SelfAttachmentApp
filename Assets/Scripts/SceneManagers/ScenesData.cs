using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ScenesData {

    public enum ProtocolType {
        NONE,
        INTRO_SAT,
        SINGING,
        EMBRACE,
        MASSAGE,
        DARK_ROOM
    }

    public static ProtocolType currentProtocol = ProtocolType.NONE;
}
