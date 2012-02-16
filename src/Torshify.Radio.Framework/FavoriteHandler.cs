using System.ComponentModel.Composition;

namespace Torshify.Radio.Framework
{
    public abstract class FavoriteHandler<T> : IFavoriteHandler
        where T : Favorite
    {
        #region Properties

        [Import]
        public IRadio Radio
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

        #endregion Properties

        #region Methods

        public abstract bool CanHandleFavorite(Favorite favorite);

        public void Play(Favorite favorite)
        {
            Play((T)favorite);
        }

        public void Queue(Favorite favorite)
        {
            Queue((T)favorite);
        }

        protected abstract void Play(T favorite);

        protected abstract void Queue(T favorite);

        #endregion Methods
    }
}