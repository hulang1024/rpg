namespace OrdinaryObjects
{
    public class ItemLock
    {
        public string SecretId { get; }
        
        public ItemLock(string secretId)
        {
            SecretId = secretId;
        }

        public bool IsMatched(LockKey key)
        {
            return true;//this.secretId == key?.SecretId;
        }
    }
}