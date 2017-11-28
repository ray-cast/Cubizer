using UnityEngine;

namespace Cubizer
{
	public class DbComponent : CubizerComponent<DbModels>
	{
		private string _url;
		private string _dbname;
		private DbSqlite _sqlite;
		private bool _active;

		public override bool active
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

		public DbComponent(string name)
		{
			Debug.Assert(!string.IsNullOrEmpty(name));
			_dbname = name;
			_active = true;
		}

		public override void OnEnable()
		{
			_url = model.settings.url;
			if (string.IsNullOrEmpty(_url))
			{
				_url = Application.persistentDataPath + "/";
				model.SetDefaultURL(_url);
			}

			if (this.active)
			{
				context.behaviour.events.OnLoadChunkAfter += this.OnLoadChunkDataAfter;
				context.behaviour.events.OnAddBlockBefore += this.OnAddBlockBefore;
				context.behaviour.events.OnRemoveBlockBefore += this.OnRemoveBlockBefore;

				_sqlite = new DbSqlite(_url + _dbname + ".db");
			}
		}

		public override void OnDisable()
		{
			context.behaviour.events.OnLoadChunkAfter -= this.OnLoadChunkDataAfter;
			context.behaviour.events.OnAddBlockBefore -= this.OnAddBlockBefore;
			context.behaviour.events.OnRemoveBlockBefore -= this.OnRemoveBlockBefore;

			if (_sqlite != null)
			{
				_sqlite.Dispose();
				_sqlite = null;
			}
		}

		private void OnLoadChunkDataAfter(ChunkPrimer chunk)
		{
			_sqlite.loadChunk(chunk, chunk.position.x, chunk.position.y, chunk.position.z);
		}

		private void OnAddBlockBefore(ChunkPrimer chunk, int x, int y, int z, VoxelMaterial voxel)
		{
			_sqlite.insertBlock(chunk.position.x, chunk.position.y, chunk.position.z, x, y, z, voxel.GetInstanceID());
		}

		private void OnRemoveBlockBefore(ChunkPrimer chunk, int x, int y, int z, VoxelMaterial voxel)
		{
			_sqlite.insertBlock(chunk.position.x, chunk.position.y, chunk.position.z, x, y, z, 0);
		}
	}
}