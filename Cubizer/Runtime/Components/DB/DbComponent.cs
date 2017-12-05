using UnityEngine;
using System.Threading.Tasks;

namespace Cubizer
{
	public class DbComponent : CubizerComponent<DbModels>
	{
		private string _dbUrl;
		private string _dbName;

		private DbSqlite _sqlite;

		private bool _active;

		public override bool Active
		{
			get
			{
				return _active;
			}
			set
			{
				if (_active != value)
				{
					if (value)
						this.OnEnable();
					else
						this.OnDisable();

					_active = value;
				}
			}
		}

		public DbComponent()
		{
			_active = true;
		}

		public override void OnEnable()
		{
			_dbUrl = Model.settings.url;
			if (string.IsNullOrEmpty(_dbUrl) || _dbUrl == "localhost")
			{
				_dbUrl = Application.persistentDataPath + "/";

				if (string.IsNullOrEmpty(_dbUrl))
					Model.SetDefaultURL("localhost");
			}

			_dbName = Model.settings.name;
			if (string.IsNullOrEmpty(_dbName))
			{
				_dbName = "cubizer.db";
				Model.SetDefaultName(_dbName);
			}

			Context.behaviour.Events.OnLoadChunkAfter += this.OnLoadChunkDataAfter;
			Context.behaviour.Events.OnAddBlockBefore += this.OnAddBlockBefore;
			Context.behaviour.Events.OnRemoveBlockBefore += this.OnRemoveBlockBefore;

			_sqlite = new DbSqlite(_dbUrl + _dbName);
		}

		public override void OnDisable()
		{
			Context.behaviour.Events.OnLoadChunkAfter -= this.OnLoadChunkDataAfter;
			Context.behaviour.Events.OnAddBlockBefore -= this.OnAddBlockBefore;
			Context.behaviour.Events.OnRemoveBlockBefore -= this.OnRemoveBlockBefore;

			if (_sqlite != null)
			{
				_sqlite.Dispose();
				_sqlite = null;
			}
		}

		private void OnLoadChunkDataAfter(ChunkPrimer chunk)
		{
			if (chunk != null)
				_sqlite.loadChunk(chunk, chunk.Position.x, chunk.Position.y, chunk.Position.z);
		}

		private void OnAddBlockBefore(ChunkPrimer chunk, int x, int y, int z, VoxelMaterial voxel)
		{
			Task.Run(() => { _sqlite.insertBlock(chunk.Position.x, chunk.Position.y, chunk.Position.z, x, y, z, voxel.GetInstanceID()); });
		}

		private void OnRemoveBlockBefore(ChunkPrimer chunk, int x, int y, int z, VoxelMaterial voxel)
		{
			Task.Run(() => { _sqlite.insertBlock(chunk.Position.x, chunk.Position.y, chunk.Position.z, x, y, z, 0); });
		}
	}
}