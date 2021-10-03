using Godot;
using static Godot.GD;
using System;

namespace Characters
{
    public class CharacterStyleSpecifics
    {
        public CharacterFrameSize FrameSize;
        public bool IsKid;
        public string Body;
        public string Outfit;
        public string Hairstyle;
        public string Eyes;
        public string Smartphones;
        public string Accessories;

        public CharacterStyleSpecifics(bool isKid = false)
        {
            FrameSize = CharacterFrameSize.Small;
            Body = isKid ? "Body_1_kid_32x32" : "Body_32x32_01";
        }

        public override bool Equals(object obj)
        {
            var other = (CharacterStyleSpecifics)obj;
            return this.FrameSize == other.FrameSize
                && this.IsKid == other.IsKid
                && this.Body == other.Body
                && this.Outfit == other.Outfit
                && this.Hairstyle == other.Hairstyle
                && this.Eyes == other.Eyes
                && this.Smartphones == other.Smartphones
                && this.Accessories == other.Accessories;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public enum CharacterStyleComponentType
    {
        Body, Outfit, Hairstyle, Eyes, Smartphones, Accessories
    }

    public enum CharacterFrameSize
    {
        Mini, Small, Medium
    }
}