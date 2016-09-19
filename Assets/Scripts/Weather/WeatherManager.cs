using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Tiled2Unity;

public class WeatherManager : Singleton<WeatherManager>
{
    public enum WeatherType
    {
        sun = 0,
        rain = 1,
        snow = 2
    }

    public WeatherType type;

    public GameObject player
    {
        get
        {
            return GameObject.FindObjectOfType<PlayerAffectorListener>().gameObject;
        }
    }

    // List of weatherTile scripts
    public List<WeatherTile> weatherTileList = new List<WeatherTile>();

    // List of LakeManager scripts
    public List<LakeManager> lakeList = new List<LakeManager>();

    private int _instanceTypeNum = 0;
    public int instanceTypeNum
    {
        set
        {
            // Causes the value to loop between [0, 2]
            int rVal = (value % 3 + 3) % 3;

            // Instance weather cannot be global weather
            if (rVal == (int)type)
            {
                rVal += (int)Mathf.Sign(value - _instanceTypeNum);

                // Enforce range after changing value
                rVal = (rVal % 3 + 3) % 3;
            }

            // Return
            _instanceTypeNum = rVal;
        }
        get
        {
            if (_instanceTypeNum == (int)type)
                _instanceTypeNum = ++_instanceTypeNum % 3;

            return _instanceTypeNum;
        }
    }

    private int activeInstNum;

    public GameObject instanceObstruction;
    public GameObject leftBorderParticles;
    public GameObject rightBorderParticles;
    public GameObject instanceRain;
    public GameObject instanceSnow;
    
    public int width;
    [HideInInspector]
    public Rect instanceRectArea = new Rect();
    public float duration;

    public int pickupCounter = 0;
    public int pickupRatio;

    public bool instanceIsRunning = false;

    public void SetWeatherType(int i)
    {
        type = (WeatherType)i - 1;
    }

    void Awake()
    {
        instanceRectArea.center = new Vector2(Mathf.Round(player.transform.position.x / 2) * 2, player.transform.position.y);
        instanceRectArea.width = width;
    }

    void Update()
    {}

    public void Activate()
    {
        if (!instanceIsRunning)
        {
            instanceIsRunning = true;

            instanceRectArea.center = new Vector2(Mathf.Round(player.transform.position.x / 2) * 2, player.transform.position.y);
            instanceRectArea.width = width;

            StartCoroutine(InstanceTimeOut());
            //BehaviourMachine.GlobalBlackboard.Instance.GetGameObjectVar("Player").Value.GetComponent<BehaviourMachine.StateMachine>().SendEventUpwards("CAST_INSTANCE");
        }
    }

    public void instanceDistribution(bool active, int instTypeNum)
    {
        // Set Collider size
        Vector3 colliderSize = instanceObstruction.GetComponent<BoxCollider>().size;
        instanceObstruction.GetComponent<BoxCollider>().size = new Vector3(width, colliderSize.y, colliderSize.z);

        // Calculate y position
        float height = GameObject.FindGameObjectWithTag("globalParticle").transform.position.y - 2;

        // Set Collider position
        Vector3 colliderPosition = instanceObstruction.transform.position;
        instanceObstruction.transform.position = new Vector3(player.transform.position.x, height, colliderPosition.z);

        Vector3 particleSize = new Vector3(width, 1, 1);
        Vector3 particlePos = new Vector3(instanceRectArea.center.x, 68, 0);
        Vector3 leftBorder = new Vector3(instanceRectArea.xMin, player.transform.position.y, - 1);
        Vector3 rightBorder = new Vector3(instanceRectArea.xMax, player.transform.position.y, - 1);

        leftBorderParticles.transform.position = leftBorder;
        rightBorderParticles.transform.position = rightBorder;

        instanceObstruction.SetActive(active);
        leftBorderParticles.SetActive(active);
        rightBorderParticles.SetActive(active);

        switch (instTypeNum)
        {
            case 1:
                instanceRain.transform.position = particlePos + new Vector3(0, 0, instanceRain.transform.position.z);
                instanceRain.transform.localScale = particleSize;
                instanceRain.SetActive(active);
                break;
            case 2:
                instanceSnow.transform.position = particlePos + new Vector3(0, 0, instanceSnow.transform.position.z);
                instanceSnow.transform.localScale = particleSize;
                instanceSnow.SetActive(active);
                break;
        }
    }

    // Coroutine to wait for duration before recalling the castInstance method to change everything back
    IEnumerator InstanceTimeOut()
    {
        activeInstNum = instanceTypeNum;

        instanceDistribution(true, activeInstNum);

        foreach (WeatherTile wt in weatherTileList)
        {
            wt.castInstance(instanceRectArea, activeInstNum, true);
        }

        foreach (LakeManager lm in lakeList)
        {
            lm.castInstance(true, (WeatherManager.WeatherType)activeInstNum);
        }

        yield return new WaitForSeconds(duration);

        instanceDistribution(false, activeInstNum);

        foreach (WeatherTile wt in weatherTileList)
        {
            wt.castInstance(instanceRectArea, activeInstNum, false);
        }

        foreach (LakeManager lm in lakeList)
        {
            lm.castInstance(false, (WeatherManager.WeatherType)activeInstNum);
        }

        instanceIsRunning = false;
    }

    public void NextInstance()
    {
        instanceTypeNum++;
        if (instanceTypeNum == (int)type)
            Debug.LogError("Weather changing was wrong :(");
    }

    public void PrevInstance()
    {
        instanceTypeNum--;
        if (instanceTypeNum == (int)type)
            Debug.LogError("Weather changing was wrong :(");
    }

    public void setInstanceWidth()
    {
        pickupCounter++;

        if(pickupCounter < pickupRatio * 1)
        {
            width = 13;
        }
        else if(pickupCounter < pickupRatio * 2)
        {
            width = 17;
            FindObjectOfType<InstanceIcon>().extendBar(1);
        }
        else if (pickupCounter < pickupRatio * 3)
        {
            width = 21;
            FindObjectOfType<InstanceIcon>().extendBar(2);
        }
        else if (pickupCounter < pickupRatio * 4)
        {
            width = 25;
            FindObjectOfType<InstanceIcon>().extendBar(3);
        }
    }

    // Return true if inside instance weather
    public bool withinInstanceWeather(GameObject go)
    {
        bool inInstance = false;

        if (instanceIsRunning && go.transform.position.x > instanceRectArea.xMin && go.transform.position.x < instanceRectArea.xMax)
        {
            inInstance = true;
        }

        return inInstance;
    }

    public bool withinInstanceWeather(float f)
    {
        bool inInstance = false;

        if(instanceIsRunning && f > instanceRectArea.xMin && f < instanceRectArea.xMax)
        {
            inInstance = true;
        }

        return inInstance;
    }

    public int checkCurrentWeather(GameObject go)
    {
        int weatherType = -1;

        if (instanceIsRunning && go.transform.position.x > instanceRectArea.xMin && go.transform.position.x < instanceRectArea.xMax)
        {
            weatherType = activeInstNum;
        }
        else
        {
            weatherType = (int)type;
        }

        return weatherType;
    }

    // Overloaded as particles are not game objects
    public int checkCurrentWeather(ParticleSystem.Particle p)
    {
        int weatherType = -1;

        if (instanceIsRunning && p.position.x > instanceRectArea.xMin && p.position.x < instanceRectArea.xMax)
        {
            weatherType = activeInstNum;
        }
        else
        {
            weatherType = (int)type;
        }

        return weatherType;
    }

    public static int getWeatherLayer(int type)
    {
        return getWeatherLayer((WeatherType)type);
    }

    public static int getWeatherLayer(WeatherManager.WeatherType type)
    {
        int rVal;
        switch (type)
        {
            case WeatherManager.WeatherType.sun:
                rVal = 1 << LayerMask.NameToLayer("Sun");
                break;
            case WeatherManager.WeatherType.rain:
                rVal = 1 << LayerMask.NameToLayer("Rain");
                break;
            case WeatherManager.WeatherType.snow:
                rVal = 1 << LayerMask.NameToLayer("Snow");
                break;
            default:
                rVal = 0;
                break;
        }

        return rVal;
    }

    void OnDeserialized()
    {
        weatherTileList.Clear();

        WeatherTile[] TileList = FindObjectsOfType<WeatherTile>();
        TileList = TileList[0].gameObject.GetComponents<WeatherTile>();

        foreach(WeatherTile tile in TileList)
        {
            weatherTileList.Add(tile);
        }

        if (instanceIsRunning)
        {
            StartCoroutine(InstanceTimeOut());
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        Gizmos.DrawRay(new Vector3(instanceRectArea.xMin, 100, 0), Vector3.down * 200);
        Gizmos.DrawRay(new Vector3(instanceRectArea.xMax, 100, 0), Vector3.down * 200);
    }
}