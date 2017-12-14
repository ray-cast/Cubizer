using System;

namespace Cubizer.Models
{
	public struct VoxFileHeader
	{
		public byte[] header;
		public Int32 version;
	}

	public struct VoxFilePack
	{
		public byte[] name;
		public Int32 chunkContent;
		public Int32 chunkNums;
		public Int32 modelNums;
	}

	public struct VoxFileSize
	{
		public byte[] name;
		public Int32 chunkContent;
		public Int32 chunkNums;
		public Int32 x;
		public Int32 y;
		public Int32 z;
	}

	public struct VoxFileXYZI
	{
		public byte[] name;
		public Int32 chunkContent;
		public Int32 chunkNums;
		public VoxData voxels;
	}

	public struct VoxFileRGBA
	{
		public byte[] name;
		public Int32 chunkContent;
		public Int32 chunkNums;
		public uint[] values;
	}

	public struct VoxFileChunkChild
	{
		public VoxFileSize size;
		public VoxFileXYZI xyzi;
	}

	public struct VoxFileChunk
	{
		public byte[] name;
		public Int32 chunkContent;
		public Int32 chunkNums;
	}

	public struct VoxFileMaterial
	{
		public int id;
		public int type;
		public float weight;
		public int propertyBits;
		public float[] propertyValue;
	}

	public sealed class VoxFileData
	{
		public VoxFileHeader hdr;
		public VoxFileChunk main;
		public VoxFilePack pack;
		public VoxFileChunkChild[] chunkChild;
		public VoxFileRGBA palette;
	}
}