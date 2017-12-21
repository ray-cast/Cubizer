using System;
using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Play.Clientbound
{
	[Packet(Packet)]
	public class SpawnMob : IPacketSerializable
	{
		public const int Packet = 0x03;

		public uint entityID;
		public Guid entityUUID;
		public uint type;
		public double x;
		public double y;
		public double z;
		public byte yaw;
		public byte pitch;
		public byte head;
		public short velocityX;
		public short velocityY;
		public short velocityZ;
		public byte metaData;

		public uint packetId
		{
			get
			{
				return Packet;
			}
		}

		public object Clone()
		{
			return new SpawnMob();
		}

		public void Deserialize(NetworkReader br)
		{
			br.ReadVarInt(out entityID);
			br.Read(out entityUUID);
			br.ReadVarInt(out type);
			br.Read(out x);
			br.Read(out y);
			br.Read(out z);
			br.Read(out yaw);
			br.Read(out pitch);
			br.Read(out head);
			br.Read(out velocityX);
			br.Read(out velocityY);
			br.Read(out velocityZ);
			br.Read(out metaData);
		}

		public void Serialize(NetworkWrite bw)
		{
			bw.WriteVarInt(entityID);
			bw.Write(entityUUID);
			bw.WriteVarInt(type);
			bw.Write(x);
			bw.Write(y);
			bw.Write(z);
			bw.Write(yaw);
			bw.Write(pitch);
			bw.Write(head);
			bw.Write(velocityX);
			bw.Write(velocityY);
			bw.Write(velocityZ);
			bw.Write(metaData);
		}
	}
}