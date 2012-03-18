using Torshify.Radio.Framework;
using Term = EchoNest.Term;

namespace Torshify.Radio.EchoNest.Views.Style
{
    public class StyleTrackStreamData : TrackStreamData
    {        
        /// <summary>
        ///     Name(s) of seed artist(s) for the playlist
        ///     Multiples allowed (no more than 5 total total artist_id, artist, track_id, and song_id parameters)
        /// </summary>
        /// <example>
        ///     Weezer, the+beatles
        /// </example>
        public Term[] Artist
        {
            get;
            set;
        }
        /// <summary>
        ///     the type of the playlist to be generated.
        /// </summary>
        /// <example>
        ///     <list type = "bullet">
        ///         <item>artist - plays songs for the given artists</item>
        ///         <item>artist-radio - plays songs for the given artists and similar artists</item>
        ///         <item>artist-description - plays songs from artists matching the given description</item>
        ///         <item>song-radio - plays songs similar to the song specified.</item>
        ///         <item>catalog - the playlist is personalized based upon the given seed catalog. Results are limited to items listed in the given catalog.</item>
        ///         <item>catalog-radio - the playlist is personalized based upon the given seed catalog. Results are limited to items listed in the given catalog and items that are similar to items in the given catalog.</item>
        ///     </list>
        /// </example>
        public string Type
        {
            get;
            set;
        }

        /// <summary>
        ///     The artist_pick parameter is used to determine how songs are picked for each artist in artist-type playlists. If the asc or desc suffix is ommitted, artist_pick defaults to descending.
        /// </summary>
        /// <example>
        ///     song_hotttness-desc tempo, duration, loudness, mode, key
        /// </example>
        /// <remarks>
        ///     (default = song_hotttness-desc)
        /// </remarks>
        public string ArtistPick
        {
            get;
            set;
        }

        /// <summary>
        ///     the maximum variety of artists to be represented in the playlist. A higher number will allow for more variety in the artists.
        /// </summary>
        /// <example>
        ///     0 - 1
        /// </example>
        /// <remarks>
        ///     (default = 0.3)
        /// </remarks>
        public double? Variety
        {
            get;
            set;
        }

        /// <summary>
        ///     Controls the distribution of artists in the playlist. A focused distribution yields a playlist of songs that are tightly clustered around the seeds, whereas a wandering distribution yields a playlist from a broader range of artists.
        /// </summary>
        /// <example>
        ///     focused, wandering
        /// </example>
        /// <remarks>
        ///     (default = focused)
        /// </remarks>
        public string Distribution
        {
            get;
            set;
        }

        /// <summary>
        ///     Controls the trade between known music and unknown music. A value of 0 means no adventurousness, only known and preferred music will be played. A value of 1 means high adventurousness, mostly unknown music will be played. A value of auto indicates that the adventurousness should be automatically determined based upon the taste profile of the user. This parameter only applies to catalog and catalog-radio type playlists.
        /// </summary>
        /// <example>
        ///     0 - 1
        /// </example>
        /// <remarks>
        ///     (default = 0.2)
        /// </remarks>
        public double? Adventurousness
        {
            get;
            set;
        }

        /// <summary>
        ///     ID of seed artist catalog for the playlist
        /// </summary>
        /// <example>
        ///     CAKSMUX1321A708AA4
        /// </example>
        public string SeedCatalog
        {
            get;
            set;
        }

        /// <summary>
        ///     description of the type of songs that can be included in the playlist
        /// </summary>
        /// <example>
        ///     alt-rock,-emo,harp^2
        /// </example>
        public Term[] DescriptionTerms
        {
            get;
            set;
        }

        /// <summary>
        ///     a musical style or genre like rock, jazz, or funky. See the method list_terms for details on what styles are currently available
        /// </summary>
        /// <example>
        ///     jazz, metal^2
        /// </example>
        public Term[] Styles
        {
            get;
            set;
        }

        /// <summary>
        ///     a mood like happy or sad. See the method list_terms for details on what moods are currently available
        /// </summary>
        /// <example>
        ///     happy, sad^.5
        /// </example>
        public Term[] Moods
        {
            get;
            set;
        }

        /// <summary>
        ///     the minimum artist hotttnesss for songs in the playlist
        /// </summary>
        /// <example>
        ///     0.0 &lt; hotttnesss &lt; 1.0
        /// </example>
        /// <remarks>
        ///     (default=0.0)
        /// </remarks>
        public double? ArtistMinHotttnesss
        {
            get;
            set;
        }

        /// <summary>
        ///     the maximum artist hotttness for songs in the playlist
        /// </summary>
        /// <example>
        ///     0.0 &lt; hotttnesss &lt; 1.0
        /// </example>
        /// <remarks>
        ///     (default=1.0)
        /// </remarks>
        public double? ArtistMaxHotttnesss
        {
            get;
            set;
        }

        /// <summary>
        ///     the minimum artist familiarity for songs in the playlist
        /// </summary>
        /// <example>
        ///     0.0 &lt; familiarity  &lt; 1.0
        /// </example>
        /// <remarks>
        ///     (default=0.0)
        /// </remarks>
        public double? ArtistMinFamiliarity
        {
            get;
            set;
        }

        /// <summary>
        ///     the maximum artist familiarity for songs in the playlist
        /// </summary>
        /// <example>
        ///     0.0 &lt; familiarity  &lt; 1.0
        /// </example>
        /// <remarks>
        ///     (default=1.0)
        /// </remarks>
        public double? ArtistMaxFamiliarity
        {
            get;
            set;
        }

        /// <summary>
        ///     Matches artists that have an earliest start year after the given value
        /// </summary>
        /// <example>
        ///     1970, 2011, present
        /// </example>
        public string ArtistStartYearAfter
        {
            get;
            set;
        }

        /// <summary>
        ///     Matches artists that have an earliest start year before the given value
        /// </summary>
        /// <example>
        ///     1970, 2011, present
        /// </example>
        public string ArtistStartYearBefore
        {
            get;
            set;
        }

        /// <summary>
        ///     Matches artists that have a latest end year after the given value
        /// </summary>
        /// <example>
        ///     1970, 2011, present
        /// </example>
        public string ArtistEndYearAfter
        {
            get;
            set;
        }

        /// <summary>
        ///     Matches artists that have a latest end year before the given value
        /// </summary>
        /// <example>
        ///     1970, 2011, present
        /// </example>
        public string ArtistEndYearBefore
        {
            get;
            set;
        }

        /// <summary>
        ///     the key of songs in the playlist
        /// </summary>
        /// <example>
        ///     (c, c-sharp, d, e-flat, e, f, f-sharp, g, a-flat, a, b-flat, b) 0 - 11
        /// </example>
        public string Key
        {
            get;
            set;
        }

        /// <summary>
        ///     the minimum danceability of any song
        /// </summary>
        /// <example>
        ///     0.0 &lt; danceability &lt; 1.0
        /// </example>
        /// <remarks>
        ///     (default=0.0)
        /// </remarks>
        public double? MinDanceability
        {
            get;
            set;
        }

        /// <summary>
        ///     the maximum danceability of any song
        /// </summary>
        /// <example>
        ///     0.0 &lt; danceability &lt; 1.0
        /// </example>
        /// <remarks>
        ///     (default=1.0)
        /// </remarks>
        public double? MaxDanceability
        {
            get;
            set;
        }

        /// <summary>
        ///     the minimum energy of any song
        /// </summary>
        /// <example>
        ///     0.0 &lt; energy &lt; 1.0
        /// </example>
        /// <remarks>
        ///     (default=0.0)
        /// </remarks>
        public double? MinEnergy
        {
            get;
            set;
        }

        /// <summary>
        ///     the maximum energy of any song
        /// </summary>
        /// <example>
        ///     0.0 &lt; energy &lt; 1.0
        /// </example>
        /// <remarks>
        ///     (default=1.0)
        /// </remarks>
        public double? MaxEnergy
        {
            get;
            set;
        }

        /// <summary>
        ///     the minimum latitude for the location of artists in the playlist
        /// </summary>
        /// <example>
        ///     -90.0 &lt; latitude &lt; 90.0
        /// </example>
        /// <remarks>
        ///     (default=-90.0)
        /// </remarks>
        public double? MinLatitude
        {
            get;
            set;
        }

        /// <summary>
        ///     the maximum latitude for the location of artists in the playlist
        /// </summary>
        /// <example>
        ///     -90.0 &lt; latitude &lt; 90.0
        /// </example>
        /// <remarks>
        ///     (default=90.0)
        /// </remarks>
        public double? MaxLatitude
        {
            get;
            set;
        }

        /// <summary>
        ///     the minimum longitude for the location of artists in the playlist
        /// </summary>
        /// <example>
        ///     -180.0 &lt; longitude &lt; 180.0
        /// </example>
        /// <remarks>
        ///     (default=-180.0)
        /// </remarks>
        public double? MinLongitude
        {
            get;
            set;
        }

        /// <summary>
        ///     the maximum longitude for the location of artists in the playlist
        /// </summary>
        /// <example>
        ///     -180.0 &lt; longitude &lt; 180.0
        /// </example>
        /// <remarks>
        ///     (default=180.0)
        /// </remarks>
        public double? MaxLongitude
        {
            get;
            set;
        }

        /// <summary>
        ///     the minimum loudness of any song on the playlist
        /// </summary>
        /// <example>
        ///     -100.0 &lt; loudness &lt; 100.0 (dB)
        /// </example>
        /// <remarks>
        ///     (default=-100.0)
        /// </remarks>
        public double? MinLoudness
        {
            get;
            set;
        }

        /// <summary>
        ///     the maximum loudness of any song on the playlist
        /// </summary>
        /// <example>
        ///     -100.0 &lt; loudness &lt; 100.0 (dB)
        /// </example>
        /// <remarks>
        ///     (default=100.0)
        /// </remarks>
        public double? MaxLoudness
        {
            get;
            set;
        }

        /// <summary>
        ///     the minimum tempo for any included songs
        /// </summary>
        /// <example>
        ///     0.0 &lt; tempo &lt; 500.0 (BPM)
        /// </example>
        /// <remarks>
        ///     (default=0.0)
        /// </remarks>
        public double? MinTempo
        {
            get;
            set;
        }

        /// <summary>
        ///     the maximum tempo for any included songs
        /// </summary>
        /// <example>
        ///     0.0 &lt; tempo &lt; 500.0 (BPM)
        /// </example>
        /// <remarks>
        ///     (default=500.0)
        /// </remarks>
        public double? MaxTempo
        {
            get;
            set;
        }

        /// <summary>
        ///     the mode of songs in the playlist
        /// </summary>
        /// <example>
        ///     (minor, major) 0, 1
        /// </example>
        public string Mode
        {
            get;
            set;
        }

        /// <summary>
        ///     the minimum hotttnesss for songs in the playlist
        /// </summary>
        /// <example>
        ///     0.0 &lt; hotttnesss &lt; 1.0
        /// </example>
        /// <remarks>
        ///     (default=0.0)
        /// </remarks>
        public double? SongMinHotttnesss
        {
            get;
            set;
        }

        /// <summary>
        ///     the maximum hotttnesss for songs in the playlist
        /// </summary>
        /// <example>
        ///     0.0 &lt; hotttnesss &lt; 1.0
        /// </example>
        /// <remarks>
        ///     (default=1.0)
        /// </remarks>
        public double? SongMaxHotttnesss
        {
            get;
            set;
        }

        /// <summary>
        ///     indicates how the songs should be ordered in the playlist
        /// </summary>
        /// <example>
        ///     tempo-asc, duration-asc, loudness-asc, artist_familiarity-asc, artist_hotttnesss-asc, artist_start_year-asc, artist_start_year-desc, artist_end_year-asc, artist_end_year-desc, song_hotttness-asc, latitude-asc, longitude-asc, mode-asc, key-asc, tempo-desc, duration-desc, loudness-desc, artist_familiarity-desc, artist_hotttnesss-desc, song_hotttnesss-desc, latitude-desc, longitude-desc, mode-desc, key-desc, energy-asc, energy-desc, danceability-asc, danceability-desc
        /// </example>
        public string Sort
        {
            get;
            set;
        }
    }
}