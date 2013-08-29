using System;
using Mono.Data.SqliteClient;
using System.Data;
using System.Xml;
using System.Threading;
using System.IO;

namespace iDJTools
{
	class MainClass
	{
		
		static bool Run = true;
		static bool append = true;
		static string iTunesLib = "/Users/Tris/Music/iTunes/iTunes Music Library.xml";
		static string iDjDatabase = "/Volumes/IDJ IPOD/.library/database";
		
		public static bool ParseArgs(string[] args)
		{
			for (int x = 0; x < args.Length; x ++)
			{
				if (args[x] == "-i") { iTunesLib = args[x+1]; }
				if (args[x] == "-d") { iDjDatabase = args[x+1]; }
				if (args[x] == "-p") { append = false; }
			}
			if (iTunesLib != String.Empty && iDjDatabase != String.Empty) { return true; }
			return false;
		}
		
		public static void Main (string[] args)
		{
			Console.WriteLine("iDJ Track Rater v0.1 - Tristan Phillips");
			if (!ParseArgs(args)) { ShowUsage(); return; }
			Process();
		}
		
		public static void ShowUsage()
		{
			Console.WriteLine("usage: iDJTools.exe -i <iTunes Music Library.xml path>\r\n\t-d <iDJ database file path>\r\n\t[-p prepends rating, default is append]");
		}
		
		public static void Process()
		{
			
			XmlDocument iTunes = new XmlDocument();
			Console.WriteLine("Opening iTunes Library . . .");
			iTunes.Load(iTunesLib);
			Console.WriteLine("Backing up iDJ Library . . .");
			File.Copy(iDjDatabase, iDjDatabase + ".backup", true);
			Console.WriteLine("Opening iDJ Library . . .");
			SqliteConnection con = new SqliteConnection("Uri=" + iDjDatabase);
			con.Open();
			Console.WriteLine("Reading iDJ tracks . . .");
			SqliteDataAdapter a = new SqliteDataAdapter("Select * from tracks;", con);
			DataSet d = new DataSet("tracks");
			a.Fill(d);
			Console.WriteLine("Processing " + d.Tables[0].Rows.Count + " track(s) . . .");
			ThreadStart s = new ThreadStart(ListenForKey);
			Console.WriteLine("Press return to start run. Return during stops processing. Ctrl+c now to abort.");
			Console.ReadLine();
			Thread t = new Thread(s);
			t.Start();
			int rated = 0;
			foreach(DataRow r in d.Tables[0].Rows)
			{
				if (!Run) { Console.WriteLine("Stopped by user"); break; }
				int trackID = int.Parse(r.ItemArray[0].ToString());
				string track = r.ItemArray[3].ToString();
				string trackSearch = r.ItemArray[4].ToString();
				string album = r.ItemArray[5].ToString();
				Console.WriteLine(track + " - " + album);
				XmlNode trackNode = null;
				try{ trackNode = iTunes.SelectSingleNode("/plist/dict/dict/dict/string[text()='" + track + "']").ParentNode; } catch{}
				if (trackNode != null)
				{
					XmlNode ratingNode = trackNode.SelectSingleNode("key[text()='Rating']");
					if (ratingNode != null)
					{
						int rating = int.Parse(ratingNode.NextSibling.InnerText);
						rating = rating /20;
						Console.WriteLine(" ***** Applying Rating " + rating + " ***** ");
						string sql = "";
						if (append)
						{
							sql = "update tracks set title = '" + track + " [" + rating + "/5]' where id = " + trackID + ";";
							sql += "update tracks set title_canonical = '" + trackSearch + "[R" + rating + "]' where id = " + trackID + ";";
						}
						else
						{
							sql = "update tracks set title = '[" + rating + "/5] " + track + "' where id = " + trackID + ";";
							sql += "update tracks set title_canonical = '" + trackSearch + "[R" + rating + "]' where id = " + trackID + ";";
						}
						SqliteCommand c = new SqliteCommand(sql, con);
						c.ExecuteNonQuery();
						rated ++;
					}
				}
			}
			Console.WriteLine("Done! " + rated + " track(s) rated. (press return)");
			con.Close();
		}
		
		static void ListenForKey()
		{
			Console.ReadLine();
			Run = false;
		}
		
	}
}

