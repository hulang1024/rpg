using Godot;
using static Godot.GD;
using System.Collections.Generic;

namespace OrdinaryObjects
{
    public abstract class Door : StaticBody2D
    {
        public enum State { Opened, Opening, Closing, Closed }
        public enum LockState { /*Unlocking,*/ NotLocked, /*Locking,*/ Locked };

        protected State state = State.Closed;
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

        protected LockState lockState = LockState.NotLocked;
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

        protected int styleId = 1;
        [Export]
        public int StyleId
        {
            get { return styleId; }
            set
            {
                styleId = value;
                if (sprite != null)
                {
                    preloadSpriteTextures();
                }
                if (Engine.EditorHint)
                {
                    playAnimation();
                }
            }
        }

        protected ItemLock itemLock = ItemLockKeyGenerator.CreateLock();

        protected List<Node> obstacles = new List<Node>();

        protected Sprite sprite;
        protected AnimationPlayer animationPlayer;

        public override void _Ready()
        {
            sprite = GetNode<Sprite>("Sprite");

            animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            animationPlayer.Connect("animation_finished", this, "onAnimationFinished");
            animationPlayer.Connect("animation_started", this, "onAnimationStarted");

            // 阻挡物检测Area，避免关门之后，造成门的碰撞体与其它碰撞体交叉产生问题
            // 同时作为一种游戏机制
            var obstacleArea = GetNode<Area2D>("ObstacleArea2D");
            obstacleArea.Connect("body_entered", this, "onObstacleAreaNodeEntered");
            obstacleArea.Connect("body_exited", this, "onObstacleAreaNodeExited");

            preloadSpriteTextures();
            playAnimation();
        }

        public void Open()
        {
            if (!canOperate()) return;

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

            Print("开门中");
            state = State.Opening;
            playAnimation();
        }

        public void Close()
        {
            if (!canOperate()) return;

            if (state != State.Opened)
            {
                Print("不能关门，因为门不是开状态");
                return;
            }
            Print("关门中");
            state = State.Closing;
            playAnimation();
        }

        public void Unlock(LockKey key)
        {
            if (!canOperate()) return;

            if (!(state == State.Closed && lockState == LockState.Locked))
                return;
            if (!itemLock.IsMatched(key))
            {
                Print("不匹配的钥匙");
                return;
            }
            //lockState = LockState.Unlocking;
            lockState = LockState.NotLocked;
            Print("开锁完成");
        }

        public void Lock(LockKey key)
        {
            if (!canOperate()) return;

            if (!(state == State.Closed && lockState == LockState.NotLocked))
                return;
            if (!itemLock.IsMatched(key))
            {
                Print("不匹配的钥匙");
                return;
            }
            //lockState = LockState.Locking;
            lockState = LockState.Locked;
            Print("上锁完成");
        }

        public void Toggle()
        {
            switch (state)
            {
                case State.Closed:
                    Open();
                    break;
                case State.Opened:
                    Close();
                    break;
            }
        }

        public void Toggle(LockKey key)
        {
            switch (lockState)
            {
                case LockState.NotLocked:
                    Lock(key);
                    break;
                case LockState.Locked:
                    Unlock(key);
                    break;
            }
        }

        private bool canOperate()
        {
            return obstacles.Count == 0;
        }

        protected virtual void playAnimation()
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

        protected abstract void preloadSpriteTextures();

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
                    playAnimation();
                    break;
            }
        }

        protected virtual void onAnimationStarted(string animName) { }

        private void onObstacleAreaNodeEntered(Node2D node)
        {
            if (! obstacles.Exists(o => o == node.Owner))
            {
                obstacles.Add(node.Owner);
            }
        }

        private void onObstacleAreaNodeExited(Node2D node)
        {
            obstacles.Remove(node.Owner);
        }
        #endregion
    }
}