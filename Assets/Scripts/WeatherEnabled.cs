using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeatherEnabled : MonoBehaviour
{
    [Tooltip("GameObject will be enabled on this global weather")]
    public WeatherManager.WeatherType weatherWhenenEnabled;

    [Tooltip("List of GameObjects effected by the global weather type")]
    public List<GameObject> listOfObjects = new List<GameObject>();

	// Update is called once per frame
	void Update ()
    {
        foreach (GameObject go in listOfObjects)
        {
            if (WeatherManager.Instance.type == weatherWhenenEnabled)
            {
                go.SetActive(true);
            }
            else
            {
                go.SetActive(false);
            }
        }
	}
}
