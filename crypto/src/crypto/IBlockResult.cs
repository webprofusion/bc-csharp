﻿
namespace Org.BouncyCastle.Crypto
{
    /// <summary>
    /// Operators that reduce their input to a single block return an object
    /// of this type.
    /// </summary>
    public interface IBlockResult
    {
        /// <summary>
        /// Return the final result of the operation.
        /// </summary>
        /// <returns>A block of bytes, representing the result of an operation.</returns>
        byte[] DoFinal();

        /// <summary>
        /// Store the final result of the operation by copying it into the destination array.
        /// </summary>
        /// <returns>The number of bytes copied into destination.</returns>
        /// <param name="destination">The byte array to copy the result into.</param>
        /// <param name="offset">The offset into destination to start copying the result at.</param>
        int DoFinal(byte[] destination, int offset);
    }
}
