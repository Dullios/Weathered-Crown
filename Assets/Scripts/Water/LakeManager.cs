using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Created following this tutorial:
 * http://gamedevelopment.tutsplus.com/tutorials/creating-dynamic-2d-water-effects-in-unity--gamedev-14143
 */

public class LakeManager : MonoBehaviour
{
    int edgeCount;
    int nodeCount;

    float[] xPositions;
    float[] yPositions;
    float[] yEdgePositions;
    float[] velocities;
    float[] accelerations;

    float[] leftDeltas;
    float[] rightDeltas;

    Vector3[] vertices;

    GameObject[] meshObjects;
    Mesh[] meshes;

    [Tooltip("Width of the texture used (pixel width divided by pixels per unit")]
    public float textureWidth;
    // Number of mesh nodes per texture tile
    int edgesPerTextureTile;

    //[Tooltip("Delay of scrolling texture")]
    //public float edgeScrollTick = 1;

    GameObject[] colliders;

    const float springConstant = 0.02f;
    public float damping = 0.04f;
    private float spread = 0.05f;
    const float z = 5f;

    // Water drawn from left to right, left being the object's X value
    private float left;
    [Tooltip("How deep the water is")]
    public float depth;
    [Tooltip("Depth of the perspective top of the water")]
    public float perspectiveDepth;
    [Tooltip("How wide the water is starting from the object's position")]
    public float width;

    [Tooltip("Splash prefab to be instantiated")]
    public GameObject splash;

    private float setDepth;

    public Material waterMat;
    public GameObject waterMesh;
    public Material iceMat;

    public WeatherManager.WeatherType type;
    public WeatherManager.WeatherType prevType;

    [HideInInspector]
    public bool instanceRunning = false;
    [HideInInspector]
    public WeatherManager.WeatherType currentInstType;

    private GameObject ice;

    public GameObject iceberg;
    public int icebergSize;

    public GameObject player
    {
        get
        {
            return GameObject.FindObjectOfType<PlayerAffectorListener>().gameObject;
        }
    }

	// Use this for initialization
	void Start ()
    {
        edgeCount = Mathf.RoundToInt(width) * 5;
        nodeCount = edgeCount + 1;
        edgesPerTextureTile = (int)(textureWidth * 5);
        
        SpawnWater();

        WeatherManager.Instance.lakeList.Add(this);

        prevType = (WeatherManager.WeatherType)(-1);

        StartCoroutine(WaitForInstanceCast());

        //StartCoroutine(scrollTexture());
	}

	public void SpawnWater()
    {
        // initialize variables
        xPositions = new float[nodeCount];
        yPositions = new float[nodeCount];
        yEdgePositions = new float[nodeCount];
        velocities = new float[nodeCount];
        accelerations = new float[nodeCount];

        leftDeltas = new float[xPositions.Length];
        rightDeltas = new float[xPositions.Length];

        meshObjects = new GameObject[edgeCount];
        meshes = new Mesh[edgeCount];
        colliders = new GameObject[edgeCount];

        vertices = new Vector3[6];

        left = transform.position.x;
        setDepth = depth;
        depth = transform.position.y - depth;

        // fill arrays
        for(int i = 0; i < nodeCount; i++)
        {
            xPositions[i] = left + width * i / edgeCount;
            yPositions[i] = transform.position.y;
            yEdgePositions[i] = transform.position.y - perspectiveDepth;
            accelerations[i] = 0;
            velocities[i] = 0;
        }

        // creating meshes
        for (int i = 0; i < edgeCount; i++)
        {
            meshes[i] = new Mesh();

            vertices = new Vector3[6];
            vertices[0] = new Vector3(xPositions[i], yEdgePositions[i], z);
            vertices[1] = new Vector3(xPositions[i + 1], yEdgePositions[i + 1], z);
            vertices[2] = new Vector3(xPositions[i], depth, z);
            vertices[3] = new Vector3(xPositions[i + 1], depth, z);
            vertices[4] = new Vector3(xPositions[i], yPositions[i], z);
            vertices[5] = new Vector3(xPositions[i + 1], yPositions[i + 1], z);

            if(i >= edgeCount - 5)
            {
                vertices[4] = new Vector3(xPositions[i], yPositions[i] - 0.2f * (5 - (edgeCount - i)), z);
                vertices[5] = new Vector3(xPositions[i + 1], yPositions[i + 1] - 0.2f * (5 - (edgeCount - (i + 1))), z);
            }

            float uvXStart = (i % edgesPerTextureTile) / (float)(edgesPerTextureTile);
            float uvXEnd = (i + 1) % edgesPerTextureTile / (float)(edgesPerTextureTile);
            // Change end UV to 1 when 0 to wrap texture tile
            uvXEnd = uvXEnd == 0 ? 1 : uvXEnd;

            Vector2[] UVs = new Vector2[6];
            UVs[0] = new Vector2(uvXStart, 0.68f);
            UVs[1] = new Vector2(uvXEnd, 0.68f);
            UVs[2] = new Vector2(uvXStart, 0);
            UVs[3] = new Vector2(uvXEnd, 0);
            UVs[4] = new Vector2(uvXStart, 1);
            UVs[5] = new Vector2(uvXEnd, 1);

            if (i >= edgeCount - 5)
            {
                UVs[4] = new Vector2(uvXStart, (1 - 0.075f * (5 - (edgeCount - i))));
                UVs[5] = new Vector2(uvXEnd, (1 - 0.075f * (5 - (edgeCount - (i + 1)))));
            }

            int[] tris = new int[12] { 0, 1, 3, 3, 2, 0, 4, 5, 1, 1, 0, 4 };

            meshes[i].vertices = vertices;
            meshes[i].uv = UVs;
            meshes[i].triangles = tris;

            meshObjects[i] = (GameObject)Instantiate(waterMesh, Vector3.zero, Quaternion.identity);
            meshObjects[i].GetComponent<MeshFilter>().mesh = meshes[i];
            meshObjects[i].GetComponent<MeshRenderer>().material.renderQueue = -10;
            meshObjects[i].transform.parent = transform;
            meshObjects[i].layer = 4;
            
            // collision setup
            colliders[i] = new GameObject();
            colliders[i].name = "Trigger";
            colliders[i].layer = LayerMask.NameToLayer("Default");
            colliders[i].AddComponent<Rigidbody2D>();
            colliders[i].GetComponent<Rigidbody2D>().isKinematic = true;
            colliders[i].AddComponent<BoxCollider2D>();
            colliders[i].transform.parent = transform;
            colliders[i].transform.position = new Vector3((float)(left + width * (i + 0.5f) / edgeCount), (transform.position.y - (colliders[i].GetComponent<BoxCollider2D>().size.y / 2)) - 0.37f, 0);
            colliders[i].transform.localScale = new Vector3(width / edgeCount, 1, 1);
            colliders[i].GetComponent<BoxCollider2D>().isTrigger = true;
            colliders[i].AddComponent<WaterDetector>();
        }
    }

    public void castInstance(bool instance, WeatherManager.WeatherType instType)
    {
        instanceRunning = instance;
        currentInstType = instType;
    }

	void Update ()
    {
        bool isInView = false;

        foreach(Transform t in transform)
        {
            if(t.name == "Water Mesh(Clone)")
            {
                if(t.GetComponent<MeshRenderer>().isVisible)
                {
                    isInView = true;
                    break;
                }
            }
        }

        if (isInView)
        {
            // Check Player's position compared to the water to determine render order
            if (gameObject.transform.position.y - 0.5f < player.transform.position.y)
            {
                foreach (Transform t in transform)
                {
                    if (t.name == "Water Mesh(Clone)")
                    {
                        t.GetComponent<MeshRenderer>().sortingLayerName = "Ice";
                    }
                }
            }
            else
            {
                foreach (Transform t in transform)
                {
                    if (t.name == "Water Mesh(Clone)")
                    {
                        t.GetComponent<MeshRenderer>().sortingLayerName = "Foreground";
                    }
                }
            }

            // dampening factor
            for (int i = 0; i < xPositions.Length; i++)
            {
                float force = springConstant * (yPositions[i] - transform.position.y) + velocities[i] * damping;
                accelerations[i] = -force;
                yPositions[i] += velocities[i];
                yEdgePositions[i] += velocities[i];
                velocities[i] += accelerations[i];
            }

            // wave propagation
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < xPositions.Length; j++)
                {
                    // Water is frozen
                    if (type == WeatherManager.WeatherType.snow && !instanceRunning)
                    {
                        velocities[j] = 0;
                        accelerations[j] = 0;

                        yPositions[j] = transform.position.y;
                        yEdgePositions[j] = transform.position.y - perspectiveDepth;
                    }
                    else
                    {
                        if (j > 0)
                        {
                            leftDeltas[j] = spread * (yPositions[j] - yPositions[j - 1]);
                            velocities[j - 1] += leftDeltas[j];
                        }

                        if (j < xPositions.Length - 1)
                        {
                            rightDeltas[j] = spread * (yPositions[j] - yPositions[j + 1]);
                            velocities[j + 1] += rightDeltas[j];
                        }

                        // Prevent the first and end points from moving
                        if (j == 0)
                        {
                            velocities[j] = 0;
                        }
                        for (int k = 1; k <= 5; k++)
                        {
                            velocities[xPositions.Length - k] = 0;
                        }
                    }
                }
            }

            for (int i = 0; i < xPositions.Length; i++)
            {
                if (i > 0)
                {
                    yPositions[i - 1] += leftDeltas[i];
                    yEdgePositions[i - 1] += leftDeltas[i];
                }

                if (i < xPositions.Length - 1)
                {
                    yPositions[i + 1] += rightDeltas[i];
                    yEdgePositions[i + 1] += rightDeltas[i];
                }
            }
        }

        type = WeatherManager.Instance.type;
        #region Weather Change Effect
        if (type != prevType)
        {
            if(type == WeatherManager.WeatherType.sun)
            {
                for (int i = 0; i < edgeCount; i++)
                {
                    colliders[i].GetComponent<BoxCollider2D>().isTrigger = true;
                }

                if(prevType == WeatherManager.WeatherType.snow)
                {
                    foreach (GameObject g in meshObjects)
                    {
                        g.GetComponent<MeshRenderer>().material = waterMat;
                    }
                }

                prevType = type;
            }
            else if(type == WeatherManager.WeatherType.rain)
            {
                for (int i = 0; i < edgeCount; i++)
                {
                    colliders[i].GetComponent<BoxCollider2D>().isTrigger = true;
                }

                if (prevType == WeatherManager.WeatherType.snow)
                {
                    foreach (GameObject g in meshObjects)
                    {
                        g.GetComponent<MeshRenderer>().material = waterMat;
                    }
                }

                prevType = type;
            }
            else if(type == WeatherManager.WeatherType.snow)
            {
                foreach (GameObject g in meshObjects)
                {
                    g.GetComponent<MeshRenderer>().material = iceMat;
                }

                for (int i = 0; i < edgeCount; i++)
                {
                    colliders[i].GetComponent<BoxCollider2D>().isTrigger = false;
                }

                prevType = type;
            }
        }
        #endregion
        
        if(instanceRunning && currentInstType == WeatherManager.WeatherType.sun && type == WeatherManager.WeatherType.snow)
        {
            foreach(GameObject go in colliders)
            {
                if(WeatherManager.Instance.withinInstanceWeather(go))
                {
                    go.GetComponent<BoxCollider2D>().isTrigger = true;
                }
            }

            foreach (GameObject go in meshObjects)
            {
                if (WeatherManager.Instance.withinInstanceWeather(go.GetComponent<MeshFilter>().sharedMesh.vertices[0].x))
                {
                    go.GetComponent<MeshRenderer>().material = waterMat;
                }
            }

            // Stop frozen water outside sun weather zone from waving
            for(int i = 0; i < xPositions.Length - 1; i++)
            {
                if(!WeatherManager.Instance.withinInstanceWeather(xPositions[i]))
                {
                    velocities[i] = 0;
                }
            }
        }
        else if (!instanceRunning && type == WeatherManager.WeatherType.snow)
        {
            foreach (GameObject go in colliders)
            {
                go.GetComponent<BoxCollider2D>().isTrigger = false;
            }

            foreach(GameObject go in meshObjects)
            {
                go.GetComponent<MeshRenderer>().material = iceMat;
            }
        }

        UpdateMeshes();

        //scrollTexture();
	}

    IEnumerator WaitForInstanceCast()
    {
        while(!WeatherManager.Instance.instanceIsRunning)
        {
            yield return null;
        }

        if (WeatherManager.Instance.instanceIsRunning && currentInstType == WeatherManager.WeatherType.snow && ice == null)
        {
            int count = 0;
            List<GameObject> collidersInInstance = new List<GameObject>();

            foreach (GameObject go in colliders)
            {
                if (WeatherManager.Instance.withinInstanceWeather(go))
                {
                    count++;
                    collidersInInstance.Add(go);
                }
            }

            if (count >= icebergSize && icebergSize > 0)
            {
                float x = collidersInInstance[(int)(collidersInInstance.Count / 2)].transform.position.x;
                float y = collidersInInstance[(int)(collidersInInstance.Count / 2)].transform.position.y;

                ice = (GameObject)Instantiate(iceberg, new Vector3(x, y, 0), Quaternion.identity);

                ice.GetComponent<IcebergResizer>().lakeScript = this;
            }
        }

        StartCoroutine(WaitForInstanceEnd());
    }

    IEnumerator WaitForInstanceEnd()
    {
        while(WeatherManager.Instance.instanceIsRunning)
        {
            yield return null;
        }

        StartCoroutine(WaitForInstanceCast());
    }

    // update as water moves
    void UpdateMeshes()
    {
        for(int i = 0; i < meshes.Length; i++)
        {
            
            vertices[0] = new Vector3(xPositions[i], yEdgePositions[i], z);
            vertices[1] = new Vector3(xPositions[i + 1], yEdgePositions[i + 1], z);
            vertices[2] = new Vector3(xPositions[i], depth, z);
            vertices[3] = new Vector3(xPositions[i + 1], depth, z);
            vertices[4] = new Vector3(xPositions[i], yPositions[i], z);
            vertices[5] = new Vector3(xPositions[i + 1], yPositions[i + 1], z);

            if (i >= edgeCount - 5)
            {
                vertices[4] = new Vector3(xPositions[i], yPositions[i] - 0.2f * (5 - (edgeCount - i)), z);
                vertices[5] = new Vector3(xPositions[i + 1], yPositions[i + 1] - 0.2f * (5 - (edgeCount - (i + 1))), z);
            }

            meshes[i].vertices = vertices;
        }
    }

    IEnumerator scrollTexture()
    {
        float uvXStart = meshes[0].uv[0].x;
        float uvXEnd = meshes[0].uv[1].x;

        float uvIncrement = (uvXEnd - uvXStart);


        Vector2[] prevMeshUV = meshes[0].uv;

        while (true)
        {
            //yield return new WaitForSeconds(edgeScrollTick);
            yield return null;


            for (int i = 0; i < prevMeshUV.Length; i++)
            {
                prevMeshUV[i].x -= uvIncrement;
                if (prevMeshUV[i].x < 0 || (Mathf.Approximately(prevMeshUV[i].x, 0) && i % 2 == 1)) prevMeshUV[i].x += 1;
            }

            for (int i = 0; i < meshes.Length; i++)
            {
                Vector2[] thisUV = meshes[i].uv;
                meshes[i].uv = prevMeshUV;
                prevMeshUV = thisUV;
            }
        }
    }

    public void Splash(float xPos, float velocity)
    {
        if(xPos >= xPositions[0] && xPos <= xPositions[xPositions.Length - 1])
        {
            xPos -= xPositions[0];

            int index = Mathf.RoundToInt((xPositions.Length - 1) * (xPos / (xPositions[xPositions.Length - 1] - xPositions[0])));

            if(setDepth <= 3 && velocity != 0)
            {
                velocity /= 2;
            }

            velocities[index] = velocity;

            float lifetime = 0.93f + Mathf.Abs(velocity) * 0.07f;
            splash.GetComponent<ParticleSystem>().startSpeed = 8 + 2 * Mathf.Pow(Mathf.Abs(velocity), 0.5f);
            splash.GetComponent<ParticleSystem>().startSpeed = 9 + 2 * Mathf.Pow(Mathf.Abs(velocity), 0.5f);
            splash.GetComponent<ParticleSystem>().startLifetime = lifetime;

            // direct particles away from splashing on land
            Vector3 position = new Vector3(xPositions[index], yPositions[index] - 0.35f, 5);
            Quaternion rotation = Quaternion.LookRotation(new Vector3(xPositions[Mathf.FloorToInt(xPositions.Length / 2)], transform.position.y + 8, 5) - position);

            GameObject splish = (GameObject)Instantiate(splash, position, rotation);
            Destroy(splish, lifetime + 0.3f);
        }
    }

    void OnDestroy()
    {
        WeatherManager.Instance.lakeList.Remove(this);
    }

    //public void OnTriggerStay2D(Collider2D hit)
    //{
    //    float force;

    //    if(hit.gameObject.tag == "Strider")
    //    {
    //        force = 100 * (springConstant * 0.45f) - (hit.GetComponent<Rigidbody2D>().velocity.y * (damping + 0.04f));
    //        hit.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, force), ForceMode2D.Impulse);
    //    }
    //}
}
