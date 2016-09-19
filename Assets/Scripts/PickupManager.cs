/*
 * Written by: Russell
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PickupManager : Singleton<PickupManager>
{
    private Animator pickupAnim;

    private bool hasShined = false;

    private Transform iconGUI;

    public float speed;
    private float lerpVal;

    // Use this for initialization
	void Start ()
    {
        pickupAnim = GetComponent<Animator>();

        iconGUI = FindObjectOfType<Camera>().gameObject.transform.FindChild("Canvas").transform.FindChild("Icon Object");
	}
	
	// Update is called once per frame
	void Update ()
    {
        
	}

    // When touched, pickup is disabled and appropriate property is incremented
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            WeatherManager.Instance.setInstanceWidth();

            HubManager.Instance.pickupCount++;

            StartCoroutine(MoveToIcon());
        }
    }

    IEnumerator MoveToIcon()
    {
        while (lerpVal <= 1)
        {
            Vector3 pos = transform.position;

            pos = Vector3.Lerp(transform.position, iconGUI.position, lerpVal);

            transform.position = pos;

            lerpVal += Time.deltaTime * speed;

            yield return new WaitForFixedUpdate();
        }

        disablePickup();
    }

    void disablePickup()
    {
        gameObject.SetActive(false);
    }

    void OnBecameInvisible()
    {
        hasShined = false;
    }

    void OnBecameVisible()
    {
        if (!hasShined)
        {
            pickupAnim.SetTrigger("isInView");

            hasShined = true;
        }
    }
}