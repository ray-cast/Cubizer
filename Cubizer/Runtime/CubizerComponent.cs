using UnityEngine;
using UnityEngine.Rendering;

namespace Cubizer
{
	internal interface ICubizerComponent
	{
		bool Active { get; }

		CubizerContext Context { get; }

		void OnEnable();
		void OnDisable();

		void Update();

		CubizerModel GetModel();
	}

	public abstract class CubizerComponent<T> : ICubizerComponent
		where T : CubizerModel
	{
		public abstract bool Active
		{
			get; set;
		}

		public virtual CubizerContext Context
		{
			get; internal set;
		}

		public virtual T Model
		{
			get; internal set;
		}

		public virtual void Init(CubizerContext _context, T _model)
		{
			Model = _model;
			Context = _context;
		}

		public virtual CubizerModel GetModel()
		{
			return Model;
		}

		public virtual void OnEnable()
		{
		}

		public virtual void OnDisable()
		{
		}

		public virtual void Update()
		{
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