//using System;
//using System.Data;
//using Mono.Data.Sqlite;
//using System.Collections.Generic;
//
//namespace iTunesRatingSync
//{
//	public class Database
//	{
//
//		private SqliteConnection con = new SqliteConnection("URI=file:ratings.db");
//
//		public Database()
//		{
//			con.Open();
//			//new SqliteCommand("drop table track", con).ExecuteNonQuery();
//			//new SqliteCommand("drop table playlist", con).ExecuteNonQuery();
//			try 
//			{
//				new SqliteCommand("create table track(hash varchar(100), name varchar(300), artist varchar(300), album varchar(300), genre varchar(300), time int, rating int, comments varchar(500), iTunesId int)", con).ExecuteNonQuery();				
//			} 
//			catch {}
//			try
//			{
//				new SqliteCommand("create table Playlist(name varchar(300), trackhash varchar(100), iTunesId int)", con).ExecuteNonQuery();	
//			}
//			catch{}
//		}
//
//		public List<Track> ListTracks(int minRating)
//		{
//			List<Track> ret = new List<Track>();
//			string sql = "select t.hash, t.name, t.artist, t.album, t.genre, t.time, t.rating, t.comments, p.name, t.iTunesId " +
//				"from Track t " +
//				"left join Playlist p on t.hash = p.trackhash";
//			sql += " where t.rating >= " + minRating;
//			sql += " order by p.name, t.rating t.desc, t.name, t.album, t.artist";
//			SqliteDataReader r = new SqliteCommand(sql, con).ExecuteReader();
//			while (r.Read())
//			{
//				Track t = new Track() 
//				{ 
//					Hash= r.GetString(0).Trim(), 
//					TrackName = r.GetString(1).Trim(), 
//					Artist = r.GetString(2).Trim(), 
//					Album = r.GetString(3).Trim(), 
//					Genre = r.GetString(4).Trim(), 
//					Time = r.GetInt32(5), 
//					Rating = r.GetInt16(6), 
//					Comments = r.GetString(7) ?? "",
//					Playlist = r.GetValue(8) != DBNull.Value ? r.GetString(8) : "",
//					iTunesId = r.GetInt32(9)
//				};
//				t.GenerateHash();
//				ret.Add(t);
//			}
//			return ret;
//		}
//
//		public Track GetTrack(string hash)
//		{
//			string sql = "select t.hash, t.name, t.artist, t.album, t.genre, t.time, t.rating, t.comments, p.name, t.iTunesId " +
//				"from Track t " +
//				"left join Playlist p on t.hash = p.trackhash " + 
//				"where t.hash = '" + hash + "'";
//			try 
//			{
//				SqliteDataReader r = new SqliteCommand(sql, con).ExecuteReader();
//				if (r.Read())
//				{
//					Track t = new Track() 
//					{ 
//						Hash= r.GetString(0).Trim(), 
//						TrackName = r.GetString(1).Trim(), 
//						Artist = r.GetString(2).Trim(), 
//						Album = r.GetString(3).Trim(), 
//						Genre = r.GetString(4).Trim(), 
//						Time = r.GetInt32(5), 
//						Rating = r.GetInt16(6), 
//						Comments = r.GetString(7) ?? "",
//						Playlist = r.GetValue(8) != DBNull.Value ? r.GetString(8) : "",
//						iTunesId = r.GetInt32(9)
//					};
//					t.GenerateHash();
//					return t;
//				}
//				else
//				{
//					return null;
//				}
//			} 
//			catch (Exception e)
//			{
//				Console.WriteLine("Running: " + sql + " caused " + e.Message);
//				throw e;
//			}
//		}
//
//		public void SaveTrack(Track t)
//		{
//			//Console.Write("Updating Database . . . ");
//			new SqliteCommand("delete from track " +
//                  "where hash = '" + t.Hash + "'", con).ExecuteNonQuery();
//			new SqliteCommand("insert into track values('" + 
//                  t.Hash + "', '" + 
//                  t.TrackName.Replace ("'", "''") + "', '" + 
//                  t.Artist.Replace ("'", "''") + "', '" + 
//                  t.Album.Replace ("'", "''") + "', '" + 
//                  t.Genre.Replace ("'", "''") + "', " + 
//                  t.Time + ", " + 
//                  t. Rating + ", '" +
//                  t.Comments.Replace ("'", "''") + "', " +
//                  t.iTunesId.ToString () + ")", con).ExecuteNonQuery();
//			//Console.WriteLine("OK");
//		}
//
//	}
//}
//
