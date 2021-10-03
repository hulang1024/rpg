using Godot;
using static Godot.GD;
using System;

namespace Characters
{
    public class PlayerCharacterController : Node2D
    {
        private Character character;
        
        private State NowState
        {
            get { return character.NowState; }
            set { character.NowState = value; }
        }

        public override void _Ready()
        {
            character = GetNode<Character>("../PlayerCharacter");
        }

        public override void _Process(float delta)
        {
            character.DirInput.x = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
            character.DirInput.y = Input.GetActionStrength("move_down") - Input.GetActionStrength("move_up");
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (@event.IsActionPressed("character_sit"))
            {
                NowState = NowState == State.Sit ? State.Idle : State.Sit;
            }
            else if (@event.IsActionPressed("character_phone"))
            {
                if (NowState == State.Phoning)
                {
                    character.CancelPhoning();
                }
                else
                {
                    NowState = State.Phoning;
                    character.phoningState = PhoningState.TakeUp;
                }
            }
            else if (@event.IsActionPressed("character_pickup"))
            {
                // todo: 判定是否可以
                NowState = State.Pickup;
            }
            else if (@event.IsActionPressed("character_take_cart"))
            {
                // todo: 判定是否可以
                character.IsTakeCart = !character.IsTakeCart;
            }
            else if (@event.IsActionPressed("character_take_book"))
            {
                // todo: 翻书页
                NowState = NowState == State.Reading ? State.Idle : State.Reading;
            }
            else if (@event.IsActionPressed("character_gift"))
            {
                // todo: 判定是否可以
                NowState = State.Gift;
            }
            else if (@event.IsActionPressed("character_lift"))
            {
                // todo: 判定是否可以
                NowState = NowState == State.Lift ? State.Idle : State.Lift;
            }
            else if (@event.IsActionPressed("character_throw"))
            {
                // todo: 判定是否可以
                NowState = State.Throw;
            }
            else if (@event.IsActionPressed("character_hit"))
            {
                switch (NowState)
                {
                    case State.Idle:
                        NowState = State.Hit;
                        break;
                    case State.IdleGun:
                        NowState = State.Shoot;
                        break;
                }
            }
            else if (@event.IsActionPressed("character_punch"))
            {
                NowState = State.Punch;
            }
            else if (@event.IsActionPressed("character_stab"))
            {
                NowState = State.Stab;
            }
            else if (@event.IsActionPressed("character_grab_gun"))
            {
                NowState = State.GrabGun;
            }
        }
    }
}