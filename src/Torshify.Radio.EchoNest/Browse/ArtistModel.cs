using System.Collections.Generic;

using EchoNest.Artist;

using Microsoft.Practices.Prism.ViewModel;

using Torshify.Radio.Framework;
using System.Linq;

namespace Torshify.Radio.EchoNest.Browse
{
    public class ArtistModel : NotificationObject
    {
        #region Fields

        private IEnumerable<RadioTrackContainer> _albums;
        private ImageItem _image;
        private IEnumerable<ImageItem> _images;
        private string _name;
        private IEnumerable<NewsItem> _news;
        private BiographyItem _biography;
        private IEnumerable<BiographyItem> _biographies;
        private IEnumerable<BlogItem> _blogs;
        private IEnumerable<VideoItem> _videos;

        #endregion Fields

        #region Properties

        public IEnumerable<RadioTrackContainer> Albums
        {
            get { return _albums; }
            set
            {
                _albums = value;
                RaisePropertyChanged("Albums");
            }
        }

        public ImageItem Image
        {
            get 
            {
                return _image;
            }
            set 
            {
                _image = value;
                RaisePropertyChanged("Image");
            }
        }

        public IEnumerable<ImageItem> Images
        {
            get { return _images; }
            set
            {
                _images = value;
                RaisePropertyChanged("Images", "HasImages");
            }
        }

        public bool HasImages
        {
            get { return _image != null && _images.Any(); }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged("Name");
            }
        }

        public IEnumerable<NewsItem> News
        {
            get { return _news; }
            set
            {
                _news = value;
                RaisePropertyChanged("News", "HasNews");
            }
        }

        public bool HasNews
        {
            get { return _news != null && _news.Any(); }
        }

        public BiographyItem Biography
        {
            get 
            {
                return _biography;
            }
            set
            {
                _biography = value;
                RaisePropertyChanged("Biography");
            }
        }

        public IEnumerable<BiographyItem> Biographies
        {
            get 
            {
                return _biographies;
            }
            set 
            {
                _biographies = value;
                RaisePropertyChanged("Biographies", "HasBiographies");
            }
        }

        public bool HasBiographies
        {
            get
            {
                return _biographies != null && _biographies.Any();
            }
        }

        public IEnumerable<BlogItem> Blogs
        {
            get
            {
                return _blogs;
            }
            set 
            {
                _blogs = value;
                RaisePropertyChanged("Blogs");
            }
        }

        public IEnumerable<VideoItem> Videos
        {
            get 
            {
                return _videos;
            }
            set 
            {
                _videos = value;
                RaisePropertyChanged("Videos");
            }
        }

        #endregion Properties
    }
}