namespace Cubizer.Models
{
	public struct VOXVisiableFaces
	{
		public bool left;
		public bool right;
		public bool bottom;
		public bool top;
		public bool back;
		public bool front;

		public bool Any
		{
			get
			{
				return left | right | bottom | top | front | back;
			}
		}

		public int Count
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

		public bool this[int index]
		{
			get
			{
				switch (index)
				{
					case 0: return left;
					case 1: return right;
					case 2: return top;
					case 3: return bottom;
					case 4: return front;
					case 5: return back;
					default: throw new System.ArgumentOutOfRangeException("Out of index:" + index);
				}
			}
		}

		public VOXVisiableFaces(bool all = true)
		{
			left = all;
			right = all;
			bottom = all;
			top = all;
			front = all;
			back = all;
		}

		public VOXVisiableFaces(bool _left, bool _right, bool _bottom, bool _top, bool _back, bool _front)
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