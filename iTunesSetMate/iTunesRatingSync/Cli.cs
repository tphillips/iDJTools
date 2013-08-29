using System;
using System.Linq;
using System.Collections.Generic;

namespace iTunesPlaylists
{
	public class Cli
	{

		static iTunesLib iTunes;
		static bool writePages = true;
		static string iTunesPath = "";
		static bool writeiDjPlaylists = false;
		static string iDJPath = "";

		public static bool ParseArgs(string[] args)
		{
			for (int x = 0; x < args.Length; x++)
			{
				if (args[x] == "-?") { ShowUsage(); return false;}
				if (args[x] == "--help") { ShowUsage(); return false;}
				if (args[x] == "-i") { iTunesPath = args[x + 1]; }
				if (args[x] == "--itunes-xml-path") { iTunesPath = args[x + 1]; }
				if (args[x] == "-N") { writePages = false; }
				if (args[x] == "--no-pages") { writePages = false; }
				if (args[x] == "-d") { iDJPath = args[x + 1]; writeiDjPlaylists = true;}
				if (args[x] == "--idj-database-path") { iDJPath = args[x + 1]; writeiDjPlaylists = true;}
			}
			if (!writePages && !writeiDjPlaylists) 
			{
				Console.WriteLine("Nothing to do!"); ShowUsage(); return false;
			}
			if (iTunesPath == "") 
			{ 
				Console.WriteLine("Looking for an iTunes Library . . .");
				iTunesPath = iTunesLib.FindLibrary(); 
			} 
			if (iTunesPath == "")
			{
				Console.WriteLine("iTunes path not found or set!"); ShowUsage(); return false;
			}
			return true;
		}

		static void ShowUsage ()
		{
			Console.WriteLine("iTunesSetMate.exe\r\n\t" +
			                  "-i --itunes-xml-path <iTunes xml path>\r\n\t" +
			                  "-N --no-pages Don't write html pages\r\n\t" +
			                  "-d --idj-database-path <iDJ database path> Sync iTunes playlists to iDJ\r\n\r\n" +
			    			  "Don't forget to eject the iDJ device properly!\r\n" +
			                  "Music tracks in an iTunes playlist will only be added to the\r\n" +
			                  "iDJ playlist if they can be found on the iDJ device.\r\n" +
			                  "Certain iTunes playlists will be ignored.\r\n\r\n" +
			                  "So this software can be run with no arguments - If no iTunes xml path\r\n" +
			                  "is provided, the software will attempt to find it.\r\n");
		}

		public static void Main(string[] args)
		{
			Console.WriteLine("\r\niTunesSetMate v1.1 - Tristan Phillips\r\n");
			if (!ParseArgs(args)) { return; }
			iDJDatabase idj;
			Console.WriteLine("Opening iTunes Library " + iTunesPath + " . . .");
			iTunes = new iTunesLib(iTunesPath);
			if (writeiDjPlaylists)
			{
				Console.WriteLine("Opening iDJ database . . .");
				idj = new iDJDatabase(iDJPath);
				idj.DeleteiDJPlaylists ();
			}
			Console.WriteLine("Listing iTunes Tracks . . .");
			iTunes.GetTracks();
			Console.WriteLine("Listing iTunes Playlists . . .");
			iTunes.GetPlayLists();
			List<string> files = new List<string>();
			foreach(Playlist p in iTunes.Playlists)
			{
				var tracks = (from t in p.Tracks orderby t.Rating descending select t).ToList();
				if (tracks.Count() > 0)
				{
					if (writeiDjPlaylists) 
					{ 
						Console.WriteLine("Creating iDJ playlist " + p.Name + " . . .");
						if (!idj.CreateiDJPlaylist (p, false))
						{
							Console.WriteLine("iDJ playlist " + p.Name + " was NOT created");
						}
					}
					if (writePages)
					{
						files.Add (p.Name);
						Console.WriteLine("Writing " + p.Name + " . . .");
						LibPage.GeneratePage(tracks, p.Name + ".htm", p.Name);
					}
				}
			}
			if (writeiDjPlaylists) { idj.Dispose (); }
			if (writePages) { LibPage.WriteIndex(files); }
		}

	}
}

