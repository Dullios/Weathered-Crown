using UnityEngine;
using System.Collections;
using BehaviourMachine;

public class WeatherBehaviour
{
    public abstract class WeatherConditionNode : ConditionNode
    {
        [VariableInfo(canBeConstant = true, nullLabel = "Use Self", requiredField = false)]
        public GameObjectVar _gameObject;
        public GameObjectVar gameObject
        {
            get
            {
                if(_gameObject.isNone)
                {
                    return self;
                }
                else
                {
                    return _gameObject.Value;
                }
            }

            set
            {
                _gameObject = value;
            }
        }

        public WeatherManager.WeatherType type;
    }

    [NodeInfo(category = "Weather/Condition/")]
    public class CheckCurrentWeather : WeatherConditionNode
    {
        public override Status Update()
        {

            if (WeatherManager.Instance.checkCurrentWeather(gameObject) == (int)type)
            {
                if (onSuccess.id != 0)
                    owner.root.SendEvent(onSuccess.id);

                return Status.Success;
            }

            return Status.Failure;
        }
    }
}
