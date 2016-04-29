using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Facepunch.Extend;

// 
//  Read/Write string and byte arrays 
// 
namespace SilentOrbit.ProtocolBuffers
{

    public interface IProto
    {
        void WriteToStream( Stream stream );
        void ReadFromStream( Stream stream, int size, bool isDelta = false );
    }

    public static partial class ProtocolParser
    {
        static byte[] staticBuffer = new byte[1024 * 128];

        public static float ReadSingle( Stream stream )
        {
            stream.Read( staticBuffer, 0, 4 );
            return staticBuffer.ReadFloat();
        }

        public static void WriteSingle( Stream stream, float f )
        {
            staticBuffer.WriteFloat( f );
            stream.Write( staticBuffer, 0, 4 );
        }

        public static string ReadString(Stream stream)
        {
            UnityEngine.Profiler.BeginSample( "ProtoParser.ReadString" );

            int length = (int)ReadUInt32(stream);
            if ( length <= 0 )
                return string.Empty;

            string str = string.Empty;

            if ( length >= staticBuffer.Length )
            {
                UnityEngine.Profiler.BeginSample( "new Buffer" );
                byte[] buffer = new byte[length];
                UnityEngine.Profiler.EndSample();
                stream.Read( buffer, 0, length );
                str = Encoding.UTF8.GetString( buffer, 0, length );
            }
            else
            {
                stream.Read( staticBuffer, 0, length );
                str = Encoding.UTF8.GetString( staticBuffer, 0, length );
            }
                
            UnityEngine.Profiler.EndSample();

            return str;
        }

        /// <summary>
        /// Reads a length delimited byte array
        /// </summary>
        public static byte[] ReadBytes(Stream stream)
        {
            UnityEngine.Profiler.BeginSample( "ProtoParser.ReadBytes" );

            //VarInt length
            int length = (int)ReadUInt32(stream);

            //Bytes
            byte[] buffer = new byte[length];
            int read = 0;
            while (read < length)
            {
                int r = stream.Read(buffer, read, length - read);
                if (r == 0)
                    throw new ProtocolBufferException("Expected " + (length - read) + " got " + read);
                read += r;
            }

            UnityEngine.Profiler.EndSample();

            return buffer;
        }

        /// <summary>
        /// Skip the next varint length prefixed bytes.
        /// Alternative to ReadBytes when the data is not of interest.
        /// </summary>
        public static void SkipBytes(Stream stream)
        {
            int length = (int)ReadUInt32(stream);
            if (stream.CanSeek)
                stream.Seek(length, SeekOrigin.Current);
            else
                ReadBytes(stream);
        }

        public static void WriteString( Stream stream, string val )
        {
            UnityEngine.Profiler.BeginSample( "ProtoParser.WriteString" );
            var len = Encoding.UTF8.GetBytes( val, 0, val.Length, staticBuffer, 0 );

            WriteUInt32( stream, (uint)len );
            stream.Write( staticBuffer, 0, len );
            UnityEngine.Profiler.EndSample();
        }

        /// <summary>
        /// Writes length delimited byte array
        /// </summary>
        public static void WriteBytes(Stream stream, byte[] val)
        {
            WriteUInt32(stream, (uint)val.Length);
            stream.Write(val, 0, val.Length);
        }

    }
}

