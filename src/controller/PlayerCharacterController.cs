using Godot;
using static Godot.GD;
using System;
using OrdinaryObjects;
using InventorySystem;

namespace Characters
{
    public class PlayerCharacterController : Node2D
    {
        private Character character;
        
        private State State
        {
            get { return character.State; }
            set { character.State = value; }
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
                State = State == State.Sit ? State.Idle : State.Sit;
            }
            else if (@event.IsActionPressed("character_phone"))
            {
                if (State == State.Phoning)
                {
                    character.CancelPhoning();
                }
                else
                {
                    State = State.Phoning;
                    character.phoningState = PhoningState.TakeUp;
                }
            }
            else if (@event.IsActionPressed("character_pickup"))
            {
                // todo: 判定是否可以
                State = State.Pickup;
            }
            else if (@event.IsActionPressed("character_take_cart"))
            {
                // todo: 判定是否可以
                character.IsTakeCart = !character.IsTakeCart;
            }
            else if (@event.IsActionPressed("character_take_book"))
            {
                // todo: 翻书页
                State = State == State.Reading ? State.Idle : State.Reading;
            }
            else if (@event.IsActionPressed("character_gift"))
            {
                // todo: 判定是否可以
                State = State.Gift;
            }
            else if (@event.IsActionPressed("character_lift"))
            {
                // todo: 判定是否可以
                State = State == State.Lift ? State.Idle : State.Lift;
            }
            else if (@event.IsActionPressed("character_throw"))
            {
                // todo: 判定是否可以
                State = State.Throw;
            }
            else if (@event.IsActionPressed("character_hit"))
            {
                switch (State)
                {
                    case State.Idle:
                        State = State.Hit;
                        break;
                    case State.IdleGun:
                        State = State.Shoot;
                        break;
                }
            }
            else if (@event.IsActionPressed("character_punch"))
            {
                State = State.Punch;
            }
            else if (@event.IsActionPressed("character_stab"))
            {
                State = State.Stab;
            }
            else if (@event.IsActionPressed("character_grab_gun"))
            {
                State = State.GrabGun;
            }
            else if (@event.IsActionPressed("gameplay_toggle_door"))
            {
                var doorHandle = character.ActionObjects.Find(o => o is DoorHandle);
                (doorHandle as DoorHandle)?.Toggle();
            }
            else if (@event.IsActionPressed("gameplay_toggle_lock"))
            {
                var doorHandle = character.ActionObjects.Find(o => o is DoorHandle);
                if (doorHandle != null)
                {
                    /*
                    var keyItem = (InventoryItem)character.Inventory.Items
                        .Find(item => item.ObjectType == ItemObjectType.LockKey);
                    if (keyItem != null)
                    {
                        (door as Door)?.ToggleLock(character, null);
                    }*/
                    (doorHandle as DoorHandle)?.ToggleLock(null);
                }
            }
        }
    }
}