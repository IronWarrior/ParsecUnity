using ParsecGaming;
using System;
using System.Runtime.InteropServices;

public static class ParsecExtensions
{
    // Parsec encodes submitted user data from strings into null-terminated UTF-8 bytes.
    public static string HostGetUserDataString(this Parsec parsec, uint key)
    {
        IntPtr ptr = parsec.GetBuffer(key);

        int length = 0;

        while (Marshal.ReadByte(ptr, length) != 0)
            ++length;

        byte[] buffer = new byte[length];
        Marshal.Copy(ptr, buffer, 0, buffer.Length);

        string str = System.Text.Encoding.UTF8.GetString(buffer);

        Parsec.Free(ptr);

        return str;
    }
}
