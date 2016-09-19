using UnityEngine;
using System.Collections;

public class FreeParallaxDemo : MonoBehaviour
{
    public FreeParallax parallax;
    public GameObject cloud;

    Vector3 cameraPos;
    Camera mainCamera
    {
        get
        {
            return GameObject.FindObjectOfType<Camera>();
        }
    }

    // Use this for initialization
    void Start()
    {
        if (cloud != null)
        {
            cloud.GetComponent<Rigidbody2D>().velocity = new Vector2(0.1f, 0.0f);
        }

        cameraPos = mainCamera.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = mainCamera.transform.position;

        if (parallax != null)
        {
            //if (Input.GetKey(KeyCode.LeftArrow))
            //{
            //    parallax.Speed = 15.0f;
            //}
            //else if (Input.GetKey(KeyCode.RightArrow))
            //{
            //    parallax.Speed = -15.0f;
            //}
            if (Mathf.Approximately(pos.x, cameraPos.x))
            {
                parallax.Speed = 0.0f;
            }
            else if (pos.x < cameraPos.x)
            {
                parallax.Speed = 15.0f;
            }
            else
            {
                parallax.Speed = -15.0f;
            }

            cameraPos = pos;
        }
    }
}
