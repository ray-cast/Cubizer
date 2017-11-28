using UnityEngine;

namespace Cubizer
{
	public class DbComponent : CubizerComponent<DbModels>
	{
		private string _url;
		private string _dbname;
		private DbSqlite _sqlite;

		public DbComponent(string name)
		{
			Debug.Assert(!string.IsNullOrEmpty(name));
			_dbname = name;
		}

		public override void OnEnable()
		{
			_url = model.settings.url;
			if (string.IsNullOrEmpty(_url))
				_url = Application.persistentDataPath + "/";

			context.behaviour.events.OnLoadChunkAfter += this.OnLoadChunkDataAfter;

			context.behaviour.events.OnAddBlockBefore += this.OnAddBlockBefore;
			context.behaviour.events.OnRemoveBlockBefore += this.OnRemoveBlockBefore;

			_sqlite = new DbSqlite(_url + _dbname + ".db");
		}

		public override void OnDisable()
		{
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