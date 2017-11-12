using UnityEngine;

namespace Cubizer
{
	public abstract class ChunkGenerator : MonoBehaviour
	{
		public void InvokeDefaultOnEnable()
		{
			if (transform.parent != null)
			{
				var generator = transform.parent.GetComponent<ChunkGeneratorManager>();
				generator.OnGeneratorEnable(this);
			}
		}

		public void InvokeDefaultOnDisable()
		{
			if (transform.parent != null)
			{
				var generator = transform.parent.GetComponent<ChunkGeneratorManager>();
				generator.OnGeneratorDisable(this);
			}
		}

		public void OnEnable()
		{
			this.InvokeDefaultOnEnable();
		}

		public void OnDestroy()
		{
			this.InvokeDefaultOnDisable();
		}

		public abstract void OnCreateChunk(ChunkPrimer map);
	}
}