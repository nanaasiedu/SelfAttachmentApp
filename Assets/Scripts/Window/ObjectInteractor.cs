using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInteractor : MonoBehaviour, Interactible {

    public Material defaultMaterial;
    public Material hoverMaterial;
    public Material selectMaterial;

    private Renderer renderer;
    private bool isFocused;

	// Use this for initialization
	void Start () {
        renderer = gameObject.GetComponent<Renderer>();
        renderer.material = defaultMaterial;
        isFocused = false;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void GazeEntered() {
        renderer.material = hoverMaterial;
        isFocused = true;
    }

    public void GazeExited() {
        renderer.material = defaultMaterial;
        isFocused = false;
    }

    public void OnSelect() {
        renderer.material = selectMaterial;
    }

    public void OffSelect() {
        unSelectMaterial();
    }

    public void Deselect() {
        unSelectMaterial();
    }

    private void unSelectMaterial() {
        renderer.material = isFocused ? hoverMaterial : defaultMaterial;
    }
}
