using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PickupCounter : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        GetComponent<Text>().text = ": " + HubManager.Instance.pickupCount;
	}
}
