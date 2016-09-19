using UnityEngine;
using System.Collections;

// Forces the camera to not move outside the boundaries of the current level
public class WorldBounds : CameraBehaviourBase
{
    Rect worldRect;

    void Start()
    {
        CalculateWorldBounds();
    }

    void CalculateWorldBounds()
    {
        Bounds bounds;

        //var tiledMap = transform.root.GetComponentInChildren<Tiled2Unity.TiledMap>();//GameObject.FindObjectOfType<Tiled2Unity.TiledMap>();

        //worldRect = new Rect(
        //    tiledMap.transform.position.x,
        //    tiledMap.transform.position.y - tiledMap.GetMapHeightInPixelsScaled(),
        //    tiledMap.GetMapWidthInPixelsScaled(),
        //    tiledMap.GetMapHeightInPixelsScaled());

        //Collider2D[] allColliders = GameObject.FindObjectsOfType<Collider2D>();
        Collider2D[] allColliders = transform.root.GetComponentsInChildren<Collider2D>();
        if (allColliders.Length > 0)
            bounds = allColliders[0].bounds;
        else
            return;

        for (int i = 1; i < allColliders.Length; i++)
        {
            bounds.Encapsulate(allColliders[i].bounds);
        }

        worldRect = new Rect(
            bounds.center.x - bounds.extents.x,
            bounds.center.y - bounds.extents.y,
            bounds.size.x,
            bounds.size.y);
    }

    public override void Evaluate()
    {
        Rect cameraRect = new Rect(
            camera.transform.position.x - camera.orthographicSize * camera.aspect,
            camera.transform.position.y - camera.orthographicSize,
            camera.orthographicSize * camera.aspect * 2,
            camera.orthographicSize * 2);

        Vector3 shift = Vector3.zero;
        // Vertical
        if (cameraRect.yMin < worldRect.yMin || cameraRect.height >= worldRect.height)  // Bottom
            shift.y = worldRect.yMin - cameraRect.yMin;
        else if (cameraRect.yMax > worldRect.yMax)  // Top
            shift.y = worldRect.yMax - cameraRect.yMax;

        // Horizontal
        if (cameraRect.width > worldRect.width)
            shift.x = worldRect.center.x - cameraRect.center.x;
        else if (cameraRect.xMin < worldRect.xMin)       // Left
            shift.x = worldRect.xMin - cameraRect.xMin;
        else if (cameraRect.xMax > worldRect.xMax)  // Right
            shift.x = worldRect.xMax - cameraRect.xMax;

        transform.Translate(shift);
    }
}
