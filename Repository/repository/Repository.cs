using europarser.interfaces;
using EuroRepository.repository;

namespace europarser.repository
{
    public static partial class Repository
    {
        public static DbVerkkItems VerkkItems => DbVerkkItems.Instance;
        public static DbEuroMade EuroMade => DbEuroMade.Instance;
        public static ILogger Logger => DbLogger.GetInstance();
        public static DbParser Parser => DbParser.Instance;
    }
}
