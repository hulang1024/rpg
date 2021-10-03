
namespace Characters
{
    public enum Gender
    {
        Male,
        Female
    }

    public class PremadeCharacterInfo
    {
        public string Id = null;
        public CharacterStyleSpecifics Styles = new CharacterStyleSpecifics();
        public Gender Gender = Gender.Male;
        public string Tags;
        public string Remark;
        public string OldIds;

        public override bool Equals(object obj)
        {
            if (!(obj is PremadeCharacterInfo)) return false;
            var other = (PremadeCharacterInfo)obj;
            return this.Styles.Equals(other.Styles)
                && this.Gender == other.Gender
                && this.Tags == other.Tags
                && this.Remark == other.Remark;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}