using System;

namespace Kandu.Core
{
    public static class Log
    {
        /// <summary>
        /// Log an error into the Kandu Sql Server database
        /// </summary>
        /// <param name="ex">The provided Exception object</param>
        /// <param name="request">The request this error belongs to</param>
        /// <param name="area">The area in which the error happened, such as a class name or user action</param>
        public static void Error(Exception ex, IRequest request = null, string area = "")
        {
            Delegates.Log.Error(request?.User.UserId ?? 0, request?.Path ?? "", area, ex.Message, ex.StackTrace);
        }
    }
}
