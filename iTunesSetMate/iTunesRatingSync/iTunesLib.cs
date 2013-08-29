using System;
using System.Xml;
using System.Collections.Generic;
using System.Linq;

namespace iTunesPlaylists
{
	public class iTunesLib
	{

		private string LibPath;
		private XmlDocument Doc = new XmlDocument();
		public List<Track> Tracks {get; set;}
		public List<Playlist> Playlists {get; set;}
		public Dictionary<int, Track> tracksHash = new Dictionary<int, Track>();

		public iTunesLib(string libPath)
		{
			this.LibPath = libPath;
			Open();
		}

		public static string FindLibrary()
		{
			string start = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyMusic);
			string attempt = start + System.IO.Path.DirectorySeparatorChar + "iTunes" + System.IO.Path.DirectorySeparatorChar + "iTunes Library.xml";
			if (System.IO.File.Exists(attempt)) { return attempt; }
			attempt = start + System.IO.Path.DirectorySeparatorChar + "iTunes" + System.IO.Path.DirectorySeparatorChar + "iTunes Music Library.xml";
			if (System.IO.File.Exists(attempt)) { return attempt; }
			attempt = start + System.IO.Path.DirectorySeparatorChar + "iTunes" + System.IO.Path.DirectorySeparatorChar + "iTunes Music.xml";
			if (System.IO.File.Exists(attempt)) { return attempt; }
			attempt = start + System.IO.Path.DirectorySeparatorChar + "iTunes" + System.IO.Path.DirectorySeparatorChar + "iTunes.xml";
			if (System.IO.File.Exists(attempt)) { return attempt; }
			return "";
		}

		public void Open()
		{
			Doc.Load(LibPath);
		}

		public void GetTracks()
		{
			Tracks = new List<Track>();
			XmlNodeList TrackNodes = Doc.SelectNodes("/plist/dict/dict/dict");
			foreach(XmlNode t in TrackNodes)
			{
				Track tr = new Track();
				XmlNodeList keys = t.SelectNodes("key");
				foreach(XmlNode k in keys)
				{
					if (k.InnerText == "Name") { tr.TrackName = k.NextSibling.InnerText.Trim(); }
					if (k.InnerText == "Artist") { tr.Artist = k.NextSibling.InnerText.Trim(); }
					if (k.InnerText == "Album") { tr.Album = k.NextSibling.InnerText.Trim(); }
					if (k.InnerText == "Genre") { tr.Genre = k.NextSibling.InnerText.Trim(); }
					if (k.InnerText == "Total Time") { tr.Time = int.Parse(k.NextSibling.InnerText.Trim()); }
					if (k.InnerText == "Rating") { tr.Rating = int.Parse(k.NextSibling.InnerText.Trim()); }
					if (k.InnerText == "Comments") { tr.Comments = k.NextSibling.InnerText.Trim(); }
					if (k.InnerText == "Track ID") { tr.iTunesId = int.Parse(k.NextSibling.InnerText.Trim()); }
				}
				tr.GenerateHash();
				Tracks.Add(tr);
				tracksHash.Add (tr.iTunesId, tr);
			}
		}

		public void GetPlayLists()
		{
			Playlists = new List<Playlist>();
			XmlNodeList PLNodes = Doc.SelectNodes("/plist/dict/array/dict");
			foreach(XmlNode t in PLNodes)
			{
				Playlist p = new Playlist();
				XmlNodeList keys = t.SelectNodes("key");
				foreach(XmlNode k in keys)
				{
					if (k.InnerText == "Name") { p.Name = k.NextSibling.InnerText.Trim(); }
				}
				XmlNodeList tracks = t.SelectNodes("array/dict");
				foreach(XmlNode tr in tracks)
				{
					string innerText = tr.SelectSingleNode ("integer").InnerText;
					Track track = tracksHash[int.Parse(innerText)];
					p.Tracks.Add(track);
				}
				Playlists.Add(p);
			}
		}

		private Track GetTrackWithId(int iTunesId)
		{
			return (from t in Tracks where t.iTunesId == iTunesId select t).First();
		}

	}
}

