using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace OMPS
{
    public static class StringCompression
    {
        /*
        public class Deflate
        {
            public static byte[] CompressData(string inputString, string outputPath)
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(inputString);

                using FileStream outputFileStream = new(outputPath, FileMode.Create);
                using DeflateStream deflateStream = new(outputFileStream, CompressionMode.Compress);
                deflateStream.Write(inputBytes, 0, inputBytes.Length);
            }

            public static string DecompressData(string inputPath)
            {
                using FileStream inputFileStream = new(inputPath, FileMode.Open);
                using DeflateStream deflateStream = new(inputFileStream, CompressionMode.Decompress);
                using MemoryStream outputMemoryStream = new();
                deflateStream.CopyTo(outputMemoryStream);
                byte[] decompressedBytes = outputMemoryStream.ToArray();
                return Encoding.UTF8.GetString(decompressedBytes);
            }
        }
        */

        public static class GZip
        {
            /// <summary>
            /// Compresses a string using GZip and returns a Base64 encoded string.
            /// </summary>
            /// <param name="uncompressedString">The string to compress.</param>
            /// <returns>A Base64 encoded string representing the GZip compressed data.</returns>
            public static MemoryStream CompressString(string uncompressedString)
            {
                byte[] uncompressedBytes = Encoding.UTF8.GetBytes(uncompressedString);
                //Debug.WriteLine(uncompressedBytes.Length);
                var memoryStream = new MemoryStream();
                using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
                {
                    gzipStream.Write(uncompressedBytes, 0, uncompressedBytes.Length);
                }
                memoryStream.Position = 0;
                return memoryStream;
                //return Encoding.UTF8.GetString(memoryStream.ToArray());
            }

            /// <summary>
            /// Decompresses a Base64 encoded GZip compressed string.
            /// </summary>
            /// <param name="compressedString">The Base64 encoded GZip compressed string.</param>
            /// <returns>The decompressed string.</returns>
            public static string DecompressString(string compressedString)
            {
                byte[] compressedBytes = Convert.FromBase64String(compressedString);
                var memoryStream = new MemoryStream(compressedBytes);
                var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress);
                var resultStream = new MemoryStream();
                gzipStream.CopyTo(resultStream);
                gzipStream.Close();
                return Encoding.UTF8.GetString(resultStream.ToArray());
            }
        }
    }
}
