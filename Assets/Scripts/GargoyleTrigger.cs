using UnityEngine;
using System.Collections;

public class GargoyleTrigger : MonoBehaviour
{
    
    public GameObject gargoylePrefab;
    public GameObject gargoyleSpawn;

    private bool hasSpawn = false;

	void OnTriggerEnter2D(Collider2D col)
    {
        if(!hasSpawn && col.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            GameObject gargoyle = (GameObject)Instantiate(gargoylePrefab, gargoyleSpawn.transform.position, Quaternion.identity);
            gargoyle.transform.SetParent(gameObject.transform.parent);
            
            hasSpawn = true;
        }
    }
}
