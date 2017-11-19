using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;

namespace Cubizer
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Terrain))]
	[AddComponentMenu("Cubizer/TerrainLoader")]
	public class TerrainLoader : MonoBehaviour
	{
		public string rootPath;
		public string username = "/SaveData/";

		private Terrain _terrain;

		private void Start()
		{
			_terrain = GetComponent<Terrain>();
			_terrain.events.onSaveChunkData += this.OnSaveData;
			_terrain.events.onLoadChunkData += this.OnLoadData;

			rootPath = Application.persistentDataPath;

			CreateDirectory(rootPath);
		}

		private void Reset()
		{
			rootPath = Application.persistentDataPath;
		}

		private void OnSaveData(GameObject chunk)
		{
			var data = chunk.GetComponent<ChunkData>();
			if (data != null)
			{
				var archive = "chunk" + "_" + data.chunk.position.x + "_" + data.chunk.position.y + "_" + data.chunk.position.z;
				CreateFile(archive, data.chunk);
			}
		}

		private bool OnLoadData(int x, int y, int z, out ChunkPrimer chunk)
		{
			var archive = "chunk" + "_" + x + "_" + y + "_" + z;
			if (IsFileExists(archive))
				chunk = Load(archive);
			else
				chunk = null;

			return chunk != null;
		}

		private bool IsFileExists(string archive)
		{
			return File.Exists(rootPath + username + archive);
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

			using (var stream = new FileStream(rootPath + username + archive, FileMode.Create, FileAccess.Write))
			{
				var serializer = new BinaryFormatter();
				serializer.Serialize(stream, data.voxels);
			}
		}

		private ChunkPrimer Load(string archive)
		{
			using (var stream = new FileStream(rootPath + username + archive, FileMode.Open, FileAccess.Read))
			{
				return new BinaryFormatter().Deserialize(stream) as ChunkPrimer;
			}
		}
	}
}