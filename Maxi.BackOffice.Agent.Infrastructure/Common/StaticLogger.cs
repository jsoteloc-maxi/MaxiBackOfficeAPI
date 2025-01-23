using NLog;

namespace Maxi.BackOffice.Agent.Infrastructure.Common
{
    public static class GLogger
    {
        private static readonly Logger logger = LogManager.GetLogger("*");

        public static void Fatal(Exception ex)
        {
            //logger.Fatal(ex);
            ELogger.Fatal(ex);
        }

        public static void Fatal(Exception ex, string msg, params object[] args)
        {
            //logger.Fatal(ex, msg, args);
            ELogger.Fatal(ex, msg, args);
        }

        public static void Error(Exception ex)
        {
            //logger.Error(ex);
            ELogger.Error(ex);
        }

        public static void Error(Exception ex, string msg, params object[] args)
        {
            //logger.Error(ex, msg, args);
            ELogger.Error(ex, msg, args);
        }

        public static void Info(string msg, params object[] args)
        {
            logger.Info(msg, args);
        }

        public static void Debug(string msg, params object[] args)
        {
            logger.Debug(msg, args);
        }
    }


    public static class ELogger
    {
        private static readonly Logger logger = NLog.LogManager.GetLogger("Errors");

        public static void Fatal(Exception ex)
        {
            logger.Fatal(ex);
        }

        public static void Fatal(Exception ex, string msg, params object[] args)
        {
            logger.Fatal(ex, msg, args);
        }

        public static void Error(Exception ex)
        {
            logger.Error(ex);
        }

        public static void Error(Exception ex, string msg, params object[] args)
        {
            logger.Error(ex, msg, args);
        }
    }


}
