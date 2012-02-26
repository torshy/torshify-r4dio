namespace Torshify.Radio.Framework
{
    public interface ISettingsSection : IHeaderInfoProvider<HeaderInfo>
    {
        object UI
        {
            get;
        }

        void Load();

        void Save();
    }
}