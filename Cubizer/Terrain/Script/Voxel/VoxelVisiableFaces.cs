namespace Cubizer
{
	public struct VoxelVisiableFaces
	{
		public bool left;
		public bool right;
		public bool bottom;
		public bool top;
		public bool back;
		public bool front;

		public bool any
		{
			get
			{
				return left | right | bottom | top | front | back;
			}
		}

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

		public VoxelVisiableFaces(bool all = true)
		{
			left = all;
			right = all;
			bottom = all;
			top = all;
			front = all;
			back = all;
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