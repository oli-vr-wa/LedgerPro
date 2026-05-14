using System.Security.Cryptography;
using LedgerPro.Core.Interfaces;

namespace LedgerPro.Infrastructure.Services;

/// <summary>
/// Implements the IFileHasher interface to provide functionality for calculating a hash value for a given file stream.
/// </summary>
public class FileHasher : IFileHasher
{
    /// <summary>
    /// Calculates a hash string for the given file stream. The method reads the contents of the stream and applies a hashing algorithm to generate
    /// a unique hash value that can be used to identify the file. This is typically used to prevent duplicate imports of bank statements by generating 
    /// a consistent hash based on the file's contents.
    /// </summary>
    /// <param name="fileStream">The file stream for which to calculate the hash.</param>
    /// <returns>The hash string representing the file's contents.</returns>
    /// <exception cref="ArgumentException">Thrown when the file stream is null or cannot be read.</exception>
    public async Task<string> CalculateHashAsync(Stream fileStream)
    {
        if (fileStream == null || !fileStream.CanRead)
        {
            throw new ArgumentException("File stream is null or cannot be read.");
        }

        using var sha256 = SHA256.Create();
        fileStream.Position = 0; // Ensure we start reading from the beginning of the stream
        var hashBytes = await sha256.ComputeHashAsync(fileStream);
        fileStream.Position = 0; // Reset stream position after hashing

        // Convert the hash bytes to a hexadecimal string
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
    }
}
