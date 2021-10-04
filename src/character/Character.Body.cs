using Godot;
using OrdinaryObjects;

namespace Characters
{
    public partial class Character
    {
        /*
        private Node enteredBody;
        public Node EnteredBody
        {
            get { return enteredBody; }
        }
        */
    
        private void initBody()
        {
            //var bodyArea = GetNode<Area2D>("BodyArea");
            //bodyArea.Connect("body_entered", this, "onBodyAreaBodyEntered");
            //bodyArea.Connect("body_exited", this, "onBodyAreaBodyExited");
        }

        /*
        private void onBodyAreaBodyEntered(Node body)
        {
            enteredBody = body;
        }

        private void onBodyAreaBodyExited(Node body)
        {
            enteredBody = null;
        }*/
    }
}
