using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleMesh.Service
{
    static public class Utility
    {
        public static byte[] MessagePack(Message message, TypeList lookuptable)
        {
            byte[] header = new byte[8];
            byte[] bytescratch;
            bytescratch = SimpleMesh.Utility.ToNetworkOrder(message.DataLength);
            header[0] = bytescratch[0];
            header[1] = bytescratch[1];
            bytescratch = SimpleMesh.Utility.ToNetworkOrder(message.Conversation);
            header[2] = bytescratch[0];
            header[3] = bytescratch[1];
            bytescratch = SimpleMesh.Utility.ToNetworkOrder(lookuptable.ByName(message.Type).TypeID);
            header[4] = bytescratch[0];
            header[5] = bytescratch[1];
            bytescratch = SimpleMesh.Utility.ToNetworkOrder(message.Sequence);
            header[6] = bytescratch[0];
            header[7] = bytescratch[1];
            byte[] rv = new byte[header.Length + message.Payload.Length];
            System.Buffer.BlockCopy(header, 0, rv, 0, header.Length);
            System.Buffer.BlockCopy(message.Payload, 0, rv, header.Length, message.Payload.Length);
            return rv;
        }
        public static void DebugMessage(int number, string message)
        {
            Console.Write(message + "\n\r");
        }
        public static long ToUnixTimestamp(System.DateTime dt)
        {
            DateTime unixRef = new DateTime(1970, 1, 1, 0, 0, 0);
            return (dt.Ticks - unixRef.Ticks) / 10000000;
        }
        public static DateTime FromUnixTimestamp(long timestamp)
        {
            DateTime unixRef = new DateTime(1970, 1, 1, 0, 0, 0);
            return unixRef.AddSeconds(timestamp);
        }

    }
}
