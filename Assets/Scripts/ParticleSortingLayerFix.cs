using UnityEngine;
using System.Collections;

public class ParticleSortingLayerFix : MonoBehaviour {

    public string layer;

	// Use this for initialization
	void Start () {
        this.GetComponent<Renderer>().sortingLayerName = layer;
	}
}
