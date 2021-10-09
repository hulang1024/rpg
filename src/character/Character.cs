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

        private State state = State.Idle;
        public State State
        {
            internal set
            {
                if (state != value)
                {
                    oldState = state;
                    state = value;
                    // Print(string.Format("{0,8} -> {1}", oldState, nowState));
                }
            }
            get => state;
        }

        private State oldState = State.Idle;

        public Dir Dir { get; private set; } = Dir.Down;

        public float walkSpeed;
        public float WalkSpeed
        {
            get => walkSpeed;
            set
            {
                walkSpeed = value;
                animationPlayer.PlaybackSpeed = value;
            }
        }
        // 步长
        private int stepLength = 32;

        public Vector2 DirInput { get; } = Vector2.Zero;

        internal PhoningState phoningState;

        internal bool IsTakeCart = false;

        private Vector2 velocity = Vector2.Zero;

        public Inventory Inventory { get; } = new Inventory();

        public List<IActionableObject> ActionObjects { get; } = new List<IActionableObject>();

        private Sprite sprite;
        private AnimationPlayer animationPlayer;

        public override void _Ready()
        {
            sprite = GetNode<Sprite>("Sprite");
            animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            InitBody();
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
                PlayAnimation();
                return;
            }

            // todo: 下面代码考虑移到Controller
            if (DirInput.x == 0 && DirInput.y == 0)
            {
                switch (State)
                {
                    case State.Idle:
                    case State.Walk:
                    case State.PushCart:
                        State = State.Idle;
                        velocity.x = 0;
                        velocity.y = 0;
                        break;
                }
            }
            else
            {
                switch (State)
                {
                    case State.Idle:
                        State = IsTakeCart ? State.PushCart : State.Walk;
                        UpdateDir(delta);
                        break;
                    case State.Walk:
                    case State.PushCart:
                        UpdateDir(delta);
                        break;
                    case State.Phoning:
                        CancelPhoning();
                        break;
                    case State.GrabGun:
                    case State.IdleGun:
                        State = State.Walk;
                        UpdateDir(delta);
                        break;
                }
            }

            PlayAnimation();
        }

        private void UpdateDir(float delta)
        {
            if (DirInput.x > 0)
                Dir = Dir.Right;
            else if (DirInput.x < 0)
                Dir = Dir.Left;
            else if (DirInput.y > 0)
                Dir = Dir.Down;
            else if (DirInput.y < 0)
                Dir = Dir.Up;

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

        private void PlayAnimation()
        {
            switch (State)
            {
                case State.Idle:
                    PlayAnimation("idle");
                    break;
                case State.Walk:
                    PlayAnimation("walk");
                    break;
                case State.Sleep:
                    PlayAnimation("sleep");
                    break;
                case State.Sit:
                    if (Dir == Dir.Left || Dir == Dir.Right)
                        PlayAnimation("sit");
                    break;
                case State.Phoning:
                    PlayPhoningAnimation();
                    break;
                case State.Pickup:
                    PlayAnimation("pickup");
                    break;
                case State.PushCart:
                    PlayAnimation("push_cart");
                    break;
                case State.Reading:
                    PlayAnimation("reading", false);
                    break;
                case State.Gift:
                    PlayAnimation("gift");
                    break;
                case State.Lift:
                    PlayAnimation("lift");
                    break;
                case State.Throw:
                    PlayAnimation("throw");
                    break;
                case State.Hit:
                    PlayAnimation("hit");
                    break;
                case State.Punch:
                    PlayAnimation("punch");
                    break;
                case State.Stab:
                    PlayAnimation("stab");
                    break;
                case State.GrabGun:
                    PlayAnimation("grab_gun");
                    break;
                case State.IdleGun:
                    PlayAnimation("idle_gun");
                    break;
                case State.Shoot:
                    PlayAnimation("shoot");
                    break;
                case State.Hurt:
                    PlayAnimation("hurt");
                    break;
            }
        }

        private void PlayPhoningAnimation()
        {
            switch (phoningState)
            {
                case PhoningState.TakeUp:
                    PlayAnimation("phone_take_up", false);
                    break;
                case PhoningState.Check:
                    PlayAnimation("phone_check", false);
                    break;
                case PhoningState.TakeBack:
                    PlayAnimation("phone_take_back", false);
                    break;
            }
        }

        private void PlayAnimation(string animName, bool hasDir = true)
        {
            animationPlayer.Play(
                string.Format("{0}{1}", animName, hasDir ? $"_{dir_names[(int)Dir]}" : ""));
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
                State = State.Idle;
            }

            else if (animName.StartsWith("grab_gun"))
            {
                if (State == State.GrabGun)
                    State = State.IdleGun;
            }
            else if (animName.StartsWith("idle_gun"))
            {
                // pass
            }
            else if (animName.StartsWith("shoot"))
            {
                if (State == State.Shoot)
                    State = State.IdleGun;
            }
            else
            {
                State = State.Idle;
            }
        }
    }
}