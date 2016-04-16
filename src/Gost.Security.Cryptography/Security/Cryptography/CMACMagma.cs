﻿using System;
using System.Security.Cryptography;

namespace Gost.Security.Cryptography
{
    using static CryptoConstants;
    using static CryptoUtils;

    /// <summary>
    /// Computes a Cipher-based Message Authentication Code (CMAC) using <see cref="Magma"/> algorithm.
    /// </summary>
    public class CMACMagma : KeyedHashAlgorithm
    {
        #region Constants

        private static readonly byte[] s_irreduciblePolynomial =
        {
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1B
        };

        #endregion

        private readonly CMACAlgorithm _cmacAlgorithm;

        /// <summary>
        /// Gets the size, in bits, of the computed hash code.
        /// </summary>
        /// <value>
        /// The size, in bits, of the computed hash code.
        /// </value>
        public override int HashSize => _cmacAlgorithm.HashSize;

        /// <summary>
        /// Gets or sets the key to use in the hash algorithm.
        /// </summary>
        /// <value>
        /// The key to use in the hash algorithm.
        /// </value>
        /// <exception cref="CryptographicException">
        /// An attempt was made to change the <see cref="Key"/>
        /// property after hashing has begun.
        /// </exception>
        public override byte[] Key
        {
            get { return _cmacAlgorithm.Key; }
            set
            {
                base.Key = value;
                _cmacAlgorithm.Key = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CMACMagma"/> class.
        /// </summary>
        public CMACMagma()
            : this(GenerateRandomBytes(32))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CMACMagma"/> class with the specified key data.
        /// </summary>
        /// <param name="key">
        /// The secret key for <see cref="CMACMagma"/> encryption. 
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="key"/> parameter is null. 
        /// </exception>
        public CMACMagma(byte[] key)
            : this(MagmaAlgorithmName, key)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CMACMagma"/> class with the specified key data
        /// and using the specified implementation of <see cref="Magma"/>.
        /// </summary>
        /// <param name="algorithmName">
        /// The name of the <see cref="Magma"/> implementation to use. 
        /// </param>
        /// <param name="key">
        /// The secret key for <see cref="CMACMagma"/> encryption. 
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="key"/> parameter is null. 
        /// </exception>
        public CMACMagma(string algorithmName, byte[] key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            _cmacAlgorithm = new CMACAlgorithm(algorithmName, key, s_irreduciblePolynomial, Magma.Create);
        }

        /// <summary>
        /// Initializes an instance of <see cref="CMACMagma"/>.
        /// </summary>
        public override void Initialize()
            => _cmacAlgorithm.Initialize();

        /// <summary>
        /// Routes data written to the object into the <see cref="Magma"/>
        /// encryptor for computing the Cipher-based Message Authentication Code (CMAC).
        /// </summary>
        /// <param name="array">
        /// The input data.
        /// </param>
        /// <param name="ibStart">
        /// The offset into the byte array from which to begin using data.
        /// </param>
        /// <param name="cbSize">
        /// The number of bytes in the array to use as data.
        /// </param>
        protected override void HashCore(byte[] array, int ibStart, int cbSize)
            => _cmacAlgorithm.TransformBlock(array, ibStart, cbSize, null, 0);

        /// <summary>
        /// Returns the computed Cipher-based Message Authentication Code (CMAC)
        /// after all data is written to the object.
        /// </summary>
        /// <returns>
        /// The computed MAC.
        /// </returns>
        protected override byte[] HashFinal()
        {
            _cmacAlgorithm.TransformFinalBlock(EmptyArray<byte>.Value, 0, 0);
            return _cmacAlgorithm.Hash;
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="CMACGrasshopper"/>
        /// and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">
        /// true to release both managed and unmanaged resources;
        /// false to release only unmanaged resources.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _cmacAlgorithm.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}