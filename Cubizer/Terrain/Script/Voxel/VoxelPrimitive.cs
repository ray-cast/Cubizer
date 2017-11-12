namespace Cubizer
{
	public class VoxelPrimitive
	{
		public struct Vector3
		{
			public byte x;
			public byte y;
			public byte z;

			public Vector3(byte xx, byte yy, byte zz)
			{
				x = xx;
				y = yy;
				z = zz;
			}
		}

		public Vector3 begin;
		public Vector3 end;

		public VoxelMaterial material;
		public VoxelVisiableFaces faces;

		public VoxelPrimitive(Vector3 begin, Vector3 end, VoxelMaterial _material)
		{
			this.begin = begin;
			this.end = end;

			material = _material;

			faces.left = true;
			faces.right = true;
			faces.top = true;
			faces.bottom = true;
			faces.front = true;
			faces.back = true;
		}

		public VoxelPrimitive(byte begin_x, byte end_x, byte begin_y, byte end_y, byte begin_z, byte end_z, VoxelMaterial _material)
		{
			begin.x = begin_x;
			begin.y = begin_y;
			begin.z = begin_z;

			end.x = end_x;
			end.y = end_y;
			end.z = end_z;

			material = _material;

			faces.left = true;
			faces.right = true;
			faces.top = true;
			faces.bottom = true;
			faces.front = true;
			faces.back = true;
		}

		public VoxelPrimitive(byte begin_x, byte end_x, byte begin_y, byte end_y, byte begin_z, byte end_z, VoxelVisiableFaces _faces, VoxelMaterial _material)
		{
			begin.x = begin_x;
			begin.y = begin_y;
			begin.z = begin_z;

			end.x = end_x;
			end.y = end_y;
			end.z = end_z;

			material = _material;
			faces = _faces;
		}

		public void GetTranslateScale(out UnityEngine.Vector3 pos, out UnityEngine.Vector3 scale)
		{
			pos.x = (begin.x + end.x + 1) * 0.5f - 0.5f;
			pos.y = (begin.y + end.y + 1) * 0.5f - 0.5f;
			pos.z = (begin.z + end.z + 1) * 0.5f - 0.5f;

			scale.x = (end.x + 1 - begin.x);
			scale.y = (end.y + 1 - begin.y);
			scale.z = (end.z + 1 - begin.z);
		}
	}
}