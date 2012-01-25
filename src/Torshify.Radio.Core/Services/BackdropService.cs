using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using Microsoft.Practices.Prism.Logging;

using Torshify.Radio.Framework;

namespace Torshify.Radio.Core.Services
{
    [Export(typeof(IBackdropService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class BackdropService : IBackdropService
    {
        #region Fields

        private const string ApiKey = "590b54eae4a816b5144c09f15a8f3876";

        private readonly ILoggerFacade _logger;

        private Random _randomGen = new Random();

        #endregion Fields

        #region Constructors

        [ImportingConstructor]
        public BackdropService(ILoggerFacade logger)
        {
            _logger = logger;
            CacheLocation = AppConstants.BackdropCacheFolder;
            Directory.CreateDirectory(CacheLocation);
        }

        #endregion Constructors

        #region Properties

        public string CacheLocation
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        public Task<IEnumerable<string>> Query(string artistName)
        {
            return Task<IEnumerable<string>>.Factory.StartNew(() =>
            {
                string[] fileNames;

                if (TryGet(artistName, out fileNames))
                {
                    return fileNames;
                }

                string downloadFolder = Path.Combine(CacheLocation, StringToHash(artistName));
                Directory.CreateDirectory(downloadFolder);

                try
                {
                    string replacedSpaces = artistName.Replace(" ", "_");
                    Uri siteUri = new Uri("http://htbackdrops.com/api/" + ApiKey + "/searchXML?keywords=" + replacedSpaces + "&limit=7");

                    string result = "";

                    HttpWebRequest request = WebRequest.Create(siteUri) as HttpWebRequest;
                    using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                    {
                        StreamReader reader = new StreamReader(response.GetResponseStream());
                        result = reader.ReadToEnd();
                    }

                    //load xml from web request result and get the image id's
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(result);

                    XmlNodeList nodelist = doc.SelectNodes("/search/images/image/id");

                    if (nodelist != null && nodelist.Count == 0)
                    {
                        return new string[0];
                    }

                    List<string> fetchedImages = new List<string>();

                    foreach (var node in nodelist.Cast<XmlNode>())
                    {
                        string imageId = node.InnerText;
                        string url = "http://htbackdrops.com/api/" + ApiKey + "/download/" + imageId + "/intermediate";

                        try
                        {
                            HttpWebRequest webRequest = WebRequest.Create(url) as HttpWebRequest;
                            using (HttpWebResponse response = webRequest.GetResponse() as HttpWebResponse)
                            {
                                Stream responseStream = response.GetResponseStream();
                                byte[] buffer = new byte[1024];
                                int bytes;
                                bytes = responseStream.Read(buffer, 0, buffer.Length);

                                string filePath = Path.Combine(downloadFolder, imageId + ".jpg");
                                using (FileStream fileStream = File.OpenWrite(filePath))
                                {
                                    while (bytes > 0)
                                    {
                                        fileStream.Write(buffer, 0, bytes);
                                        bytes = responseStream.Read(buffer, 0, buffer.Length);
                                    }
                                }

                                fetchedImages.Add(filePath);
                            }
                        }
                        catch (Exception e)
                        {
                            _logger.Log(e.Message, Category.Exception, Priority.Medium);
                        }
                    }

                    return fetchedImages.ToArray();
                }
                catch (Exception e)
                {
                    _logger.Log(e.Message, Category.Exception, Priority.Medium);
                }

                return new string[0];
            });
        }

        public bool TryGet(string artistName, out string fileName)
        {
            string[] files;

            if (TryGet(artistName, out files))
            {
                int randomIndex = _randomGen.Next(0, files.Length);
                fileName = files[randomIndex];
                return true;
            }

            fileName = null;
            return false;
        }

        public bool TryGet(string artistName, out string[] fileNames)
        {
            string possibleLocation = Path.Combine(CacheLocation, StringToHash(artistName));

            if (Directory.Exists(possibleLocation))
            {
                string[] files = Directory.GetFiles(possibleLocation);
                if (files.Length > 0)
                {
                    fileNames = files;
                    return true;
                }
            }

            fileNames = null;
            return false;
        }

        private static string ByteArrayToString(byte[] arrInput)
        {
            int i;
            StringBuilder sOutput = new StringBuilder(arrInput.Length);
            for (i = 0; i < arrInput.Length; i++)
            {
                sOutput.Append(arrInput[i].ToString("X2"));
            }
            return sOutput.ToString();
        }

        private static string StringToHash(string text)
        {
            var tmpSource = Encoding.ASCII.GetBytes(text);
            var tmpHash = new MD5CryptoServiceProvider().ComputeHash(tmpSource);
            return ByteArrayToString(tmpHash);
        }

        #endregion Methods
    }
}