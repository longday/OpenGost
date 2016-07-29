﻿using Xunit;

namespace Gost.Security.Cryptography
{
    using static CryptoUtils;
    using static ECHelper;

    public abstract class GostECDsaTest<T> : AsymmetricAlgorithmTest<T>
        where T : GostECDsa
    {
        protected T Create(ECParameters parameters)
        {
            T algorithm = Create();
            algorithm.ImportParameters(parameters);
            return algorithm;
        }

        protected void CheckExportParameters(ECParameters parameters)
        {
            ECParameters exportedParameters;
            using (T algorithm = Create(parameters))
            {
                exportedParameters = algorithm.ExportParameters(false);

                exportedParameters.Validate();
                AssertEqual(parameters, exportedParameters, false);
                Assert.Null(exportedParameters.D);

                if (parameters.D != null)
                {
                    exportedParameters = algorithm.ExportParameters(true);
                    exportedParameters.Validate();
                    AssertEqual(parameters, exportedParameters, true);
                }
            }
        }

        protected bool VerifyHash(ECParameters parameters, byte[] hash, byte[] signature)
        {
            using (T algorithm = Create(parameters))
                return algorithm.VerifyHash(hash, signature);
        }

        protected bool VerifyHash(ECParameters parameters, string hashHex, string signatureHex)
            => VerifyHash(parameters, hashHex.HexToByteArray(), signatureHex.HexToByteArray());

        protected void SignAndVerifyHash(ECParameters parameters)
        {
            byte[] hash, signature;
            using (T algorithm = Create(parameters))
            {
                hash = GenerateRandomBytes(algorithm.KeySize / 8);
                signature = algorithm.SignHash(hash);
            }

            Assert.True(VerifyHash(parameters, hash, signature));
        }

        protected void WriteAndReadXmlString(ECParameters parameters)
        {
            parameters.Validate();

            string xmlString;
            using (T algorithm = Create(parameters))
                xmlString = algorithm.ToXmlString(false);

            Assert.False(string.IsNullOrEmpty(xmlString));

            ECParameters newParameters;
            using (T algorithm = Create(xmlString))
                newParameters = algorithm.ExportParameters(false);

            AssertEqual(parameters, newParameters, false);
        }
    }
}
