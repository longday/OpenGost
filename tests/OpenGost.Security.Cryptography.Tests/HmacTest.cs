﻿using System.Security.Cryptography;
using Xunit;

namespace OpenGost.Security.Cryptography
{
    public abstract class HmacTest<T> : CryptoConfigRequiredTest
        where T : HMAC, new()
    {
        protected abstract int BlockSize { get; }

        protected abstract HashAlgorithm CreateHashAlgorithm();

        protected void VerifyHmac(string dataHex, string keyHex, string digestHex)
        {
            byte[] digestBytes = digestHex.HexToByteArray();
            byte[] computedDigest;

            using (var hmac = new T())
            {
                Assert.True(hmac.HashSize > 0);

                byte[] key = keyHex.HexToByteArray();
                hmac.Key = key;

                // make sure the getter returns different objects each time
                Assert.NotSame(key, hmac.Key);
                Assert.NotSame(hmac.Key, hmac.Key);

                // make sure the setter didn't cache the exact object we passed in 
                key[0] = (byte)(key[0] + 1);
                Assert.NotEqual<byte>(key, hmac.Key);

                computedDigest = hmac.ComputeHash(dataHex.HexToByteArray());
            }

            Assert.Equal(digestBytes, computedDigest);
        }

        protected void VerifyHmacRfc2104()
        {
            // Ensure that keys shorter than the threshold don't get altered.
            using (var hmac = new T())
            {
                byte[] key = new byte[BlockSize];
                hmac.Key = key;
                byte[] retrievedKey = hmac.Key;
                Assert.Equal<byte>(key, retrievedKey);
            }

            // Ensure that keys longer than the threshold are adjusted via Rfc2104 Section 2.
            using (var hmac = new T())
            {
                byte[] overSizedKey = new byte[BlockSize + 1];
                hmac.Key = overSizedKey;
                byte[] actualKey = hmac.Key;
                byte[] expectedKey = CreateHashAlgorithm().ComputeHash(overSizedKey);
                Assert.Equal<byte>(expectedKey, actualKey);

                // Also ensure that the hashing operation uses the adjusted key.
                byte[] data = new byte[100];
                hmac.Key = expectedKey;
                byte[] expectedHash = hmac.ComputeHash(data);

                hmac.Key = overSizedKey;
                byte[] actualHash = hmac.ComputeHash(data);
                Assert.Equal<byte>(expectedHash, actualHash);
            }
        }
    }
}