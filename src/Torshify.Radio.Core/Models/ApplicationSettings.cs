using System.Windows.Media;

namespace Torshify.Radio.Core.Models
{
    public class ApplicationSettings
    {
        #region Constructors

        public ApplicationSettings()
        {
            HotkeysEnabled = true;
            Culture = "en";
        }

        #endregion Constructors

        #region Properties

        public double WindowHeight
        {
            get; set;
        }

        public double WindowWidth
        {
            get; set;
        }

        public double WindowLeft
        {
            get;
            set;
        }

        public double WindowTop
        {
            get;
            set;
        }

        public bool FirstTimeWizardRun
        {
            get;
            set;
        }

        public string Culture
        {
            get;
            set;
        }

        public bool HotkeysEnabled
        {
            get;
            set;
        }

        public Color? AccentColor
        {
            get;
            set;
        }

        #endregion Properties
    }
}