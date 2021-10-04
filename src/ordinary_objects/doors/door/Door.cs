using Godot;
using static Godot.GD;
using Characters;
using System.Collections.Generic;

namespace OrdinaryObjects
{
    [Tool]
    public class Door : StaticBody2D
    {
        public enum State { Opened, Opening, Closing, Closed }
        public enum LockState { /*Unlocking,*/NotLocked, /*Locking,*/ Locked };

        private State state = State.Closed;
        [Export]
        public State NowState
        {
            get { return state; }
            set
            {
                state = value;
                if (state != State.Closed)
                {
                    lockState = LockState.NotLocked;
                }
                if (Engine.EditorHint)
                {
                    PropertyListChangedNotify();
                    playAnimation();
                }
            }
        }

        private LockState lockState = LockState.NotLocked;
        [Export]
        public LockState NowLockState
        {
            get { return lockState; }
            set
            {
                lockState = value;
                if (lockState == LockState.Locked)
                {
                    state = State.Closed;
                }
                if (Engine.EditorHint)
                {
                    PropertyListChangedNotify();
                    playAnimation();
                }
            }
        }

        private int styleId = 1;
        [Export]
        public int StyleId
        {
            get { return styleId; }
            set
            {
                styleId = value;
                preloadSpriteTextures();
                if (Engine.EditorHint)
                {
                    playAnimation();
                }
            }
        }

        private ItemLock itemLock = ItemLockKeyGenerator.CreateLock();

        private List<Node> Holders = new List<Node>();
        private int obstacleCount = 0;

        private Sprite sprite;
        private AnimationPlayer animationPlayer;
        private ImageTexture openSheetTexture;
        private ImageTexture lockedSheetTexture;

        public override void _Ready()
        {
            sprite = GetNode<Sprite>("Sprite");

            animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            animationPlayer.Connect("animation_finished", this, "onAnimationFinished");
            animationPlayer.Connect("animation_started", this, "onAnimationStarted");

            // 可操作开关门Area
            var holdArea = GetNode<Area2D>("HoldArea2D");
            holdArea.Connect("body_entered", this, "onHoldAreaBodyEntered");
            holdArea.Connect("body_exited", this, "onHoldAreaBodyExited");

            // 阻挡物检测Area，避免关门之后，造成门的与其它碰撞体交叉产生不自然位移
            // 同时作为一种游戏机制
            var obstacleArea = GetNode<Area2D>("ObstacleArea2D");
            obstacleArea.Connect("body_entered", this, "onObstacleAreaBodyEntered");
            obstacleArea.Connect("body_exited", this, "onObstacleAreaBodyExited");

            preloadSpriteTextures();
            playAnimation();
        }

        public void Open(Node body)
        {
            if (state != State.Closed)
            {
                Print("不能开门，因为门不是关状态");
                return;
            }
            if (lockState == LockState.Locked)
            {
                Print("不能开门，因为门已经锁上，请先开锁");
                animationPlayer.Play("locked");
                return;
            }
            if (!canToggle(body))
            {
                Print("无法开门");
                return;
            }
            Print("开门中");
            state = State.Opening;
            playAnimation();
        }

        public void Close(Node body)
        {
            if (state != State.Opened)
            {
                Print("不能关门，因为门不是开状态");
                return;
            }
            if (!canToggle(body))
            {
                Print("无法关门");
                return;
            }
            Print("关门中");
            state = State.Closing;
            playAnimation();
        }

        public void Unlock(Node body, LockKey key)
        {
            if (!(state == State.Closed && lockState == LockState.Locked))
                return;
            if (!itemLock.IsMatched(key))
            {
                Print("不匹配的钥匙");
                return;
            }
            if (!canToggle(body))
            {
                Print("无法开锁");
                return;
            }
            //lockState = LockState.Unlocking;
            lockState = LockState.NotLocked;
            Print("开锁完成");
        }

        public void Lock(Node body, LockKey key)
        {
            if (!(state == State.Closed && lockState == LockState.NotLocked))
                return;
            if (!itemLock.IsMatched(key))
            {
                Print("不匹配的钥匙");
                return;
            }
            if (!canToggle(body))
            {
                Print("无法上门锁");
                return;
            }
            //lockState = LockState.Locking;
            lockState = LockState.Locked;
            Print("上锁完成");
        }

        public void Toggle(Node body)
        {
            switch (state)
            {
                case State.Closed:
                    Open(body);
                    break;
                case State.Opened:
                    Close(body);
                    break;
            }
        }


        public void ToggleLock(Node body, LockKey key)
        {
            switch (lockState)
            {
                case LockState.NotLocked:
                    Lock(body, key);
                    break;
                case LockState.Locked:
                    Unlock(body, key);
                    break;
            }
        }

        private bool canToggle(Node body)
        {
            return Holders.Contains(body) && obstacleCount == 0;
        }

        private void playAnimation()
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

        private void preloadSpriteTextures()
        {
            string pathPrefix = "user://res/ordinary_objects/animated_door_big";
            openSheetTexture = Textures.From($"{pathPrefix}_{styleId}_32x32.png");
            lockedSheetTexture = Textures.From($"{pathPrefix}_{styleId}_locked_32x32.png");
        }

        #region 节点事件
        private void onAnimationFinished(string animName)
        {
            switch (animName)
            {
                case "opening":
                    switch (state)
                    {
                        case State.Opening:
                            state = State.Opened;
                            Print("开门完成");
                            break;
                        case State.Closing:
                            state = State.Closed;
                            Print("关门完成");
                            break;
                    }
                    break;
            }
        }

        private void onAnimationStarted(string animName)
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

        private void onHoldAreaBodyEntered(Node body)
        {
            Holders.Add(body);
            if (body is Character)
            {
                (body as Character).ActionObjects.Add(this);
            }
        }

        private void onHoldAreaBodyExited(Node body)
        {
            Holders.Remove(body);
            if (body is Character)
            {
                (body as Character).ActionObjects.Remove(this);
            }
        }

        private void onObstacleAreaBodyEntered(Node body)
        {
            obstacleCount++;
        }

        private void onObstacleAreaBodyExited(Node body)
        {
            obstacleCount--;
        }
        #endregion
    }
}