using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LedgerPro.Core.Interfaces
{
    /// <summary>
    /// Defines a contract for a service that calculates a hash value for a given file stream. 
    /// This is typically used to generate a unique identifier for files, such as bank statements, to prevent duplicate imports and ensure data integrity. 
    /// The implementation of this interface would involve reading the file stream and applying a hashing algorithm (e.g., SHA256) 
    /// to produce a consistent hash string that represents the contents of the file.
    /// </summary>
    public interface IFileHasher
    {
        /// <summary>
        /// Calculates a hash string for the given file stream. The method reads the contents of the stream and applies a hashing algorithm to generate 
        /// a unique hash value that can be used to identify the file.
        /// </summary>
        /// <param name="fileStream">The file stream for which to calculate the hash.</param>
        /// <returns>The hash string representing the file's contents.</returns>
        Task<string> CalculateHashAsync(Stream fileStream);
    }
}