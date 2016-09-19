//----------------------------------------------
//            Behaviour Machine
// Copyright © 2014 Anderson Campos Cardoso
//----------------------------------------------

using UnityEngine;
using System.Collections;

namespace BehaviourMachine {

    public delegate void OnUpdateDelegate();
    public delegate void OnLateUpdateDelegate();

    /// <summary>
    /// Wrapper class for the InternalBehaviourTree component.
    /// <summary>
    [RequireComponent(typeof(Blackboard))]
    public class BehaviourTree : InternalBehaviourTree {

        public event OnUpdateDelegate onUpdateEvent;
        public event OnLateUpdateDelegate onLateUpdateEvent;

        void Update()
        {
            if (!enabled) return;

            if (onUpdateEvent != null)
                onUpdateEvent();
        }

        void LateUpdate()
        {
            if (!enabled) return;

            if (onLateUpdateEvent != null)
                onLateUpdateEvent();
        }

    }
}