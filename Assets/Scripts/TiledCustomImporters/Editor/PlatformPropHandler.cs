using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tiled2Unity;

[CustomTiledImporter]
public class PlatformPropHandler : ICustomTiledImporter {

    public void HandleCustomProperties(GameObject gameObject, IDictionary<string, string> customProperties)
    {
        // One Way Platform Tag
        if (customProperties.ContainsKey("one-way"))
        {
            gameObject.tag = "One-Way";
            var collision = gameObject.transform.Find("Collision");
            if (collision)
                collision.tag = "One-Way";
        }

        // Moving Platform
        if (customProperties.ContainsKey("movingplatform"))
        {
            EdgeCollider2D path = gameObject.GetComponent<EdgeCollider2D>();
            if (path != null)
            {
                var platform = gameObject.AddComponent<PlatformWaypointController>();
                platform.localWaypoints = System.Array.ConvertAll<Vector2, Vector3>(path.points, p => (Vector3)p);

                platform.collisionMask = LayerMask.GetMask("Player");

                platform.verticalRayCount = (int)platform.collider.size.x * 2;
                platform.horizontalRayCount = (int)platform.collider.size.y * 2;

                // Platform parameters
                if (customProperties.ContainsKey("speed"))
                {
                    platform.speed = (float)System.Convert.ToDouble(customProperties["speed"]);
                }
                else
                    platform.speed = 1;

                if (customProperties.ContainsKey("repeat"))
                {
                    platform.cyclic = System.Convert.ToBoolean(customProperties["repeat"]);
                }
                else
                    platform.cyclic = true;
            }
            MonoBehaviour.DestroyImmediate(path);
        }
    }

    public void CustomizePrefab(GameObject prefab)
    {
    }

}
