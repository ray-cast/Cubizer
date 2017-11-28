using System;
using Mono.Data.Sqlite;

using UnityEngine;

namespace Cubizer
{
	public class DbSqlite : IDisposable
	{
		private string _path;

		private SqliteConnection _dbConnection;

		private const string queryCreateDB =
			"attach database 'auth.db' as auth;" +
			"create table if not exists auth.identity_token (username text not null, token text not null, selected int not null);" +
			"create table if not exists block (x int not null, y int not null, z int not null, xx int not null, yy int not null, zz int not null, ww int not null);" +
			"create unique index if not exists auth.identity_token_username_idx on identity_token (username);" +
			"create unique index if not exists block_idx on block (x, y, z, xx, yy, zz);";

		private const string queryInsertBlock = "insert or replace into block (x, y, z, xx, yy, zz, ww) values (?, ?, ?, ?, ?, ?, ?);";
		private const string queryLoadBlocks = "select xx, yy, zz, ww from block where x = ? and y = ? and z = ?;";

		public DbSqlite()
		{
		}

		public DbSqlite(string path)
		{
			OpenDB(path);
		}

		~DbSqlite()
		{
			this.Dispose();
		}

		public void OpenDB(string path)
		{
			try
			{
				Debug.Log("Database Start");
				Debug.Log("Connect to db:" + path);

				_dbConnection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = path }.ToString());
				_dbConnection.Open();

				Debug.Log("Connected to db:" + path);

				this.ExecuteNonQuery(queryCreateDB);

				_path = path;

				Debug.Log("Database Starting");
			}
			catch (Exception e)
			{
				this.Dispose();
				throw e;
			}
		}

		public void Dispose()
		{
			if (_dbConnection != null)
			{
				_dbConnection.Close();
				_dbConnection = null;

				Debug.Log("Disconnected from db :" + _path);
			}
		}

		public void ExecuteNonQuery(string sqlquery)
		{
			using (var dbCommand = _dbConnection.CreateCommand())
			{
				dbCommand.CommandText = sqlquery;
				dbCommand.ExecuteNonQuery();
			}
		}

		public void loadChunk(ChunkPrimer chunk, int x, int y, int z)
		{
			using (var _dbCommandLoadBlocks = _dbConnection.CreateCommand())
			{
				_dbCommandLoadBlocks.CommandText = queryLoadBlocks;
				_dbCommandLoadBlocks.Parameters.Add(new SqliteParameter("x", x));
				_dbCommandLoadBlocks.Parameters.Add(new SqliteParameter("y", y));
				_dbCommandLoadBlocks.Parameters.Add(new SqliteParameter("z", z));

				using (var dbReader = _dbCommandLoadBlocks.ExecuteReader())
				{
					if (dbReader.HasRows)
					{
						while (dbReader.Read())
						{
							var xx = dbReader.GetByte(0);
							var yy = dbReader.GetByte(1);
							var zz = dbReader.GetByte(2);
							var ww = dbReader.GetByte(3);
							if (ww > 0)
								chunk.voxels.Set(xx, yy, zz, VoxelMaterialManager.GetInstance().GetMaterial(ww));
							else
								chunk.voxels.Set(xx, yy, zz, null);
						}
					}
				}
			}
		}

		public void insertBlock(int x, int y, int z, int xx, int yy, int zz, int ww)
		{
			using (var _dbCommandInsertBlock = _dbConnection.CreateCommand())
			{
				_dbCommandInsertBlock.CommandText = queryInsertBlock;
				_dbCommandInsertBlock.Parameters.Add(new SqliteParameter("x", x));
				_dbCommandInsertBlock.Parameters.Add(new SqliteParameter("y", y));
				_dbCommandInsertBlock.Parameters.Add(new SqliteParameter("z", z));
				_dbCommandInsertBlock.Parameters.Add(new SqliteParameter("xx", xx));
				_dbCommandInsertBlock.Parameters.Add(new SqliteParameter("yy", yy));
				_dbCommandInsertBlock.Parameters.Add(new SqliteParameter("zz", zz));
				_dbCommandInsertBlock.Parameters.Add(new SqliteParameter("ww", ww));
				_dbCommandInsertBlock.ExecuteNonQuery();
			}
		}
	}
}