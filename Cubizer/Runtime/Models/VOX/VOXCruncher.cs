namespace Cubizer.Models
{
	using VOXMaterial = System.Int32;

	public sealed class VOXCruncher
	{
		public struct Vector3
		{
			public int x;
			public int y;
			public int z;

			public Vector3(int xx, int yy, int zz)
			{
				x = xx;
				y = yy;
				z = zz;
			}
		}

		public Vector3 begin;
		public Vector3 end;

		public VOXMaterial material;
		public VOXVisiableFaces faces;

		public VOXCruncher(Vector3 begin, Vector3 end, VOXMaterial _material)
		{
			this.begin = begin;
			this.end = end;
			this.material = _material;
			this.faces = new VOXVisiableFaces(true);
		}

		public VOXCruncher(int begin_x, int end_x, int begin_y, int end_y, int begin_z, int end_z, VOXMaterial _material)
		{
			this.begin = new Vector3(begin_x, begin_y, begin_z);
			this.end = new Vector3(end_x, end_y, end_z);
			this.material = _material;
			this.faces = new VOXVisiableFaces(true);
		}

		public VOXCruncher(int begin_x, int end_x, int begin_y, int end_y, int begin_z, int end_z, VOXVisiableFaces _faces, VOXMaterial _material)
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
	}
}