using UnityEngine;
using System.Collections;
using BehaviourMachine;

public class BehaviourMachineExtraMethods : MonoBehaviour {

    [NodeInfo(category="Action/GameObject/", icon="GameObject")]
    public class FindGameObjectByComponent : ActionNode
    {
        [VariableInfo(canBeConstant = true, requiredField = true)]
        public StringVar component;

        [VariableInfo(canBeConstant=true, requiredField=true)]
        public GameObjectVar storeGameObject;

        public override Status Update()
        {
            storeGameObject.Value = (FindObjectOfType(System.Type.GetType(component.Value)) as Component).gameObject;
            if (storeGameObject.Value != null)
                return Status.Success;
            else
                return Status.Failure;
        }
    }

    [NodeInfo(category="Action/GameObject/", icon="GameObject")]
    public class SetLayerByName : ActionNode
    {
        [VariableInfo(canBeConstant = true, requiredField = false, nullLabel = "Use Self")]
        public GameObjectVar gameObject;

        [VariableInfo(canBeConstant=true, requiredField=true)]
        public StringVar layerName;

        GameObject target
        {
            get
            {
                if (gameObject.isNone)
                    return self;
                else
                    return gameObject.Value;
            }
        }

        public override Status Update()
        {
            int layerInt = LayerMask.NameToLayer(layerName.Value);

            target.layer = layerInt;

            return Status.Success;
        }

    }

    [NodeInfo(category="Action/Blackboard/")]
    public class Vector3MultiplyXYZ : ActionNode
    {
        [VariableInfo(canBeConstant=false, requiredField=true)]
        public Vector3Var variable;

        [VariableInfo(canBeConstant=true, requiredField=false, nullLabel="Don't Use")]
        public FloatVar x, y, z;

        public override Status Update()
        {
            Vector3 v = variable.Value;
            if (!x.isNone && !x.isInvalid)
            {
                v.x *= x;
            }

            if (!y.isNone && !y.isInvalid)
            {
                v.y *= y;
            }

            if (!z.isNone && !z.isInvalid)
            {
                v.z *= z;
            }

            variable.Value = v;

            return Status.Success;
        }
    }

    [NodeInfo(category = "Action/Blackboard/")]
    public class Vector3DivideXYZ : ActionNode
    {
        [VariableInfo(canBeConstant = false, requiredField = true)]
        public Vector3Var variable;

        [VariableInfo(canBeConstant = true, requiredField = false, nullLabel = "Don't Use")]
        public FloatVar x, y, z;

        public override Status Update()
        {
            Vector3 v = variable.Value;
            if (!x.isNone && !x.isInvalid)
            {
                v.x /= x;
            }

            if (!y.isNone && !y.isInvalid)
            {
                v.y /= y;
            }

            if (!z.isNone && !z.isInvalid)
            {
                v.z /= z;
            }

            variable.Value = v;

            return Status.Success;
        }
    }

    [NodeInfo(category="Action/Blackboard/")]
    public class Vector3ClampXYZ : ActionNode
    {
        [VariableInfo(canBeConstant = false, requiredField = true)]
        public Vector3Var variable;

        public bool x, y, z;

        [VariableInfo(canBeConstant=true, requiredField=true)]
        public FloatVar min, max;

        public override Status Update()
        {
            Vector3 v = variable.Value;

            if (x)
            {
                v.x = Mathf.Clamp(v.x, min, max);
            }

            if (y)
            {
                v.y = Mathf.Clamp(v.y, min, max);
            }

            if (z)
            {
                v.z = Mathf.Clamp(v.z, min, max);                
            }

            variable.Value = v;

            return Status.Success;
        }
    }

    [NodeInfo(category="Action/Transform/", icon="Transform")]
    public class LookAt2D : ActionNode
    {
        [VariableInfo(canBeConstant=true, requiredField=false, nullLabel="Use Self")]
        public GameObjectVar gameObject;

        [VariableInfo(canBeConstant=true, requiredField=true)]
        public Vector3Var direction;

        public Space space;

        [VariableInfo(canBeConstant = true, requiredField = false, nullLabel = "Do Not Smooth")]
        public BoolVar smooth;

        GameObject target
        {
            get
            {
                if (gameObject.isNone)
                    return self;
                else
                    return gameObject.Value;
            }
        }

        public override Status Update()
        {
            Vector2 lookAtTarget = direction.Value;
            if (space == Space.World)
                lookAtTarget = target.transform.worldToLocalMatrix * lookAtTarget;

            float angle = Mathf.Atan2(lookAtTarget.y, lookAtTarget.x) * Mathf.Rad2Deg;

            Quaternion desired = Quaternion.AngleAxis(angle, Vector3.forward);
            if (!smooth.isNone && smooth.Value)
            {
                target.transform.rotation = Quaternion.Slerp(target.transform.rotation, desired, 0.05f);
            }
            else
                target.transform.rotation = desired;

            return Status.Success;
        }
    }

    [NodeInfo(category = "Action/Transform/", icon = "Transform")]
    public class GetDirectionTo2D : ActionNode
    {
        [VariableInfo(canBeConstant = true, requiredField = false, nullLabel = "Use Self")]
        public GameObjectVar A;

        [VariableInfo(canBeConstant = true, requiredField = false, nullLabel = "Use Self")]
        public GameObjectVar B;

        [VariableInfo(canBeConstant=false, requiredField=true)]
        public FloatVar storeDirection;

        GameObject targetA
        {
            get
            {
                if (A.isNone)
                    return self;
                else
                    return A.Value;
            }
        }

        GameObject targetB
        {
            get
            {
                if (B.isNone)
                    return self;
                else
                    return B.Value;
            }
        }

        public override Status Update()
        {
            Vector2 dirVec = targetB.transform.position - targetA.transform.position;
            storeDirection.Value = Mathf.Sign(dirVec.x);

            return Status.Success;
        }
    }

    [NodeInfo(category="Action/Animator/")]
    public class SetSpeed : ActionNode
    {
        [VariableInfo(canBeConstant=true, requiredField=false, nullLabel="Use Self")]
        public GameObjectVar gameObject;

        [VariableInfo(canBeConstant=true, requiredField=true)]
        public FloatVar newSpeed;

        GameObject target
        {
            get
            {
                if (gameObject.isNone)
                    return self;
                else
                    return gameObject.Value;
            }
        }

        public override Status Update()
        {
            Animator anim = target.GetComponent<Animator>();
            anim.speed = newSpeed.Value;

            return Status.Success;
        }
    }

    [NodeInfo(category="Action/Blackboard/", icon="Blackboard")]
    public class ColorLerp : ActionNode
    {
        [VariableInfo(canBeConstant=true, requiredField=true)]
        public ColorVar storeColor, startColor, endColor;

        [VariableInfo(canBeConstant=true, requiredField=true)]
        public FloatVar timer;

        public override Status Update()
        {
            storeColor.Value = Color.Lerp(startColor.Value, endColor.Value, timer);

            return Status.Success;
        }
    }

    [NodeInfo(category = "Condition/Physics2D/")]
    public class CheckForCollision : ConditionNode
    {
        [VariableInfo(canBeConstant = false, requiredField = false, nullLabel = "Do Not Store")]
        public GameObjectVar storeCollision;

        public LayerMask layer;

        BoxCollider2D _collider;
        BoxCollider2D collider
        {
            get
            {
                if (_collider == null)
                    _collider = self.GetComponent<BoxCollider2D>();

                return _collider;
            }
        }

        public override Status Update()
        {
            Collider2D col = Physics2D.OverlapArea(
                collider.bounds.center - collider.bounds.extents,
                collider.bounds.center + collider.bounds.extents,
                layer);

            if (col)
            {
                if (onSuccess.id != 0)
                    owner.root.SendEvent(onSuccess.id);
                if (storeCollision.id != 0)
                    storeCollision.Value = col.gameObject;

                return Status.Success;
            }

            return Status.Failure;
        }
    }

    [NodeInfo(category = "Decorator/Physics2D/")]
    public class CheckForCollisions : DecoratorNode
    {
        [VariableInfo(canBeConstant = false, requiredField = true)]
        public GameObjectVar storeCollision;

        public LayerMask layer;

        BoxCollider2D _collider;
        BoxCollider2D collider
        {
            get
            {
                if (_collider == null)
                    _collider = self.GetComponent<BoxCollider2D>();

                return _collider;
            }
        }

        public override Status Update()
        {
            if (child == null)
                return Status.Error;

            Collider2D[] colliders = Physics2D.OverlapAreaAll(
                collider.bounds.center - collider.bounds.extents,
                collider.bounds.center + collider.bounds.extents,
                layer);

            foreach (var col in colliders)
            {
                storeCollision.Value = col.gameObject;

                child.OnTick();
                if (child.status == Status.Success)
                {
                    return Status.Success;
                }
                if (child.status == Status.Error || child.status == Status.Running)
                {
                    return child.status;
                }
            }

            return Status.Failure;
        }
    }

    [NodeInfo(category = "Action/Transform/", icon = "Transform")]
    public class ReflectPosition : ActionNode
    {
        [VariableInfo(canBeConstant = true, requiredField = false, nullLabel = "Use Self")]
        public GameObjectVar gameObject;

        public Vector3Component Axis = Vector3Component.x;

        GameObject target
        {
            get
            {
                if (gameObject.isNone)
                    return self;
                else
                    return gameObject.Value;
            }
        }

        public override Status Update()
        {
            Vector3 pos = target.transform.localPosition;

            switch (Axis)
            {
                case Vector3Component.x:
                    {
                        pos.x *= -1;
                        break;
                    }
                case Vector3Component.y:
                    {
                        pos.y *= -1;
                        break;
                    }
                case Vector3Component.z:
                    {
                        pos.z *= -1;
                        break;
                    }
            }

            target.transform.localPosition = pos;

            return Status.Success;
        }
    }

    [NodeInfo(category="Function/", icon="Function")]
    public class UpdatePause : FunctionNode
    {
        [VariableInfo(canBeConstant=true, requiredField=false, nullLabel="Use TimeScale")]
        public BoolVar pause;

        bool running
        {
            get
            {
                if (pause.id != 0)
                    return !pause.Value;
                else
                    return Time.timeScale > 0;
            }
        }

        BehaviourTree myTree
        {
            get
            {
                return this.tree as BehaviourTree;
            }
        }


        public override void OnEnable()
        {
            if (this.enabled)
            {
                myTree.onUpdateEvent += OnUpdateCallback;

                m_Registered = true;
            }
        }

        public override void OnDisable()
        {
            myTree.onUpdateEvent -= OnUpdateCallback;

            m_Registered = false;
        }

        public override void Reset()
        {
            if (this.enabled)
            {
                
            }

            base.Reset();
        }

        void OnUpdateCallback()
        {
            if (running || this.tree.ignoreTimeScale)
            {
                this.OnTick();
            }
        }

    }

    [NodeInfo(category = "Function/", icon = "Function")]
    public class LateUpdatePause : FunctionNode
    {
        [VariableInfo(canBeConstant = true, requiredField = false, nullLabel = "Use TimeScale")]
        public BoolVar pause;

        bool running
        {
            get
            {
                if (pause.id != 0)
                    return !pause.Value;
                else
                    return Time.timeScale > 0;
            }
        }

        BehaviourTree myTree
        {
            get
            {
                return this.tree as BehaviourTree;
            }
        }


        public override void OnEnable()
        {
            if (this.enabled)
            {
                myTree.onLateUpdateEvent += OnLateUpdateCallback;

                m_Registered = true;
            }
        }

        public override void OnDisable()
        {
            myTree.onLateUpdateEvent -= OnLateUpdateCallback;

            m_Registered = false;
        }

        void OnLateUpdateCallback()
        {
            if (running || this.tree.ignoreTimeScale)
            {
                this.OnTick();
            }
        }

    }

    public class MovePlatformBelow : ActionNode
    {
        public Vector3Var velocity;

        public BoolVar smooth;

        Collider2D _collider;
        Collider2D collider
        {
            get
            {
                if (_collider == null)
                    _collider = self.GetComponent<Collider2D>();

                return _collider;
            }
        }

        Vector3 velDamp;
        PlatformController oldPlatform;

        public override Status Update()
        {
            Vector2 rayOrigin = collider.bounds.center - new Vector3(0, collider.bounds.extents.y - Controller2D.skinWidth * 5);
            float dist = .5f;

            RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, -Vector2.up, dist, 1 << LayerMask.NameToLayer("Default"));

            foreach (var hit in hits)
            {
                PlatformController platform = hit.collider.GetComponent<PlatformController>();
                if (platform == null) continue;

                if (platform != oldPlatform)
                {
                    velDamp = Vector3.zero;
                    if (oldPlatform != null)
                    {
                        StopPlatform(oldPlatform);
                    }
                }
                oldPlatform = platform;

                if (smooth.Value)
                    platform.move = Vector3.SmoothDamp(platform.move, velocity.Value, ref velDamp, .5f);
                else
                    platform.move = velocity.Value;

                return Status.Success;
            }

            if (oldPlatform != null)
            {
                StopPlatform(oldPlatform);
                oldPlatform = null;
            }

            return Status.Success;
        }

        public override void OnDisable()
        {
            if (oldPlatform != null)
            {
                StopPlatform(oldPlatform);
                oldPlatform = null;
            }
        }
        void StopPlatform(PlatformController platform)
        {
            if (smooth.Value)
                platform.StartCoroutine(SmoothStop(platform, velDamp));
            else
                platform.move = Vector3.zero;
        }

        IEnumerator SmoothStop(PlatformController platform, Vector3 velDamp)
        {
            Vector3 vel = velDamp;
            Vector3 oldVel = platform.move;

            while (platform.move != Vector3.zero)
            {
                if (platform.move != oldVel) break;

                platform.move = Vector3.SmoothDamp(platform.move, Vector3.zero, ref vel, .5f);
                oldVel = platform.move;

                yield return null;
            }
        }
    }

}
