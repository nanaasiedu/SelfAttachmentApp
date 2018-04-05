using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Interactible {
    void GazeEntered();

    void GazeExited();

    void OnSelect();

    void OffSelect();

    void Deselect();
}
