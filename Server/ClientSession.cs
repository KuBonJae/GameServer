using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class ClientSession : PacketSession
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected bytes : {endPoint}");

            //Packet packet = new Packet() { size = 100, packetID = 10 };
            //
            //ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
            //byte[] buffer = BitConverter.GetBytes(packet.size);
            //byte[] buffer2 = BitConverter.GetBytes(packet.packetID);
            //Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length);
            //Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length);
            //ArraySegment<byte> sendBuffer = SendBufferHelper.Close(buffer.Length + buffer2.Length);

            //Send(sendBuffer);
            Thread.Sleep(5000);
            Disconnect();
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            ushort count = 0;

            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            count += 2;
            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += 2;

            switch((PacketID)id)
            {
                case PacketID.PlayerInfoReq:
                    PlayerInfoReq p = new PlayerInfoReq();
                    p.Read(buffer);
                    Console.WriteLine($"PlayerInfoReq : {p.playerId} / {p.name}");

                    foreach(PlayerInfoReq.Skill skill in p.skills)
                    {
                        Console.WriteLine($"Skill {skill.id} / {skill.level} / {skill.duration}");
                    }

                    break;

                case PacketID.Test:

                    break;
            }
            Console.WriteLine($"RecvPacketID : {id} / Size : {size}");
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected bytes : {endPoint}");
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transfered bytes : {numOfBytes}");
        }
    }
}
