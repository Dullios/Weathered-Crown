using UnityEngine;
using System.Collections;

namespace Tiled2Unity
{
    [RequireComponent(typeof(SpriteRenderer))]

    public class InstanceIcon : MonoBehaviour
    {
        static int sunTrigger = Animator.StringToHash("sun");
        static int rainTrigger = Animator.StringToHash("rain");
        static int snowTrigger = Animator.StringToHash("snow");

        private WeatherManager.WeatherType instType;

        private Animator _animator;
        private Animator animator
        {
            get
            {
                if (_animator == null)
                    _animator = GetComponent<Animator>();

                return _animator;
            }
        }

        // Update is called once per frame
        void Update()
        {
            instType = instType != (WeatherManager.WeatherType)WeatherManager.Instance.instanceTypeNum ? (WeatherManager.WeatherType)WeatherManager.Instance.instanceTypeNum : instType;

            switch (instType)
            {
                case WeatherManager.WeatherType.sun:
                    animator.SetTrigger(sunTrigger);
                    break;
                case WeatherManager.WeatherType.rain:
                    animator.SetTrigger(rainTrigger);
                    break;
                case WeatherManager.WeatherType.snow:
                    animator.SetTrigger(snowTrigger);
                    break;
            }
        }

        public void extendBar(int i)
        {
            gameObject.transform.GetChild(i).gameObject.SetActive(true);
        }
    }
}