using UnityEngine;
using System.Collections;
using BehaviourMachine;

public class Controller2DBehaviours
{
    #region Conditions
    //[RequireComponent(typeof(Controller2D))]
    public abstract class Controller2DConditionNode : ConditionNode
    {
        [VariableInfo(canBeConstant = true, nullLabel = "Use Self", requiredField = false)]
        public GameObjectVar gameObject;

        protected Controller2D _controller;
        protected Controller2D controller
        {
            get
            {
                if (_controller == null)
                {
                    if (gameObject.isNone)
                        _controller = self.GetComponent<Controller2D>();
                    else if (!gameObject.isInvalid)
                        _controller = gameObject.Value.GetComponent<Controller2D>();
                }

                return _controller;
            }
        }
    }

    [NodeInfo(category = "Condition/Controller2D/")]
    public class IsCollisionAbove : Controller2DConditionNode
    {
        public override Status Update()
        {
            if (controller.collisions.above)
            {
                if (onSuccess.id != 0)
                    owner.root.SendEvent(onSuccess.id);

                return Status.Success;
            }
            else
            {
                return Status.Failure;
            }
        }
    }

    [NodeInfo(category = "Condition/Controller2D/")]
    public class IsCollisionBelow : Controller2DConditionNode
    {
        public override Status Update()
        {
            if (controller.collisions.below)
            {
                if (onSuccess.id != 0)
                    owner.root.SendEvent(onSuccess.id);

                return Status.Success;
            }
            else
            {
                return Status.Failure;
            }
        }
    }

    [NodeInfo(category = "Condition/Controller2D/")]
    public class IsCollisionRight : Controller2DConditionNode
    {
        public override Status Update()
        {
            if (controller.collisions.right)
            {
                if (onSuccess.id != 0)
                    owner.root.SendEvent(onSuccess.id);

                return Status.Success;
            }
            else
            {
                return Status.Failure;
            }
        }
    }

    [NodeInfo(category = "Condition/Controller2D/")]
    public class IsCollisionLeft : Controller2DConditionNode
    {
        public override Status Update()
        {
            if (controller.collisions.left)
            {
                if (onSuccess.id != 0)
                    owner.root.SendEvent(onSuccess.id);

                return Status.Success;
            }
            else
            {
                return Status.Failure;
            }
        }
    }

    [NodeInfo(category = "Condition/Controller2D/")]
    public class IsCollision : Controller2DConditionNode
    {
        public override Status Update()
        {
            if (controller.collisions.above || controller.collisions.below || controller.collisions.right || controller.collisions.left)
            {
                if (onSuccess.id != 0)
                    owner.root.SendEvent(onSuccess.id);

                return Status.Success;
            }
            else
            {
                return Status.Failure;
            }
        }
    }
    
    // Negatives
    [NodeInfo(category = "Condition/Controller2D/")]
    public class IsNoCollisionAbove : Controller2DConditionNode
    {
        public override Status Update()
        {
            if (!controller.collisions.above)
            {
                if (onSuccess.id != 0)
                    owner.root.SendEvent(onSuccess.id);

                return Status.Success;
            }
            else
            {
                return Status.Failure;
            }
        }
    }


    [NodeInfo(category = "Condition/Controller2D/")]
    public class IsNoCollisionBelow : Controller2DConditionNode
    {
        public override Status Update()
        {
            if (!controller.collisions.below)
            {
                if (onSuccess.id != 0)
                    owner.root.SendEvent(onSuccess.id);

                return Status.Success;
            }
            else
            {
                return Status.Failure;
            }
        }
    }

    [NodeInfo(category = "Condition/Controller2D/")]
    public class IsNoCollisionRight : Controller2DConditionNode
    {
        public override Status Update()
        {
            if (!controller.collisions.right)
            {
                if (onSuccess.id != 0)
                    owner.root.SendEvent(onSuccess.id);

                return Status.Success;
            }
            else
            {
                return Status.Failure;
            }
        }
    }

    [NodeInfo(category = "Condition/Controller2D/")]
    public class IsNoCollisionLeft : Controller2DConditionNode
    {
        public override Status Update()
        {
            if (!controller.collisions.left)
            {
                if (onSuccess.id != 0)
                    owner.root.SendEvent(onSuccess.id);

                return Status.Success;
            }
            else
            {
                return Status.Failure;
            }
        }
    }

    [NodeInfo(category = "Condition/Controller2D/")]
    public class IsNoCollision : Controller2DConditionNode
    {
        public override Status Update()
        {
            if (!controller.collisions.above && !controller.collisions.below && !controller.collisions.right && !controller.collisions.left)
            {
                if (onSuccess.id != 0)
                    owner.root.SendEvent(onSuccess.id);

                return Status.Success;
            }
            else
            {
                return Status.Failure;
            }
        }
    }

    // Edge
    [NodeInfo(category="Condition/Controller2D/")]
    public class IsOnEdge : Controller2DConditionNode
    {
        public Status returnWhenNotGrounded = Status.Failure;

        [VariableInfo(canBeConstant=true, nullLabel="Use Scale", requiredField=false)]
        public FloatVar facingDir;

        public override Status Update()
        {
            // No Collision below; Can't be on edge
            if (!controller.collisions.below)
                return returnWhenNotGrounded;

            float facing;
            if (facingDir.isNone || facingDir.isInvalid)
                facing = Mathf.Sign(self.transform.localScale.x);
            else
                facing = Mathf.Sign(facingDir.Value);

            // Define ray
            float dist = Controller2D.skinWidth * 2;
            Vector2 origin;

            if (facing > 0)
                origin = controller.raycastOrigins.bottomRight;
            else
                origin = controller.raycastOrigins.bottomLeft;

            RaycastHit2D hit = Physics2D.Raycast(origin, -Vector2.up, dist, controller.collisionMask);

            Debug.DrawRay(origin, -Vector2.up * dist, Color.cyan);

            // If hit, not on edge
            if (hit)
                return Status.Failure;

            return Status.Success;
        }
    }

    // Helper for the gargoyle
    [NodeInfo(category="Condition/Controller2D/", icon="CharacterController")]
    public class IsAnythingAbove : Controller2DConditionNode
    {
        [VariableInfo(canBeConstant=true, requiredField=false, nullLabel="Infinite")]
        public FloatVar maxHeight;

        public LayerMask layer;

        public override Status Update()
        {
            if (layer == 0)
                layer = controller.collisionMask;

            float rayLength = maxHeight.id == 0 ? Mathf.Infinity : maxHeight.Value;

            for (int i = 0; i < controller.verticalRayCount; i++)
            {

                Vector2 rayOrigin = controller.raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (controller.verticalRaySpacing * i + controller.velocity.x);
                RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, Vector2.up, rayLength, layer);

                foreach (RaycastHit2D hit in hits)
                {
                    if (hit.collider.gameObject != this.gameObject && !hit.collider.isTrigger)
                    {
                        return Status.Success;
                    }
                }

            }

            return Status.Failure;
        }
    }

    #endregion

    #region Actions

    //[RequireComponent(typeof(Controller2D))]
    public abstract class Controller2DActionNode : ActionNode
    {
        [VariableInfo(canBeConstant=true, nullLabel="Use Self", requiredField=false)]
        public GameObjectVar gameObject;
        protected GameObject target
        {
            get
            {
                if (gameObject.isNone)
                    return self;
                else if (!gameObject.isInvalid)
                    return gameObject;
                else
                    return null;
            }
        }

        private Controller2D _controller;
        protected Controller2D controller
        {
            get
            {
                if (_controller == null)
                { 
                    _controller = target.GetComponent<Controller2D>();
                    if (_controller == null)
                        Debug.LogError("There is no Controller2D on the target Game Object!");
                }

                return _controller;
            }
        }
    }

    [NodeInfo(category = "Action/Controller2D/", icon="CharacterController")]
    public class Move : Controller2DActionNode
    {
        [VariableInfo(canBeConstant=false, requiredField=false)]
        public Vector3Var storeInternalVel, storeExternalVel;

        [VariableInfo(canBeConstant = true, requiredField = true)]
        public FloatVar horizontalInput;

        [VariableInfo(canBeConstant = false, requiredField = false)]
        public BoolVar jumpInput, jumpRelease;

        [VariableInfo(canBeConstant = true, requiredField = true)]
        public FloatVar maxJumpHeight;

        [VariableInfo(canBeConstant = true, requiredField = false, nullLabel = "Don't Use")]
        public FloatVar minJumpHeight;
        private float minJumpHeightVal
        {
            get
            {
                if (minJumpHeight.isNone)
                    return 0;
                return minJumpHeight.Value;
            }
        }

        [VariableInfo(canBeConstant = true, requiredField = true)]
        public FloatVar timeToJumpApex;

        [VariableInfo(canBeConstant = true, requiredField = false, nullLabel = "Use 1:1")]
        public FloatVar gravityMod;
        private float gravityModVal
        {
            get
            {
                if (gravityMod.isNone)
                {
                    return 1;
                }
                return gravityMod.Value;
            }
        }

        [VariableInfo(canBeConstant = true, requiredField = true)]
        public FloatVar drag;

        [VariableInfo(canBeConstant = true, requiredField = false)]
        public Vector3Var externalForce;

        private float velocityXSmoothing;

        // To allow values to be changed at runtime, without recalculating values every frame
        private float _maxJumpHeight, _timeToApex;
        private float gravity, maxJumpVelocity, minJumpVelocity;

        private Vector3 internalVelocity, externalVelocity;
        private Vector3 velocity
        {
            get
            {
                return internalVelocity + externalVelocity;
            }
        }
        public override void Awake()
        {
            CalculateGravity();
        }

        public override Status Update()
        {
            internalVelocity = storeInternalVel.Value;
            externalVelocity = storeExternalVel.Value;

            if (maxJumpHeight != _maxJumpHeight || timeToJumpApex != _timeToApex)
            {
                CalculateGravity();
            }

            //  Horizontal
            internalVelocity.x = Mathf.SmoothDamp(internalVelocity.x, horizontalInput, ref velocityXSmoothing, drag);
            externalVelocity.x = externalForce.Value.x;

            // On JumpButton Up or Down
            if (jumpInput)
            {
                jumpInput.Value = false;
                internalVelocity.y = maxJumpVelocity;
            }
            if (jumpRelease)
            {
                jumpRelease.Value = false;
                if (internalVelocity.y > minJumpVelocity)
                    internalVelocity.y = minJumpVelocity;
            }

            float internVertGain = gravity * gravityModVal * Time.deltaTime / 2;
            float externVertGain = externalForce.Value.y * Time.deltaTime / 2;

            internalVelocity.y += internVertGain;
            externalVelocity.y += externVertGain;

            controller.Move(velocity * Time.deltaTime);

            // Reset velocity on collisions
            if (controller.velocity.x == 0)
                internalVelocity.x = 0;

            if (controller.collisions.above || controller.collisions.below)
            {
                internalVelocity.y = 0;
                externalVelocity.y = 0;
            }
            else
            {
                internalVelocity.y += internVertGain;
                externalVelocity.y += externVertGain;
            }

            //Debug.Log("Internal V: " + internalVelocity + " Gain: " + internVertGain + "   External: " + externalVelocity + " Gain: " + externVertGain);

            storeInternalVel.Value = internalVelocity;
            storeExternalVel.Value = externalVelocity;

            return Status.Success;
        }

        void CalculateGravity()
        {
            _maxJumpHeight = maxJumpHeight;
            _timeToApex = timeToJumpApex;

            float oldJump = maxJumpVelocity;
            gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
            maxJumpVelocity = timeToJumpApex * Mathf.Abs(gravity);
            minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeightVal);

            //float percentChange = (maxJumpVelocity - oldJump) / oldJump;
            //if (percentChange >= 0)
            //    percentChange += 1;
            //else
            //    percentChange = Mathf.Abs(percentChange);
            //Debug.Log("Gravity: " + gravity + "     Velocity: " + velocity + "      minjump: " + minJumpVelocity);

            //StateMachine parent = this.tree.parent as StateMachine;
            //if (parent.GetEnabledStateName().Equals("Jumping"))
            //{
            //    parent.enabledState.UpdateLogic();
            //    //foreach (var state in (parent.enabledState as BehaviourTree).enabledStates)
            //    //{
            //    //    state.UpdateLogic();
            //    //}
            //    //parent.EnableState(parent.enabledState);
            //}
        }
    }

    [NodeInfo(category="Action/Controller2D/", icon="CharacterController")]
    public class SimpleMove : Controller2DActionNode
    {
        [VariableInfo(canBeConstant=true, requiredField=true)]
        public Vector3Var move;

        public override Status Update()
        {
            controller.Move(move.Value);

            return Status.Success;
        }
    }

    [NodeInfo(category="Action/Controller2D/", icon="CharacterController")]
    public class MoveTo : Controller2DActionNode
    {
        [VariableInfo(canBeConstant = true, requiredField = true)]
        public Vector3Var target;

        public override Status Update()
        {
            Vector3 velocity = target.Value - base.target.transform.position;

            controller.Move(velocity);

            return Status.Success;
        }
    }

    #endregion
}
