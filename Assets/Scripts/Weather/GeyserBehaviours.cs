using UnityEngine;
using System.Collections;
using BehaviourMachine;

public class GeyserBehaviours {

    static int frozenValueHash = Animator.StringToHash("frozen");

    public abstract class BaseGeyserAction : ActionNode
    {
        private Geyser _geyserController;
        protected Geyser geyserController
        {
            get
            {
                if (_geyserController == null)
                    _geyserController = self.GetComponent<Geyser>();

                return _geyserController;
            }
        }
    }

    [NodeInfo(category="Geyser/")]
    public class MoveUp : BaseGeyserAction
    {
        public override Status Update()
        {
            if (geyserController.direction != Geyser.GeyserDirection.up)
                geyserController.direction = Geyser.GeyserDirection.up;

            return Status.Success;
        }
    }

    [NodeInfo(category = "Geyser/")]
    public class MoveDown : BaseGeyserAction
    {
        public override Status Update()
        {
            if (geyserController.direction != Geyser.GeyserDirection.down)
                geyserController.direction = Geyser.GeyserDirection.down;

            return Status.Success;
        }
    }

    [NodeInfo(category = "Geyser/")]
    public class Cyclic : BaseGeyserAction
    {
        public Geyser.GeyserDirection defaultDirection = Geyser.GeyserDirection.up;

        public override Status Update()
        {
            if (geyserController.direction == Geyser.GeyserDirection.none)
            {
                geyserController.direction = 
                    (geyserController.oldDirection == Geyser.GeyserDirection.none) ? defaultDirection : geyserController.oldDirection;
            }
            else if (!geyserController.isMoving)
            {
                geyserController.direction =
                    (geyserController.direction == Geyser.GeyserDirection.down) ? Geyser.GeyserDirection.up : Geyser.GeyserDirection.down;
            }

            return Status.Success;
        }
    }

    [NodeInfo(category = "Geyser/")]
    public class Freeze : BaseGeyserAction
    {
        public override Status Update()
        {
            if (geyserController.direction != Geyser.GeyserDirection.none)
                geyserController.direction = Geyser.GeyserDirection.none;

            return Status.Success;
        }
    }

    [NodeInfo(category = "Geyser/")]
    public class EnableDamage : BaseGeyserAction
    {
        public override Status Update()
        {
            var children = self.GetComponentsInChildren<Collider2D>(true);
            foreach (var c in children)
            {
                c.tag = "Damage";
                c.gameObject.layer = LayerMask.NameToLayer("Enemy");

                var anim = c.GetComponent<Animator>();
                anim.speed = 1;
                anim.SetFloat(frozenValueHash, 0);
            }

            return Status.Success;
        }
    }

    [NodeInfo(category = "Geyser/")]
    public class EnablePlatform : BaseGeyserAction
    {
        public override Status Update()
        {
            var children = self.GetComponentsInChildren<Collider2D>(true);
            foreach (var c in children)
            {
                c.tag = "Untagged";
                c.gameObject.layer = LayerMask.NameToLayer("Default");

                var anim = c.GetComponent<Animator>();
                anim.speed = 0;
                anim.SetFloat(frozenValueHash, 1);
            }

            return Status.Success;
        }
    }
}
