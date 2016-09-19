using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Camera))]
public class CameraBehaviourComponent : MonoBehaviour {

    GameObject _targetObj;
    public GameObject targetObj
    {
        get
        {
            if (_targetObj == null)
                _targetObj = GameObject.FindObjectOfType<PlayerAffectorListener>().gameObject;

            return _targetObj;
        }
    }

    public List<CameraBehaviourBase> behaviours = new List<CameraBehaviourBase>();

    void Start()
    {
        behaviours.AddRange(GetComponents<CameraBehaviourBase>());
        behaviours.Sort((x, y) => x.priority.CompareTo(y.priority));
    }

    void Update()
    {
        if (targetObj == null)
            return;

        foreach (var b in behaviours)
        {
            b.Evaluate();
        }
    }
}
