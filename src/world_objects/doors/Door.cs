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
            get => state;
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
                    PlayAnimation();
                }
            }
        }

        protected LockState lockState = LockState.NotLocked;
        [Export]
        public LockState NowLockState
        {
            get => lockState;
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
                    PlayAnimation();
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
                    PreloadSpriteTextures();
                }
                if (Engine.EditorHint)
                {
                    PlayAnimation();
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
            animationPlayer.Connect("animation_finished", this, "OnAnimationFinished");
            animationPlayer.Connect("animation_started", this, "OnAnimationStarted");

            // 阻挡物检测Area，避免关门之后，造成门的碰撞体与其它碰撞体交叉产生问题
            // 同时作为一种游戏机制
            var obstacleArea = GetNode<Area2D>("ObstacleArea2D");
            obstacleArea.Connect("body_entered", this, "OnObstacleAreaNodeEntered");
            obstacleArea.Connect("body_exited", this, "OnObstacleAreaNodeExited");

            PreloadSpriteTextures();
            PlayAnimation();
        }

        public void Open()
        {
            if (!CanOperate()) return;

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
            PlayAnimation();
        }

        public void Close()
        {
            if (!CanOperate()) return;

            if (state != State.Opened)
            {
                Print("不能关门，因为门不是开状态");
                return;
            }
            Print("关门中");
            state = State.Closing;
            PlayAnimation();
        }

        public void Unlock(LockKey key)
        {
            if (!CanOperate()) return;

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
            if (!CanOperate()) return;

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

        private bool CanOperate()
        {
            return obstacles.Count == 0;
        }

        protected virtual void PlayAnimation()
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

        protected abstract void PreloadSpriteTextures();

        #region 节点事件
        private void OnAnimationFinished(string animName)
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
                    PlayAnimation();
                    break;
            }
        }

        protected virtual void OnAnimationStarted(string animName) { }

        private void OnObstacleAreaNodeEntered(Node2D node)
        {
            if (! obstacles.Exists(o => o == node.Owner))
            {
                obstacles.Add(node.Owner);
            }
        }

        private void OnObstacleAreaNodeExited(Node2D node)
        {
            obstacles.Remove(node.Owner);
        }
        #endregion
    }
}