using System;
using System.Xml;
using Mono.Data.Sqlite;

namespace iTunesPlaylists
{
	public class Track
	{

		public string TrackName { get; set; }
		public int iTunesId { get; set; }
		public string Artist { get; set; }
		public string Album { get; set; }
		public string Genre { get; set; }
		public string Comments { get; set; }
		public int Time { get; set; }
		public int Rating { get; set; }
		public string Hash { get; set; }

		public Track()
		{
			this.Hash = "";
			this.iTunesId = -1;
			this.TrackName = "";
			this.Artist = "";
			this.Album = "";
			this.Genre = "";
			this.Time = 0;
			this.Rating = 0;
			this.Comments = "";
		}

		public Track(string hash, string trackName, string artist, string album, string genre, int time, int rating, string comments, int iTunesId)
		{
			this.Hash = hash;
			this.TrackName = trackName;
			this.Artist = artist;
			this.Album = album;
			this.Genre = genre;
			this.Time = time;
			this.Rating = rating;
			this.Comments = comments;
			this.iTunesId = iTunesId;
			GenerateHash();
		}

		public void GenerateHash()
		{
			Hash = "";
			System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
			string toHash = this.TrackName + this.Artist + this.Album;
			byte[] bytesToHash = System.Text.ASCIIEncoding.ASCII.GetBytes(toHash);
			byte[] hashBytes = md5.ComputeHash(bytesToHash);
			foreach(byte b in hashBytes)
			{
				Hash += b.ToString("x");
			}
		}

	}
}

