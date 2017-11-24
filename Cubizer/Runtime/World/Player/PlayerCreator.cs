using UnityEngine;

namespace Cubizer
{
	[DisallowMultipleComponent]
	[AddComponentMenu("Cubizer/PlayerCreator")]
	public class PlayerCreator : IPlayer
	{
		[SerializeField]
		private Camera _player;

		[SerializeField]
		private CubizerBehaviour _server;

		[SerializeField]
		private PlayerModel _model;

		public override Camera player
		{
			get { return _player; }
		}

		public override PlayerModel model
		{
			get { return _model; }
		}

		public void Start()
		{
			if (_server == null)
				_server = GetComponent<CubizerBehaviour>();

			if (_server == null)
				Debug.LogError("Please assign a server on the inspector.");

			if (_player == null)
				Debug.LogError("Please assign a camera on the inspector.");

			_server.AddPlayer(this);
		}

		public void Reset()
		{
			_model.Reset();
		}

		public void UpdateTransform()
		{
			_model.SetTransform(transform);
		}
	}
}