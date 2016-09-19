using UnityEngine;
using System.Collections;

public class PortalManager : MonoBehaviour
{
    [Tooltip("Name of level prefab")]
    public string nextLevel;

    [Tooltip("Set true to load the level with a specific weather, false to use the level's default weather")]
    public bool customWeather;

    [Tooltip("Weather type to load a level with if customWeather is true")]
    public WeatherManager.WeatherType customLevelWeather;

    public enum portalType
    {
        start,
        exit,
        completed
    }

    public portalType typeOfPortal;

    // Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        switch (typeOfPortal)
        {
            case portalType.start:
                switch (customLevelWeather)
                {
                    case WeatherManager.WeatherType.sun:
                        if (HubManager.Instance.sunCompleted)
                        {
                            gameObject.transform.parent.FindChild("ExitPortal Sun Complete").gameObject.SetActive(true);
                            gameObject.SetActive(false);
                        }
                        else
                        {
                            gameObject.transform.parent.FindChild("ExitPortal Sun Complete").gameObject.SetActive(false);
                            gameObject.SetActive(true);
                        }
                        break;
                    case WeatherManager.WeatherType.rain:
                        if (HubManager.Instance.rainCompleted)
                        {
                            gameObject.transform.parent.FindChild("ExitPortal Rain Complete").gameObject.SetActive(true);
                            gameObject.SetActive(false);
                        }
                        else
                        {
                            gameObject.transform.parent.FindChild("ExitPortal Rain Complete").gameObject.SetActive(false);
                            gameObject.SetActive(true);
                        }
                        break;
                    case WeatherManager.WeatherType.snow:
                        if (HubManager.Instance.snowCompleted)
                        {
                            gameObject.transform.parent.FindChild("ExitPortal Snow Complete").gameObject.SetActive(true);
                            gameObject.SetActive(false);
                        }
                        else
                        {
                            gameObject.transform.parent.FindChild("ExitPortal Snow Complete").gameObject.SetActive(false);
                            gameObject.SetActive(true);
                        }
                        break;
                }
                break;
            case portalType.completed:
                switch (customLevelWeather)
                {
                    case WeatherManager.WeatherType.sun:
                        if (!HubManager.Instance.sunCompleted)
                        {
                            gameObject.transform.parent.FindChild("ExitPortal Sun").gameObject.SetActive(true);
                            gameObject.SetActive(false);
                        }
                        break;
                    case WeatherManager.WeatherType.rain:
                        if (!HubManager.Instance.rainCompleted)
                        {
                            gameObject.transform.parent.FindChild("ExitPortal Rain").gameObject.SetActive(true);
                            gameObject.SetActive(false);
                        }
                        break;
                    case WeatherManager.WeatherType.snow:
                        if (!HubManager.Instance.snowCompleted)
                        {
                            gameObject.transform.parent.FindChild("ExitPortal Snow").gameObject.SetActive(true);
                            gameObject.SetActive(false);
                        }
                        break;
                }
                break;
        }
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            switch (typeOfPortal)
            {
                case portalType.exit:
                    switch (customLevelWeather)
                    {
                        case WeatherManager.WeatherType.sun:
                            HubManager.Instance.sunCompleted = true;
                            break;
                        case WeatherManager.WeatherType.rain:
                            HubManager.Instance.rainCompleted = true;
                            break;
                        case WeatherManager.WeatherType.snow:
                            HubManager.Instance.snowCompleted = true;
                            break;
                    }
                    break;
            }

            //if (!customWeather)
            //{
            //    LevelSelect.Instance.LoadLevel(nextLevel);
            //}
            //else
            //{
            //    HubManager.Instance.loadHub();
            //}
            Application.LoadLevel(nextLevel);

            WeatherManager.Instance.pickupCounter = -1;
            WeatherManager.Instance.setInstanceWidth();
        }
    }
}
