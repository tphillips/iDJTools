using System;
using Mono.Data.Sqlite;
using System.Collections.Generic;

namespace iTunesPlaylists
{
	public class iDJDatabase : IDisposable
	{

		/*
		CREATE TABLE crate ( id INTEGER PRIMARY KEY, crateindex INTEGER, trackid INTEGER);
		CREATE TABLE letterpicker ( id INTEGER PRIMARY KEY, name STRING);
		CREATE TABLE metametadata ( key STRING PRIMARY KEY, value STRING);
		CREATE TABLE playlists ( id INTEGER PRIMARY KEY, title STRING, title_canonical STRING, path STRING, hash STRING);
		CREATE TABLE playlisttracks ( id INTEGER, playlistorder INTEGER, trackid INTEGER);
		CREATE TABLE search_cesspit ( trackid STRING, type INTEGER, string STRING);
		CREATE TABLE tracks ( id INTEGER PRIMARY KEY, path STRING, format STRING, title STRING, title_canonical STRING, album STRING, album_canonical STRING, artist STRING, artist_canonical STRING, year INTEGER, genre STRING, genre_canonical STRING, tracknum INTEGER, tempo FLOAT, length INTEGER, samplerate INTEGER, numchannels INTEGER, beat_phase INTEGER, real_start INTEGER, real_end INTEGER, original_id STRING);
		CREATE INDEX playlisttracks_playlistorder_4 ON playlisttracks(playlistorder);
		CREATE INDEX tracks_album_canonical_4 ON tracks(album_canonical);
		CREATE INDEX tracks_artist_canonical_4 ON tracks(artist_canonical);
		CREATE INDEX tracks_genre_canonical_4 ON tracks(genre_canonical);
		CREATE INDEX tracks_tempo_4 ON tracks(tempo);
		CREATE INDEX tracks_title_canonical_4 ON tracks(title_canonical);
		CREATE INDEX tracks_tracknum_4 ON tracks(tracknum);
		CREATE INDEX tracks_year_4 ON tracks(year);
		*/

		private const string BLOCKED_PLAYLISTS = "Library, Music, Movies, Podcasts, iTunes U, Music Videos, Recently Played";

		SqliteConnection con;

		public iDJDatabase (string dbPath)
		{
			Backup(dbPath);
			con = new SqliteConnection("URI=file:" + dbPath);
			con.Open();
		}

		private void Backup(string dbPath)
		{
			string backupSuffix = new DateTime().ToString("ddMMyyHHmmss");
			System.IO.File.Copy(dbPath, dbPath + backupSuffix);
		}

		public void DeleteiDJPlaylists()
		{
			string sql = "delete from playlisttracks";
			SqliteCommand c = new SqliteCommand(sql, con);
			c.ExecuteNonQuery();
			sql = "delete from playlists";
			c = new SqliteCommand(sql, con);
			c.ExecuteNonQuery();
		}

		public bool CreateiDJPlaylists(List<Playlist> playlists, bool closeAfter)
		{
			foreach(Playlist p in playlists)
			{
				if (!CreateiDJPlaylist(p, false)) { return false; }
			}
			if (closeAfter) { con.Close (); }
			return true;
		}

		public bool CreateiDJPlaylist(Playlist p, bool closeAfter)
		{
			if (BLOCKED_PLAYLISTS.Contains(p.Name)) { return false; }
			int iDjId = CreatePlaylist(p.Name);
			if (iDjId == -1) return false;
			int order = 0;
			foreach(Track t in p.Tracks)
			{
				int trackId = FindiDJTrackID (t.TrackName, t.Album);
				if (trackId >= 0)
				{
					LinkTrackToPlaylist(trackId, iDjId, order);
					order ++;
				}
			}
			if (closeAfter) { con.Close (); }
			return true;
		}

		private int CreatePlaylist(string Name)
		{
			string sql = "select max(id) from playlists";
			SqliteCommand c = new SqliteCommand(sql, con);
			var res = c.ExecuteScalar();
			int ret = 0;
			if (res != null && res != DBNull.Value)
			{
				ret = int.Parse (res.ToString ());
				ret ++;
			} 
			sql = string.Format("insert into playlists values({0}, '{1}', '{2}', '', '')", 
			                    ret, Name.Replace("'", ""), Name.ToUpper().Replace("'", ""));
			c = new SqliteCommand(sql, con);
			if (c.ExecuteNonQuery () > 0)
			{
				return ret;
			}
			else
			{
				return -1;
			}
		}

		private int FindiDJTrackID(string trackName, string album)
		{
			string sql = string.Format("select id from tracks where title = '{0}' and album = '{1}'", 
			                           trackName.Replace ("'", "''"), album.Replace ("'", "''"));
			SqliteCommand c = new SqliteCommand(sql, con);
			var res = c.ExecuteScalar();
			if (res == null || res == DBNull.Value) { return -1; }
			int ret = int.Parse (res.ToString ());
			return ret;
		}
		
		public bool LinkTrackToPlaylist(int trackId, int playlistId, int order)
		{
			string sql = string.Format("insert into playlisttracks values({0},{1},{2})", 
			                           playlistId, order, trackId);
			SqliteCommand c = new SqliteCommand(sql, con);
			c.ExecuteNonQuery();
			return true;
		}
		
		#region IDisposable implementation
		public void Dispose ()
		{
			try
			{
				con.Close ();
			} 
			catch{}
		}
		#endregion
	}
}

