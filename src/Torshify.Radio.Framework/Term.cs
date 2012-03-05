namespace Torshify.Radio.Framework
{
    public class Term
    {
        public string Id
        {
            get; 
            set;
        }

        public int Index
        {
            get; 
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public int Count
        {
            get; 
            set;
        }
    }

    public class StyleTerm : Term
    {
        
    }

    public class MoodTerm : Term
    {
        
    }
 }