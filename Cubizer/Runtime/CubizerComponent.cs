using UnityEngine;
using UnityEngine.Rendering;

namespace Cubizer
{
	interface ICubizerComponent
	{
		bool active { get; }

		CubizerContext context { get; }

		void OnEnable();
		void OnDisable();

		CubizerModel GetModel();
	}

	public abstract class CubizerComponent<T> : ICubizerComponent
		where T : CubizerModel
	{
		public virtual bool active
		{
			get; set;
		}

		public CubizerContext context
		{
			get; internal set;
		}

		public T model
		{
			get; internal set;
		}

		public virtual void OnEnable() { }
		public virtual void OnDisable() { }

		public virtual void Init(CubizerContext _context, T _model)
		{
			model = _model;
			context = _context;
		}

		public CubizerModel GetModel()
		{
			return model;
		}
	}

	public abstract class CubizerComponentCommandBuffer<T> : CubizerComponent<T>
		where T : CubizerModel
	{
		public abstract CameraEvent GetCameraEvent();

		public abstract string GetName();

		public abstract void PopulateCommandBuffer(CommandBuffer cb);
	}

	public abstract class CubizerComponentRenderTexture<T> : CubizerComponent<T>
		where T : CubizerModel
	{
		public virtual void Prepare(Material material)
		{
		}
	}
}
