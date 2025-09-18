using System;
using System.Xml;

namespace PacketGenerator
{
    class Program
    {
        static void Main(string[] args) 
        {
            XmlReaderSettings settings = new XmlReaderSettings()
            {
                IgnoreComments = true,
                IgnoreWhitespace = true,
            };

            using (XmlReader r = XmlReader.Create("PDL.xml", settings))
            {
                r.MoveToContent();

                while (r.Read())
                {
                    if (r.Depth == 1 && r.NodeType == XmlNodeType.Element)
                        ParsePacket(r);
                    //Console.WriteLine(r.Name + " " + r["name"]);
                }
            }
        }

        public static void ParsePacket(XmlReader r) 
        {
            if (r.NodeType == XmlNodeType.EndElement) 
                return;

            if (r.Name.ToLower() != "packet")
            {
                Console.WriteLine("Invalid Packet Node");
                return;
            }

            string packetName = r["name"];
            if (string.IsNullOrEmpty(packetName))
            {
                Console.WriteLine("Packet Without Name");
                return;
            }

            ParseMembers(r);
        }

        public static void ParseMembers(XmlReader r)
        {
            string packetName = r["name"];

            int depth = r.Depth + 1;

            while (r.Read()) 
            {
                if (r.Depth != depth)
                    break;


            }
        }
    }
}