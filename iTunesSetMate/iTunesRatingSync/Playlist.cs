using System;
using System.Collections.Generic;

namespace iTunesPlaylists
{
	public class Playlist
	{

		public string Name { get; set; }
		public List<Track> Tracks { get; set; }

		public Playlist()
		{
			Tracks = new List<Track>();
		}

	}
}

