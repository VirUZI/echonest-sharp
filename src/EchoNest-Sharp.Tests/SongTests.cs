using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using EchoNest.Artist;
using EchoNest.Song;
using NUnit.Framework;
using Search = EchoNest.Song.Search;
using SearchArgument = EchoNest.Song.SearchArgument;
using SearchResponse = EchoNest.Song.SearchResponse;

namespace EchoNest.Tests
{
    [TestFixture]
    public class SongTests
    {
        [Test]
        public void GetSongs_WhereDescription_Christmas_IsNotNull()
        {
            //arrange
            const string description = "christmas";

            //act
            using (EchoNestSession session = new EchoNestSession(ConfigurationManager.AppSettings.Get("echoNestApiKey")))
            {
                // arrange
                SearchArgument searchArgument = new SearchArgument {Results = 10, Bucket = SongBucket.ArtistHotttness, Sort = "artist_familiarity-desc"};

                searchArgument.Description.AddRange(new TermList {description}); 

                //act
                SearchResponse searchResponse = session.Query<Search>().Execute(searchArgument);

                //assert
                Assert.IsNotNull(searchResponse);

                // output
                foreach (SongBucketItem song in searchResponse.Songs)
                {
                    Console.WriteLine("\t{0} ({1})", song.Title, song.ArtistName);
                }
                Console.WriteLine();
            }
        }


        [TestCase("Apocalypse Now Phyc Rock", "60s,guitar,psychadelic,rock,sountrack^0.5", "eeirie^0.5,dark^0.5,disturbing^0.5,groovy^0.5,melancholia^0.5,ominous^0.5", "Hendrix")]
        [TestCase("Apocalypse Now", "60s,psychadelic,rock^0.5,sountrack^0.5", "eeirie,dark,disturbing,groovy,melancholia,ominous^0.5", "Floyd")]
        public void GetSongs_ForApocalypseNow_ExpectedArtist(string title, string styles, string moods, string expect)
        {
            // arrange
            TermList styleTerms = new TermList();
            foreach (string s in styles.Split(','))
            {
                styleTerms.Add(s);
            }

            TermList moodTerms = new TermList();
            foreach (string s in moods.Split(','))
            {
                moodTerms.Add(s);
            }

            SearchArgument searchArgument = new SearchArgument
            {
                Mode = "0", /* minor */
                Sort = "artist_familiarity-desc",
                Results = 10
            };

            searchArgument.Styles.AddRange(styleTerms);

            searchArgument.Moods.AddRange(moodTerms);

            //act
            using (EchoNestSession session = new EchoNestSession(ConfigurationManager.AppSettings.Get("echoNestApiKey")))
            {
                SearchResponse searchResponse = session.Query<Search>().Execute(searchArgument);

                //assert
                Assert.IsNotNull(searchResponse);
                Assert.IsNotNull(searchResponse.Songs);

                var matches = (from s in searchResponse.Songs
                               where s.ArtistName.ToLower().Contains(expect)
                               select s).ToList();
                

                Assert.IsNotNull(matches, "Failed to find songs where artist name contains: {0}", expect);

                // output
                Console.WriteLine("Tracks for '{0}'", title);
                foreach (SongBucketItem song in searchResponse.Songs)
                {
                    Console.WriteLine("\t{0} ({1})", song.Title, song.ArtistName);
                }
                Console.WriteLine();
                Console.WriteLine();
            }
        }

        [Test]
        public void GetSongs_Profile_ExpectedArtist()
        {
            //act
            using (EchoNestSession session = new EchoNestSession(ConfigurationManager.AppSettings.Get("echoNestApiKey")))
            {
                const string songId = "spotify-WW:track:0hAN0b6tSBuHMIvBGGdXNP";
                const string songId2 = "spotify-WW:track:6zrGHFJzIaGv2O02dDay1k";
                const string expectedEchoNestId = "SOAEDJD13F6397EE1A";
                EchoNest.Song.ProfileResponse profileResponse = session.Query<EchoNest.Song.Profile>().Execute(new[] { new IdSpace(songId), new IdSpace(songId2) }, SongBucket.AudioSummary);

                //assert
                Assert.IsNotNull(profileResponse);
                Assert.IsNotNull(profileResponse.Songs);
                Assert.IsTrue(profileResponse.Songs.Count == 2);

                var songExists = profileResponse.Songs.Any(song => song.ID == expectedEchoNestId);
                Assert.IsTrue(songExists, "Song does not exist in profile response");

                // output
                Console.WriteLine("Tracks for profile '{0}'", songId);
                foreach (SongBucketItem song in profileResponse.Songs)
                {
                    Console.WriteLine("\t{0} ({1})", song.Title, song.ArtistName);
                }
                Console.WriteLine();
                Console.WriteLine();
            }
        }
    }
}