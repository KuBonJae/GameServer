START  /WAIT ..\..\PacketGenerator\bin\Debug\PacketGenerator.exe ..\..\PacketGenerator\PDL.xml
XCOPY GenPackets.cs ..\..\DummyClient\Packet /Y 
XCOPY GenPackets.cs ..\..\Server\Packet /Y 
XCOPY ClientPacketManager.cs ..\..\DummyClient\Packet /Y 
XCOPY ServerPacketManager.cs ..\..\Server\Packet /Y 