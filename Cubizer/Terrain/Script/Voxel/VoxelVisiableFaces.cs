namespace Cubizer
{
	public unsafe struct VoxelVisiableFaces
	{
		public bool left;
		public bool right;
		public bool bottom;
		public bool top;
		public bool back;
		public bool front;

		public int count
		{
			get
			{
				int facesCount = 0;
				if (left) facesCount++;
				if (right) facesCount++;
				if (top) facesCount++;
				if (bottom) facesCount++;
				if (front) facesCount++;
				if (back) facesCount++;

				return facesCount;
			}
		}

		public VoxelVisiableFaces(bool _left, bool _right, bool _bottom, bool _top, bool _back, bool _front)
		{
			left = _left;
			right = _right;
			bottom = _bottom;
			top = _top;
			back = _back;
			front = _front;
		}
	}
}