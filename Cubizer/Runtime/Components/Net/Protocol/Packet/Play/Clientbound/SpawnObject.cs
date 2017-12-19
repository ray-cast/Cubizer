using System;
using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Play.Clientbound
{
	[Packet(Packet)]
	public class SpawnObject : IPacketSerializable
	{
		public const int Packet = 0x0;

		public uint entityID;
		public Guid objectUUID;
		public byte type;
		public double x;
		public double y;
		public double z;
		public byte yaw;
		public byte pitch;
		public int data;
		public short velocityX;
		public short velocityY;
		public short velocityZ;

		public uint packetId
		{
			get
			{
				return Packet;
			}
		}

		public object Clone()
		{
			return new SpawnObject();
		}

		public void Deserialize(NetworkReader br)
		{
			/*br.ReadVarInt(out entityID);
			br.Read(out objectUUID);
			br.Read(out type);
			br.Read(out x);
			br.Read(out y);
			br.Read(out z);
			br.Read(out yaw);
			br.Read(out pitch);
			br.Read(out data);
			br.Read(out velocityX);
			br.Read(out velocityY);
			br.Read(out velocityZ);*/
		}

		public void Serialize(NetworkWrite bw)
		{
			bw.WriteVarInt(entityID);
			bw.Write(objectUUID);
			bw.Write(type);
			bw.Write(x);
			bw.Write(y);
			bw.Write(z);
			bw.Write(yaw);
			bw.Write(pitch);
			bw.Write(data);
			bw.Write(velocityX);
			bw.Write(velocityY);
			bw.Write(velocityZ);
		}
	}
}