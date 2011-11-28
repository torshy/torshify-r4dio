using Microsoft.Practices.Prism.ViewModel;

namespace Torshify.Radio.EchoNest
{
    public class TermModel : NotificationObject
    {
        public TermModel()
        {
            Boost = 1.0;
        }

        public string Name
        {
            get; 
            set;
        }

        public double Boost
        {
            get; 
            set;
        }

        public bool Require
        {
            get; 
            set;
        }

        public bool Ban
        {
            get; 
            set;
        }
    }
}