using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HubManager : Singleton<HubManager>
{
    [Tooltip("Name of the hub scene")]
    public string hubSceneName;
    [Tooltip("Name of the ending scene")]
    public string endingSceneName;

    public bool endLoaded;

    public WeatherManager.WeatherType currentHubWeather = WeatherManager.WeatherType.sun;

    //[HideInInspector]
    public bool sunCompleted = false;
    //[HideInInspector]
    public bool rainCompleted = false;
    //[HideInInspector]
    public bool snowCompleted = false;

    public GameObject crownObject;
    public GameObject exitPortal;

    private bool spawned = false;

    //[HideInInspector]
    public int pickupCount = 0;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        // If a HubManager already exists, destroy this one
        var hubManagers = GameObject.FindObjectsOfType<HubManager>();
        if (hubManagers.Length > 1)
        {
            Destroy(gameObject);
        }

        sunCompleted = true;
    }

	// Update is called once per frame
	void Update ()
    {
	}

    public void OnLevelWasLoaded(int levelInt)
    {
        if (Application.loadedLevelName == hubSceneName)
        {
            if (rainCompleted)
            {
                //LevelSelect.Instance.LoadWeatherLevel("hub_v2", WeatherManager.WeatherType.rain);
                WeatherManager.Instance.type = WeatherManager.WeatherType.rain;
            }
            else if (snowCompleted)
            {
                WeatherManager.Instance.type = WeatherManager.WeatherType.snow;
            }
            else
            {
                WeatherManager.Instance.type = WeatherManager.WeatherType.sun;
            }

            if (sunCompleted && rainCompleted && snowCompleted)
            {
                GameObject crown = (GameObject)Instantiate(crownObject, new Vector3(109, -73.9f, -1), Quaternion.identity);
                GameObject finalPortal = (GameObject)Instantiate(exitPortal, new Vector3(109, -74, 0), Quaternion.identity);

                crown.transform.SetParent(FindObjectOfType<LevelInfo>().gameObject);
                finalPortal.transform.SetParent(FindObjectOfType<LevelInfo>().gameObject);

                finalPortal.GetComponent<PortalManager>().nextLevel = "End Screen";
                finalPortal.GetComponent<PortalManager>().typeOfPortal = PortalManager.portalType.exit;
            }
        }
    }
}
