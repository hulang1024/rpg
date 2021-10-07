using Godot;
using static Godot.GD;
using System;
using System.Collections.Generic;
using InventorySystem;

namespace Characters
{
    public enum State
    {
        Idle,
        Walk,
        Sleep,
        Sit,
        Phoning,
        Pickup,
        PushCart,
        Reading,
        Gift,
        Lift,
        Throw,
        Hit,
        Punch,
        Stab,
        GrabGun,
        IdleGun,
        Shoot,
        Hurt
    }

    public enum PhoningState
    {
        TakeUp,
        Check,
        TakeBack,
        Cancelling,
    }

    public enum Dir { Right, Up, Left, Down }

    public partial class Character : KinematicBody2D
    {
        private readonly string[] dir_names = new string[4] { "right", "up", "left", "down" };
        // 一个完整的跨步动画需要多久
        private const int step_max_duration = 300;
    
        private Sprite sprite;
        private AnimationPlayer animationPlayer;

        private State nowState = State.Idle;
        public State NowState
        {
            set
            {
                if (nowState != value)
                {
                    oldState = nowState;
                    nowState = value;
                    onStateChange(value, oldState);
                }
            }
            get { return nowState; }
        }

        private State oldState = State.Idle;

        private Dir dir = Dir.Down;

        public float walkSpeed;
        public float WalkSpeed
        {
            get { return walkSpeed; }
            set {
                walkSpeed = value;
                animationPlayer.PlaybackSpeed = value;
            }
        }
        // 步长
        private int stepLength = 32;

        public Vector2 DirInput = Vector2.Zero;
        public PhoningState phoningState;
        public bool IsTakeCart = false;

        private Vector2 velocity = Vector2.Zero;

        public Inventory Inventory = new Inventory();

        public List<ActionableObject> ActionObjects = new List<ActionableObject>();

        public override void _Ready()
        {
            sprite = GetNode<Sprite>("Sprite");
            animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            initBody();
            LoadCharacterStyle();
            WalkSpeed = 1f;
        }

        public override void _Process(float delta)
        {
            if (Engine.EditorHint)
            {
                return;
            }
            else if (Preview)
            {
                playAnimation();
                return;
            }

            // todo: 下面代码考虑移到Controller
            if (DirInput.x == 0 && DirInput.y == 0)
            {
                switch (NowState)
                {
                    case State.Idle:
                    case State.Walk:
                    case State.PushCart:
                        NowState = State.Idle;
                        velocity.x = 0;
                        velocity.y = 0;
                        break;
                }
            }
            else
            {
                switch (NowState)
                {
                    case State.Idle:
                        NowState = IsTakeCart ? State.PushCart : State.Walk;
                        updateDir(delta);
                        break;
                    case State.Walk:
                    case State.PushCart:
                        updateDir(delta);
                        break;
                    case State.Phoning:
                        CancelPhoning();
                        break;
                    case State.GrabGun:
                    case State.IdleGun:
                        NowState = State.Walk;
                        updateDir(delta);
                        break;
                }
            }

            playAnimation();
        }

        private void updateDir(float delta)
        {
            if (DirInput.x > 0)
                dir = Dir.Right;
            else if (DirInput.x < 0)
                dir = Dir.Left;
            else if (DirInput.y > 0)
                dir = Dir.Down;
            else if (DirInput.y < 0)
                dir = Dir.Up;

            var unit = delta*1000 / (step_max_duration / WalkSpeed) * stepLength;
            velocity.x = DirInput.x * unit;
            velocity.y = DirInput.y * unit;

            MoveAndCollide(velocity);
        }

        public void CancelPhoning()
        {
            if (phoningState == PhoningState.Check)
            {
                // 打电话进行中的动画可以随时过渡到下个状态
                phoningState = PhoningState.TakeBack;
            }
            else
            {
                // 变成取消，待动画完成改变状态
                phoningState = PhoningState.Cancelling;
            }
        }

        private void onStateChange(State nowState, State oldState)
        {
            // Print(string.Format("{0,8} -> {1}", oldState, nowState));
        }

        private void playAnimation()
        {
            switch (NowState)
            {
                case State.Idle:
                    playAnimation("idle");
                    break;
                case State.Walk:
                    playAnimation("walk");
                    break;
                case State.Sleep:
                    playAnimation("sleep");
                    break;
                case State.Sit:
                    if (dir == Dir.Left || dir == Dir.Right)
                        playAnimation("sit");
                    break;
                case State.Phoning:
                    playPhoningAnimation();
                    break;
                case State.Pickup:
                    playAnimation("pickup");
                    break;
                case State.PushCart:
                    playAnimation("push_cart");
                    break;
                case State.Reading:
                    playAnimation("reading", false);
                    break;
                case State.Gift:
                    playAnimation("gift");
                    break;
                case State.Lift:
                    playAnimation("lift");
                    break;
                case State.Throw:
                    playAnimation("throw");
                    break;
                case State.Hit:
                    playAnimation("hit");
                    break;
                case State.Punch:
                    playAnimation("punch");
                    break;
                case State.Stab:
                    playAnimation("stab");
                    break;
                case State.GrabGun:
                    playAnimation("grab_gun");
                    break;
                case State.IdleGun:
                    playAnimation("idle_gun");
                    break;
                case State.Shoot:
                    playAnimation("shoot");
                    break;
                case State.Hurt:
                    playAnimation("hurt");
                    break;
            }
        }

        private void playPhoningAnimation()
        {
            switch (phoningState)
            {
                case PhoningState.TakeUp:
                    playAnimation("phone_take_up", false);
                    break;
                case PhoningState.Check:
                    playAnimation("phone_check", false);
                    break;
                case PhoningState.TakeBack:
                    playAnimation("phone_take_back", false);
                    break;
            }
        }

        private void playAnimation(string animName, bool hasDir = true)
        {
            animationPlayer.Play(
                string.Format("{0}{1}", animName, hasDir ? $"_{dir_names[(int)dir]}" : ""));
        }

        public void On_AnimationPlayer_animation_finished(string animName)
        {
            if (Preview) return;
            // Phoning
            if (animName.StartsWith("phone_take_up"))
            {
                phoningState = phoningState == PhoningState.Cancelling
                    ? PhoningState.TakeBack
                    : PhoningState.Check;
            }
            else if (animName.StartsWith("phone_check"))
            {
                if (phoningState == PhoningState.Cancelling)
                    phoningState = PhoningState.TakeBack;
            }
            else if (animName.StartsWith("phone_take_back"))
            {
                NowState = State.Idle;
            }

            else if (animName.StartsWith("grab_gun"))
            {
                if (NowState == State.GrabGun)
                    NowState = State.IdleGun;
            }
            else if (animName.StartsWith("idle_gun"))
            {
                // pass
            }
            else if (animName.StartsWith("shoot"))
            {
                if (NowState == State.Shoot)
                    NowState = State.IdleGun;
            }
            else
            {
                NowState = State.Idle;
            }
        }
    }
}