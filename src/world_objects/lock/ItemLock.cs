namespace OrdinaryObjects
{
    public class ItemLock
    {
        private string secretId;
        public string SecretId
        {
            get { return secretId; }
        }
        
        public ItemLock(string secretId)
        {
            this.secretId = secretId;
        }

        public bool IsMatched(LockKey key)
        {
            return true;//this.secretId == key?.SecretId;
        }
    }
}