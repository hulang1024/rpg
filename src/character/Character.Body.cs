using Godot;
using OrdinaryObjects;

namespace Characters
{
    public partial class Character
    {
        private void InitBody()
        {
            var bodyArea = GetNode<Area2D>("BodyArea");
            bodyArea.Connect("area_entered", this, "OnBodyAreaAreaEntered");
            bodyArea.Connect("area_exited", this, "OnBodyAreaAreaExited");
            bodyArea.Connect("body_entered", this, "OnBodyAreaBodyEntered");
            bodyArea.Connect("body_exited", this, "OnBodyAreaBodyExited");
        }

        private void OnBodyAreaAreaEntered(Node area)
        {
            ManageActionObject(area, true);
        }

        private void OnBodyAreaAreaExited(Node area)
        {
            ManageActionObject(area, false);
        }

        private void OnBodyAreaBodyEntered(Node body)
        {
            ManageActionObject(body, true);
        }

        private void OnBodyAreaBodyExited(Node body)
        {
            ManageActionObject(body, false);
        }

        /// <summary>
        /// 增加或删除作用动作对象（目前可能这只意味着近身的动作对象）
        /// </summary>
        /// <param name="node"></param>
        /// <param name="isEntered"></param>
        private void ManageActionObject(Node node, bool isEntered)
        {
            if (node is IActionableObject)
            {
                if (isEntered)
                {
                    ActionObjects.Add(node as IActionableObject);
                }
                else
                {
                    ActionObjects.Remove(node as IActionableObject);
                }
            }
        }
    }
}
