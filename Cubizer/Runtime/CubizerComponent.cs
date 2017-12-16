using UnityEngine;
using UnityEngine.Rendering;

namespace Cubizer
{
	internal interface ICubizerComponent
	{
		bool active { get; }

		CubizerContext context { get; }

		void OnEnable();
		void OnDisable();

		void Update();

		CubizerModel GetModel();
	}

	public abstract class CubizerComponent<T> : ICubizerComponent
		where T : CubizerModel
	{
		public abstract bool active
		{
			get; set;
		}

		public virtual CubizerContext context
		{
			get; internal set;
		}

		public virtual T model
		{
			get; internal set;
		}

		public virtual void Init(CubizerContext _context, T _model)
		{
			model = _model;
			context = _context;
		}

		public virtual CubizerModel GetModel()
		{
			return model;
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

		internal ICubizerComponent GetComponent(string name)
		{
			return context.behaviour.GetCubizerComponent(name);
		}

		internal ICubizerComponent GetComponent(System.Type type)
		{
			return context.behaviour.GetCubizerComponent(type);
		}

		internal ICubizerComponent GetComponent<_Tx>() where _Tx : ICubizerComponent
		{
			return context.behaviour.GetCubizerComponent<_Tx>();
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