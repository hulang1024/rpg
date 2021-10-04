namespace OrdinaryObjects
{
    public class LockKey
    {
        private string secretId;
        public string SecretId
        {
            get { return secretId; }
        }
        
        public LockKey(string secretId)
        {
            this.secretId = secretId;
        }
    }
}