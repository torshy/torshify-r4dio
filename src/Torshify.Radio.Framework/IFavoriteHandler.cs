using System.ComponentModel.Composition;

namespace Torshify.Radio.Framework
{
    [InheritedExport]
    public interface IFavoriteHandler
    {
        #region Methods

        bool CanHandleFavorite(Favorite favorite);

        void Play(Favorite favorite);

        void Queue(Favorite favorite);

        #endregion Methods
    }
}