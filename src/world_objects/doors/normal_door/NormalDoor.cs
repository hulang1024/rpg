using Godot;
using static Godot.GD;

namespace OrdinaryObjects
{
    [Tool]
    public class NormalDoor : Door
    {
        private ImageTexture openSheetTexture;
        private ImageTexture lockedSheetTexture;

        protected override void playAnimation()
        {
            switch (state)
            {
                case State.Opening:
                    animationPlayer.Play("opening");
                    break;
                case State.Opened:
                    animationPlayer.Play("opened");
                    break;
                case State.Closing:
                    animationPlayer.PlayBackwards("opening");
                    break;
                case State.Closed:
                    animationPlayer.Play("closed");
                    break;
            }
        }

        protected override void preloadSpriteTextures()
        {
            string pathPrefix = "user://res/ordinary_objects/animated_door_big";
            openSheetTexture = Textures.From($"{pathPrefix}_{styleId}_32x32.png");
            lockedSheetTexture = Textures.From($"{pathPrefix}_{styleId}_locked_32x32.png");
        }
        protected override void onAnimationStarted(string animName)
        {
            switch (animName)
            {
                case "opening":
                case "opened":
                case "closing":
                case "closed":
                    sprite.Texture = openSheetTexture;
                    break;
                case "locked":
                    sprite.Texture = lockedSheetTexture;
                    break;
            }
        }
    }
}