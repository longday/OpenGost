﻿using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace OpenGost.Security.Cryptography
{
    using static CryptoUtils;

    /// <summary>
    /// Accesses the managed version of the <see cref="Grasshopper"/> algorithm. This class cannot be inherited.
    /// </summary>
    [ComVisible(true)]
    public sealed class GrasshopperManaged : Grasshopper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Grasshopper"/> class.
        /// </summary>
        public GrasshopperManaged()
        { }

        /// <summary>
        /// Creates a symmetric <see cref="Grasshopper"/> decryptor object with the specified key and initialization vector.
        /// </summary>
        /// <param name="rgbKey">
        /// The secret key to be used for the symmetric algorithm. The key size must be 256 bits.
        /// </param>
        /// <param name="rgbIV">
        /// The initialization vector to be used for the symmetric algorithm.
        /// </param>
        /// <returns>
        /// A symmetric <see cref="Grasshopper"/> decryptor object.
        /// </returns>
        public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV)
        {
            return new GrasshopperManagedTransform(rgbKey, rgbIV, BlockSize, Mode, Padding, SymmetricTransformMode.Decrypt);
        }

        /// <summary>
        /// Creates a symmetric <see cref="Grasshopper"/> encryptor object with the specified key and initialization vector.
        /// </summary>
        /// <param name="rgbKey">
        /// The secret key to be used for the symmetric algorithm. The key size must be 256 bits.
        /// </param>
        /// <param name="rgbIV">
        /// The initialization vector to be used for the symmetric algorithm.
        /// </param>
        /// <returns>
        /// A symmetric <see cref="Grasshopper"/> encryptor object.
        /// </returns>
        public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV)
        {
            return new GrasshopperManagedTransform(rgbKey, rgbIV, BlockSize, Mode, Padding, SymmetricTransformMode.Encrypt);
        }

        /// <summary>
        /// Generates a random initialization vector to be used for the algorithm.
        /// </summary>
        public override void GenerateIV()
        {
            IVValue = GenerateRandomBytes(FeedbackSizeValue / 8);
        }

        /// <summary>
        /// Generates a random key to be used for the algorithm.
        /// </summary>
        public override void GenerateKey()
        {
            KeyValue = GenerateRandomBytes(KeySizeValue / 8);
        }
    }
}
