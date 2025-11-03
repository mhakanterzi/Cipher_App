using System;

namespace CipherApp.Core
{
    public interface ICipher
    {
        string Name { get; }
        string Description { get; }
        string Encrypt(string plaintext, object key);
        string Decrypt(string ciphertext, object key);
        string Explain(object key);
    }
}

