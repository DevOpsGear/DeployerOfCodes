
namespace NeonMika
{
    /// <summary>
    /// Static settings for the webserver
    /// </summary>
    static class Settings
    {
        public const int SLEEP_WAIT_FOR_SOCKET_DATA = 15;

        public const int MAX_HEADER_SIZE = 512;

        /// <summary>
        /// Buffersize for response file sending 
        /// </summary>
        public const int FILE_BUFFERSIZE = 512;

        public const string SERVER_VERSION = "1.2";
    }
}
