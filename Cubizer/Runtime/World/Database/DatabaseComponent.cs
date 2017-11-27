using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;

namespace Cubizer
{
	public class DatabaseComponent : CubizerComponent<DatabaseModels>
	{
		private string _url;
		private string _username;

		public override void OnEnable()
		{
			_url = model.settings.url;
			if (string.IsNullOrEmpty(_url))
				_url = Application.persistentDataPath;

			context.behaviour.events.onSaveChunkData += this.OnSaveData;
			context.behaviour.events.onLoadChunkData += this.OnLoadData;
		}

		public override void OnDisable()
		{
			base.OnDisable();
		}

		private void OnSaveData(GameObject chunk)
		{
			/*var data = chunk.GetComponent<ChunkData>();
			if (data != null)
			{
				var archive = "chunk" + "_" + data.chunk.position.x + "_" + data.chunk.position.y + "_" + data.chunk.position.z;
				CreateFile(archive, data.chunk);
			}*/
		}

		private bool OnLoadData(int x, int y, int z, out ChunkPrimer chunk)
		{
			/*var archive = "chunk" + "_" + x + "_" + y + "_" + z;
			if (IsFileExists(archive))
				chunk = Load(archive);
			*/
			chunk = null;
			return chunk != null;
		}

		private bool IsFileExists(string archive)
		{
			return File.Exists(_url + _username + archive);
		}

		private bool IsDirectoryExists(string archive)
		{
			return Directory.Exists(archive);
		}

		private void CreateDirectory(string archive)
		{
			if (IsDirectoryExists(archive))
				return;
			Directory.CreateDirectory(archive);
		}

		private void CreateFile(string archive, ChunkPrimer data)
		{
			Debug.Assert(data != null);

			using (var stream = new FileStream(_url + _username + archive, FileMode.Create, FileAccess.Write))
			{
				var serializer = new BinaryFormatter();
				serializer.Serialize(stream, data.voxels);
			}
		}

		private ChunkPrimer Load(string archive)
		{
			using (var stream = new FileStream(_url + _username + archive, FileMode.Open, FileAccess.Read))
			{
				return new BinaryFormatter().Deserialize(stream) as ChunkPrimer;
			}
		}
	}
}