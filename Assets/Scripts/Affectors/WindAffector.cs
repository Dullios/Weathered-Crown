using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WindAffector : PlayerAffector
{
    // ForceAffector script that exists on the object
    private ForceAffector forces;

    // Size and Offset of BoxCollider2D attached to gameObject
    private Rect windRect;
    private Vector2 windAreaOffset;
    [Tooltip("Prefab object to be instantiated")]
    public GameObject windObject;

    [Tooltip("Pixel width of sprites")]
    public float spriteWidth;
    [Tooltip("Pixel height of sprites")]
    public float spriteHeight;

    // Quarter of the pixel width and height after being converted to unity units
    private float unitWidthQuarter;
    private float unitHeightQuarter;

    // Pooling variables
    [Tooltip("Size of the object pool, if 0 will calculate based on BoxCollider2D area")]
    public int poolSize;
    private int count = 0;
    private List<GameObject> windList = new List<GameObject>();

    private float delay = 0;

    void Start()
    {
        forces = GetComponent<ForceAffector>();
        BoxCollider2D windBox = GetComponent<BoxCollider2D>();

        //windRect = new Rect();
        //windRect.center = windBox.bounds.center;
        //windRect.size = windBox.bounds.size;

        float left = transform.position.x - (GetComponent<BoxCollider2D>().size.x / 2);
        float top = transform.position.y + (GetComponent<BoxCollider2D>().size.y / 2);
        float width = GetComponent<BoxCollider2D>().size.x;
        float height = GetComponent<BoxCollider2D>().size.y;

        windRect = new Rect(left, top, width, height);
        windAreaOffset = new Vector2(gameObject.GetComponent<BoxCollider2D>().offset.x, gameObject.GetComponent<BoxCollider2D>().offset.y);

        // Reposition windRect's x and y with windAreaOffset
        windRect.x += windAreaOffset.x;
        windRect.y += windAreaOffset.y;

        unitWidthQuarter = (spriteWidth / 32) / 4;
        unitHeightQuarter = (spriteHeight / 32) / 4;

        if(poolSize <= 0)
        {
            poolSize = Mathf.RoundToInt((windRect.width * windRect.height) / 150);

            poolSize = poolSize == 0 ? 1 : poolSize;
        }
    }

    void Update()
    {
        //GameObject breeze;

        delay += Time.deltaTime;

        if (delay >= 0.3)
        {
            float xSpawnValue = windRect.center.x - Random.Range(windRect.xMin, windRect.xMax);
            float ySpawnValue = windRect.center.y - Random.Range(windRect.yMax, windRect.yMin);

            if (windList.Count < poolSize)
            {
                if (forces.force.x < 0)
                {
                    GameObject breeze = (GameObject)Instantiate(windObject, new Vector3(xSpawnValue, ySpawnValue, 1), Quaternion.identity);
                    breeze.transform.SetParent(gameObject.transform, true);
                    windList.Add(breeze);
                }
                else if (forces.force.x > 0)
                {
                    GameObject breeze = (GameObject)Instantiate(windObject, new Vector3(xSpawnValue, ySpawnValue, 1), Quaternion.Euler(new Vector3(0, 180, 0)));
                    breeze.transform.SetParent(gameObject.transform, true);
                    windList.Add(breeze);
                }
                else if (forces.force.y > 0)
                {
                    GameObject breeze = (GameObject)Instantiate(windObject, new Vector3(xSpawnValue, ySpawnValue, 1), Quaternion.Euler(new Vector3(0, 0, -90)));
                    breeze.transform.SetParent(gameObject.transform, true);
                    windList.Add(breeze);
                }
            }
            else
            {
                windList[count].transform.localPosition = new Vector3(xSpawnValue, ySpawnValue, 1);

                count++;
                if(count >= windList.Count)
                {
                    count = 0;
                }
            }

            delay = 0;
        }
    }

    //void OnDrawGizmos()
    //{
    //    Start();
    //    // Left
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawSphere((Vector3)windRect.center - new Vector3(windRect.xMin + unitWidthQuarter, 0, 0), 1);

    //    // Bottom
    //    Gizmos.DrawSphere((Vector3)windRect.center - new Vector3(0, windRect.yMin + unitHeightQuarter, 0), 1);

    //    // Right
    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawSphere((Vector3)windRect.center - new Vector3(windRect.xMax - unitWidthQuarter, 0, 0), 1);

    //    // Top
    //    Gizmos.DrawSphere((Vector3)windRect.center - new Vector3(0, windRect.yMax - unitHeightQuarter, 0), 1);
    //}
}
