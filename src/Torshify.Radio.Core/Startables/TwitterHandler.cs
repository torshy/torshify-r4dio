using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Logging;
using Torshify.Radio.Core.Views.Twitter;
using Torshify.Radio.Framework;

using Twitterizer;
using Twitterizer.Streaming;

namespace Torshify.Radio.Core.Startables
{
    public interface ITwitter
    {
        #region Methods

        void Tweet();

        #endregion Methods
    }

    [Export(typeof(ITwitter))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class TwitterHandler : IStartable, ITwitter
    {
        #region Properties

        [Import]
        public ILoggerFacade Logger
        {
            get;
            set;
        }

        [Import]
        public IToastService ToastService
        {
            get;
            set;
        }

        [Import]
        public IRadio Radio
        {
            get; 
            set;
        }

        [Import]
        public ITileService TileService
        {
            get;
            set;
        }

        protected TwitterStream Stream
        {
            get; private set;
        }

        #endregion Properties

        #region Methods

        public void Start()
        {
            TileService.Add<TwitterView>(new TileData
            {
                Title = "Peeps",
                BackgroundImage = new Uri("pack://siteoforigin:,,,/Resources/Tiles/MB_0005_weather1.png")
            });


            Task.Factory.StartNew(() =>
            {
                OAuthTokens tokens = new OAuthTokens();
                tokens.AccessToken = "478840940-tgD2Fp5NWXpDPGWyrHTxIjroDODe6F9r8JEkabQ";
                tokens.AccessTokenSecret = "Jo4fgjtkYBPTfyuigi3slqOo7lVer7rLXwj6rWs";
                tokens.ConsumerKey = "O6MTEfpHhHfhnBr4PuVmlw";
                tokens.ConsumerSecret = "lDZgfovK9FEtn8MBsTpGPn8WvuTbGal2yBD4kHLgI";

                StreamOptions options = new StreamOptions();
                Stream = new TwitterStream(tokens, "v1", options);
                Stream.StartUserStream(Friends,
                                       Stopped,
                                       Created,
                                       Deleted,
                                       DirectMessageCreated,
                                       DirectMessageDeleted,
                                       Callback);
                Radio.CurrentTrackChanged += RadioOnCurrentTrackChanged;
            })
            .ContinueWith(task =>
            {
                if (task.Exception != null)
                {
                    Logger.Log(task.Exception.ToString(), Category.Exception, Priority.Medium);
                }
            });
        }

        private void RadioOnCurrentTrackChanged(object sender, TrackChangedEventArgs e)
        {
            if (e.CurrentTrack != null)
            {
                var link = e.CurrentTrack.ToLink();
                var linkAsString = link.Uri;
                Console.WriteLine(linkAsString);
            }
        }

        public void Tweet()
        {
        }

        private void DirectMessageDeleted(TwitterStreamDeletedEvent status)
        {
        }

        private void DirectMessageCreated(TwitterDirectMessage status)
        {
        }

        private void Friends(TwitterIdCollection friendids)
        {
        }

        private void Callback(TwitterStreamEvent eventdetails)
        {
        }

        private void Deleted(TwitterStreamDeletedEvent status)
        {
        }

        private void Created(TwitterStatus status)
        {
            Task.Factory.StartNew(state =>
                                  {
                                      var ts = (TwitterStatus) state;

                                      try
                                      {
                                          TrackLink trackLink = TrackLink.FromUri(ts.Text);
                                          Track track = Radio.FromLink(trackLink);
                                          ToastService.Show(track.Name + " by " + track.Artist);
                                      }
                                      catch (Exception e)
                                      {
                                          Console.WriteLine(e);
                                      }
                                  }, status);
        }

        private void Stopped(StopReasons stopreason)
        {
        }

        #endregion Methods
    }
}