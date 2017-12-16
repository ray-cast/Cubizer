using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;

#pragma warning disable 0169 // "field x is never used"

namespace Cubizer
{
	[Serializable]
	public class CubizerProfile : ScriptableObject
	{
		public TerrainModels terrain = new TerrainModels();
		public DbModels database = new DbModels();
		public ChunkManagerModels chunk = new ChunkManagerModels();
		public BiomeManagerModels biome = new BiomeManagerModels();
		public TimeModels time = new TimeModels();
		public LiveManagerModels lives = new LiveManagerModels();
		public NetworkModels network = new NetworkModels();

		public static bool Save(string path, CubizerProfile profile)
		{
			using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
			{
				var serializer = new BinaryFormatter();
				serializer.Serialize(stream, profile);
				return true;
			}
		}

		public static CubizerProfile Load(string path)
		{
			using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				return new BinaryFormatter().Deserialize(stream) as CubizerProfile;
			}
		}
	}
}