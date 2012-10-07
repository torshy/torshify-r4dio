using System;
using System.Reflection;
using System.Threading.Tasks;
using EightTracks;

namespace Torshify.Radio.EightTracks
{
    /// <summary>
    /// Adds an extension method to the <c>EightTracksSession</c> class, that enables setting of the
    /// <c>MaxResponseContentBufferSize</c> property of the session's private <c>_httpClient</c> field.
    /// This is a workaround for a limitation in the current <c>8tracks-Sharp</c> library, that uses the
    /// default buffer size of 64K for all queries in the session.
    /// </summary>
    public static class EightTracksExtensions
    {
        #region Fields

        private static FieldInfo _eightTracksSessionHttpClientField;
        private static PropertyInfo _httpClientMaxResponseContentBufferSizeProperty;

        #endregion Fields

        #region Constructors

        static EightTracksExtensions()
        {
            _eightTracksSessionHttpClientField = null;
            _httpClientMaxResponseContentBufferSizeProperty = null;
            Task.Factory.StartNew(ReflectHiddenFields);
        }

        #endregion Constructors

        #region Methods

        public static void SetHttpClientMaxResponseContentBufferSize(this EightTracksSession session, int bufferSize)
        {
            if ((_eightTracksSessionHttpClientField != null) && (_httpClientMaxResponseContentBufferSizeProperty != null))
            {
                var httpClient = _eightTracksSessionHttpClientField.GetValue(session);
                if (httpClient != null)
                {
                    _httpClientMaxResponseContentBufferSizeProperty.SetValue(httpClient, bufferSize, null);
                }
            }
        }

        private static void ReflectHiddenFields()
        {
            _eightTracksSessionHttpClientField = typeof(EightTracksSession).GetField(
                "_httpClient",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

            Type httpClientType = typeof(EightTracksSession).Assembly.GetType("System.Net.Http.HttpClient", true);
            if (httpClientType != null)
            {
                _httpClientMaxResponseContentBufferSizeProperty = httpClientType.GetProperty(
                    "MaxResponseContentBufferSize",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            }
        }

        #endregion Methods
    }
}
