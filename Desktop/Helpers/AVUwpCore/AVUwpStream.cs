using System;
using System.Diagnostics;
using System.IO;
using Windows.Storage.Streams;

public static class AVUwpStream
{
    public static Stream AsStream(this IRandomAccessStream windowsRuntimeStream)
    {
        try
        {
            return windowsRuntimeStream.AsStreamForWrite();
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Failed to convert UWP random access stream: " + ex.Message);
            return null;
        }
    }
}