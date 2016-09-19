using UnityEngine;
using System.Collections;

/*
 * Written by: Russell Brabers
 */

public class GeyserManager : MonoBehaviour
{
    [Tooltip("Number of geyser prefabs used")]
    public int geyserHeight;

    [Tooltip("Prefab for the geyser")]
    public GameObject geyserPrefab;

    private Sprite geyserSprite;

    [Tooltip("List of prefabs based on the geyserHeight")]
    private GameObject[] prefabList;

    [Tooltip("Rate at which the geyser raises and lowers")]
    [Range(0.0f, 1.0f)]
    public float speed;

    [Tooltip("Time between geyser rising and lowering")]
    public float delay;

    // Whether or not the geyser is rising or lowering
    private bool rising = false;

    public float timePast = 0;
    private int index = 0;

	// Use this for initialization
	void Start ()
    {
	    prefabList = new GameObject[geyserHeight];
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!rising)
        {
            timePast += Time.deltaTime;
        }

        if (timePast >= delay && !rising)
        {
            timePast = 0;
            rising = true;

            for(int i = 0; i < prefabList.Length; i++)
            {
                int numChildren = gameObject.transform.childCount;
                Vector3 startingPos = Vector3.zero;

                if (numChildren != 0)
                {
                    startingPos = gameObject.transform.GetChild(numChildren - 1).transform.localPosition;
                }

                prefabList[i] = (GameObject)Instantiate(geyserPrefab, startingPos, Quaternion.identity);
                prefabList[i].SetParent(gameObject);

                Vector2 objectSize = prefabList[i].GetComponent<BoxCollider2D>().size;

                prefabList[i].transform.position = Vector3.Lerp(prefabList[i].transform.position, new Vector3(0, prefabList[i].transform.position.y + (objectSize.y / 2), 0), Time.deltaTime * speed);
            }
        }
	}

    void spawnBlock()
    {
        int numChildren = gameObject.transform.childCount;
        Vector3 startingPos = numChildren != 0 ? gameObject.transform.GetChild(numChildren - 1).transform.localPosition : Vector3.zero;

        prefabList[index] = (GameObject)Instantiate(geyserPrefab, startingPos, Quaternion.identity);
        prefabList[index].SetParent(gameObject);
    }

    IEnumerator raise(GameObject geyserBlock)
    {
        yield return null;
    }
}