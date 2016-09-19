using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tiled2Unity;

[CustomTiledImporter]
public class DefaultWeatherPropHandler : ICustomTiledImporter {

    public void HandleCustomProperties(GameObject gameObject, IDictionary<string, string> customProperties)
    {
        if (customProperties.ContainsKey("defaultweather"))
        {
            // This was added when the game object was created
            LevelInfo info = gameObject.GetComponent<LevelInfo>();

            // Was not added to map properties
            if (gameObject.transform.parent != null || info == null)
            {
                Debug.LogError("Default weather must be applied to the map properties in tiled: View > Map Properties");
                return;
            }


            if (customProperties["defaultweather"].Equals("Sun"))
            {
                info.defaultWeather = WeatherManager.WeatherType.sun;
            }
            else if (customProperties["defaultweather"].Equals("Rain"))
            {
                info.defaultWeather = WeatherManager.WeatherType.rain;
            }
            else if (customProperties["defaultweather"].Equals("Snow"))
            {
                info.defaultWeather = WeatherManager.WeatherType.snow;
            }
            else
            {
                Debug.LogError("Default weather is set to an invalid value");
            }
        }
    }

    public void CustomizePrefab(GameObject prefab)
    {

    }

}
