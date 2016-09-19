using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IcebergResizer : RaycastController
{
    [Tooltip("Value of scale reached before iceberg is destroyed")]
    [Range(0, 0.99f)]
    public float sizeBeforeDestroy;

    // Incrementing time
    private float tick;

    [HideInInspector]
    public LakeManager lakeScript;

    // Handle entities standing on the shrinking platform
    HashSet<Transform> passengersToMove = new HashSet<Transform>();
    Dictionary<Transform, Controller2D> passengerDictionary = new Dictionary<Transform, Controller2D>();

    // Collider
    BoxCollider2D _collider;
    BoxCollider2D collider
    {
        get
        {
            if (_collider == null)
                _collider = GetComponent<BoxCollider2D>();

            return _collider;
        }
    }

	// Update is called once per frame
	void Update ()
    {
        if (WeatherManager.Instance.checkCurrentWeather(gameObject) != 2)
        {
            UpdateRaycastOrigins();
            CheckForPassengers();

            tick += Time.deltaTime;

            float yBeforeScale = collider.bounds.center.y + collider.bounds.extents.y;

            Vector3 scale = gameObject.transform.localScale;
            scale.x *= (float)(100 - tick) / 100;
            scale.y *= (float)(100 - tick) / 100;
            gameObject.transform.localScale = scale;

            if (gameObject.transform.localScale.x <= sizeBeforeDestroy)
            {
                Destroy(gameObject);
            }
            else
            {
                float deltaDistance = collider.bounds.center.y + collider.bounds.extents.y - yBeforeScale;
                MovePassengers(deltaDistance);
            }
        }
	}

    void MovePassengers(float distance)
    {
        foreach (Transform passenger in passengersToMove)
        {
            if (!passengerDictionary.ContainsKey(passenger))
            {
                var passengerController = passenger.GetComponent<Controller2D>();
                if (passengerController)
                    passengerDictionary.Add(passenger, passengerController);
            }

            passengerDictionary[passenger.transform].Move(new Vector3(0, distance, 0), true);
        }
    }

    void CheckForPassengers()
    {
        // Only need to worry about passengers on top of the iceberg
        float rayLength = 2 * skinWidth;

        passengersToMove.Clear();

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = raycastOrigins.topLeft;
            rayOrigin += Vector2.right * verticalRaySpacing * i;
            RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, Vector2.up, rayLength, collisionMask);

            foreach (var hit in hits)
            {
                if (!passengersToMove.Contains(hit.transform))
                {
                    passengersToMove.Add(hit.transform);
                }
            }
        }
    }
}
