// Kurtis Thiessen - June 2

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tiled2Unity;

[CustomTiledImporter]
public class AffectorPropHandler : ICustomTiledImporter {

    public void HandleCustomProperties(GameObject gameObject, IDictionary<string, string> customProperties)
    {
        // Friction Affector
        if (customProperties.ContainsKey("grounddrag") ||
            customProperties.ContainsKey("airdrag") ||
            customProperties.ContainsKey("maxspeed") ||
            customProperties.ContainsKey("sprintmult"))
        {
            HandleFrictionProperties(gameObject, customProperties);
        }

        // Gravity Affector
        if (customProperties.ContainsKey("jumpheight") &&
            customProperties.ContainsKey("timetoapex"))
        {
            HandleGravityProperties(gameObject, customProperties);
        }
        // One is set and the other is not, return an error
        else if (customProperties.ContainsKey("jumpheight") !=
                 customProperties.ContainsKey("timetoapex"))
        {
            Debug.LogError("Only on gravity affector property is set. You must set both or neither");
        }

        // Force Affector
        if (customProperties.ContainsKey("force:x") ||
            customProperties.ContainsKey("force:y"))
        {
            HandleForceProperties(gameObject, customProperties);
        }
    }

    private void HandleFrictionProperties(GameObject gameObject, IDictionary<string, string> customProperties)
    {
        FrictionAffector friction = gameObject.AddComponent<FrictionAffector>();

        // Ground Drag
        if (customProperties.ContainsKey("grounddrag"))
        {
            float val = -1;
            try
            {
                val = (float)System.Convert.ToDouble(customProperties["grounddrag"]);
            }
            catch (System.FormatException)
            {
                val = -1;
                Debug.LogError("grounddrag property formatted improperly");
            }

            if (val >= 0)
                friction.groundDrag = val;
        }

        // Air Drag
        if (customProperties.ContainsKey("airdrag"))
        {
            float val = -1;
            try
            {
                val = (float)System.Convert.ToDouble(customProperties["airdrag"]);
            }
            catch (System.FormatException)
            {
                val = -1;
                Debug.LogError("airdrag property formatted improperly");
            }

            if (val >= 0)
                friction.airDrag = val;
        }
        
        // Move Speed
        if (customProperties.ContainsKey("movespeed"))
        {
            float val = -1;
            try
            {
                val = (float)System.Convert.ToDouble(customProperties["movespeed"]);
            }
            catch (System.FormatException)
            {
                val = -1;
                Debug.LogError("movespeed property formatted improperly");
            }

            if (val >= 0)
                friction.maxMoveSpeed = val;
        }
        
        // Sprint Multiplier
        if (customProperties.ContainsKey("sprintmult"))
        {
            float val = -1;
            try
            {
                val = (float)System.Convert.ToDouble(customProperties["sprintmult"]);
            }
            catch (System.FormatException)
            {
                val = -1;
                Debug.LogError("sprintmult property formatted improperly");
            }

            if (val >= 0)
                friction.sprintMult = val;
        }

    }

    private void HandleGravityProperties(GameObject gameObject, IDictionary<string, string> customProperties)
    {
        GravityAffector gravity = gameObject.AddComponent<GravityAffector>();

        // Jump Height
        if (customProperties.ContainsKey("jumpheight"))
        {
            float val = -1;
            try
            {
                val = (float)System.Convert.ToDouble(customProperties["jumpheight"]);
            }
            catch (System.FormatException)
            {
                val = -1;
                Debug.LogError("jumpheight property formatted improperly");
            }

            if (val >= 0)
                gravity.jumpHeight = val;
        }

        // Time to Apex
        if (customProperties.ContainsKey("timetoapex"))
        {
            float val = -1;
            try
            {
                val = (float)System.Convert.ToDouble(customProperties["timetoapex"]);
            }
            catch (System.FormatException)
            {
                val = -1;
                Debug.LogError("timetoapex property formatted improperly");
            }

            if (val >= 0)
                gravity.timeToApex = val;
        }
    }

    private void HandleForceProperties(GameObject gameObject, IDictionary<string, string> customProperties)
    {
        ForceAffector force = gameObject.AddComponent<ForceAffector>();

        // X Force
        if (customProperties.ContainsKey("force:x"))
        {
            float val = -1;
            try
            {
                val = (float)System.Convert.ToDouble(customProperties["force:x"]);
            }
            catch (System.FormatException)
            {
                val = -1;
                Debug.LogError("force:x property formatted improperly");
            }

            force.force.x = val;
        }

        // Y Force
        if (customProperties.ContainsKey("force:y"))
        {
            float val = -1;
            try
            {
                val = (float)System.Convert.ToDouble(customProperties["force:y"]);
            }
            catch (System.FormatException)
            {
                val = -1;
                Debug.LogError("force:y property formatted improperly");
            }

            force.force.y = val;
        }
    }

    public void CustomizePrefab(GameObject prefab)
    {
        // Do Nothing
    }
}
