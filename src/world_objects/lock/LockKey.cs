namespace OrdinaryObjects
{
    public class LockKey
    {
        public string SecretId { get; }
        
        public LockKey(string secretId)
        {
            SecretId = secretId;
        }
    }
}