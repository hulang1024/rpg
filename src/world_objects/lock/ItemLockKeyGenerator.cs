using System;

namespace OrdinaryObjects
{
    public sealed class ItemLockKeyGenerator
    {
        public static ItemLock CreateLock()
        {
            var secretId = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString("X");
            return new ItemLock(secretId);
        }

        public static LockKey CreateMatchedKey(ItemLock itemLock)
        {
            return new LockKey(itemLock.SecretId);
        }
    }
}