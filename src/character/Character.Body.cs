using Godot;
using OrdinaryObjects;

namespace Characters
{
    public partial class Character
    {
        private void initBody()
        {
            var bodyArea = GetNode<Area2D>("BodyArea");
            bodyArea.Connect("area_entered", this, "onBodyAreaAreaEntered");
            bodyArea.Connect("area_exited", this, "onBodyAreaAreaExited");
            bodyArea.Connect("body_entered", this, "onBodyAreaBodyEntered");
            bodyArea.Connect("body_exited", this, "onBodyAreaBodyExited");
        }

        private void onBodyAreaAreaEntered(Node area)
        {
            manageActionObject(area, true);
        }

        private void onBodyAreaAreaExited(Node area)
        {
            manageActionObject(area, false);
        }

        private void onBodyAreaBodyEntered(Node body)
        {
            manageActionObject(body, true);
        }

        private void onBodyAreaBodyExited(Node body)
        {
            manageActionObject(body, false);
        }

        /// <summary>
        /// 增加或删除作用动作对象（目前可能这只意味着近身的动作对象）
        /// </summary>
        /// <param name="node"></param>
        /// <param name="isEntered"></param>
        private void manageActionObject(Node node, bool isEntered)
        {
            if (node is ActionableObject)
            {
                if (isEntered)
                {
                    ActionObjects.Add(node as ActionableObject);
                }
                else
                {
                    ActionObjects.Remove(node as ActionableObject);
                }
            }
        }
    }
}
