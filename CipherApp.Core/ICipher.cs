using System;

namespace CipherApp.Core
{
    /// <summary>
    /// Defines the shared contract for every cipher implementation used by the application.
    /// </summary>
    public interface ICipher
    {
        string Name { get; }
        string Description { get; }
        string Encrypt(string plaintext, object key);
        string Decrypt(string ciphertext, object key);
        string Explain(object key);
    }
}

