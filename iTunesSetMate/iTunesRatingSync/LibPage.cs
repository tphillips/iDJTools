using System;
using System.Collections.Generic;
using System.Text;

namespace iTunesPlaylists
{
	public class LibPage
	{

		private static string header = "<!DOCTYPE html> " +
			"<html>  " +
				"<head>\r\n" +
				"<style>\r\n" +
				".stars { \r\n" +
				"background-image:url(data:image/png;base64,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%3D%3D);\r\n" +
				"width: 100px; \r\n" +
				"height: 18px; \r\n" +
				"}\r\n" +
				".star20 { background-position: 0px -45px; }\r\n" +
				".star40 { background-position: 0px -85px; }\r\n" +
				".star60 { background-position: 0px -130px; }\r\n" +
				".star80 { background-position: 0px -175px; }\r\n" +
				".star100 { background-position: 0px -218px; }\r\n" +
				".Switch { float: right; position: absolute; top: 30px; right: 30px; o--pacity:0.4; }" +
				".played { text-decoration:line-through; opacity:0.7; " +
				"</style>\r\n" +
				"<title>_TITLE_</title>\r\n" +
				"<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">\r\n" +
				"<link rel=\"stylesheet\" href=\"http://code.jquery.com/mobile/1.2.0-rc.1/jquery.mobile-1.2.0-rc.1.min.css\" />\r\n" +
				"<script src=\"http://code.jquery.com/jquery-1.8.1.min.js\"></script>\r\n" +
				"<script src=\"http://code.jquery.com/mobile/1.2.0-rc.1/jquery.mobile-1.2.0-rc.1.min.js\"></script>\r\n" +
				"</head>\r\n" +
				"<body>\r\n" +
				"<div data-role=\"page\">\r\n" +
//				"<div data-role=\"header\">\r\n" +
//				"<h1>_TITLE_</h1>\r\n" +
//				"</div></header>\r\n" +
				"<div data-role=\"content\">\r\n" +
				"<ul data-role=\"listview\" data-inset=\"true\" data-filter=\"true\" data-filter-placeholder=\"Search _COUNT_ track(s)\">\r\n";
		
		private static string footer = "</ul></div></div><body><html>";

		private static StringBuilder Page = new StringBuilder();

		public LibPage()
		{
		}

		public static void WriteIndex (List<string> files)
		{
			Page = new StringBuilder();
			Page.Append(header.Replace("_TITLE_", "Playlists"));
			foreach(string file in files)
			{
				Page.Append("<li><a href='" + file + ".htm' target='_blank'>" + file + "</a></li>");
			}
			Page.Append(footer);
			using (System.IO.StreamWriter w = new System.IO.StreamWriter("iTunesPlaylists.htm", false))
			{
				w.Write(Page.ToString());
				w.Flush();
				w.Close();
			}
		}

		public static void GeneratePage(IList<Track> tracks, string fileName, string title)
		{
			Page = new StringBuilder();
			Page.Append(header.Replace("_TITLE_", title).Replace("_COUNT_", tracks.Count.ToString()));
			int rating = 0;
			foreach(Track t in tracks)
			{
				if (rating != t.Rating) 
				{ 
					rating = t.Rating; 
					int star = (int)(0.05f * rating);
					AddListDivider(star);
				}
				AddListItem(t);
			}
			Page.Append(footer);
			using (System.IO.StreamWriter w = new System.IO.StreamWriter(fileName, false))
			{
				w.Write(Page);
				w.Flush();
				w.Close();
			}
		}

		public static void AddListDivider(int star)
		{
			if (star > 0)
			{
				AddListDivider(star + " Star");
			}
			else
			{
				AddListDivider("Unrated");
			}
		}

		public static void AddListDivider(string header)
		{
			Page.Append("<li data-role=\"list-divider\" role=\"heading\" >" + header + "</li>");
		}
		
		public static void AddListItem(Track track)
		{
			string comments = "";
			if (!String.IsNullOrEmpty(track.Comments)) { comments = "<br/><i>" + track.Comments + "</i>"; }
			if (string.IsNullOrEmpty(track.Album.Trim())) { track.Album = "Unknown Album"; }
			if (string.IsNullOrEmpty(track.Genre.Trim())) { track.Genre = "Unknown Genre"; }
			Page.Append("<li>" +
			            "<div onclick=\"$(this).addClass('played');\">" + 
							"<h3>" + track.TrackName + " : " + track.Artist + "</h3>" + 
							"<p>" + track.Album + " (" + track.Genre + ")" +
							comments + "<br/>" +
							"<div class=\"stars star" + track.Rating + "\"></div></p>" +
						"</div>" +
					"</li>\r\n");
		}

	}
}

