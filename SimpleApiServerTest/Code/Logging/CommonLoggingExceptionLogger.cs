using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http.ExceptionHandling;

namespace SimpleApiServerTest.Code.Logging
{
    public class CommonLoggingExceptionLogger : ExceptionLogger
    {
        public override void Log(ExceptionLoggerContext context)
        {
            ILog log = LogManager.GetLogger(typeof(CommonLoggingExceptionLogger).Namespace);
            // log = new ProxyWrapper<ILog>(log).GetTransparentProxy();
            //var owinContext = context.Request.GetOwinContext();
            //if (owinContext != null)
            //{
            //    var traceId = TraceIdMiddleware.GetTraceId(owinContext.Environment);
            //    log = new LoggerWrapper(traceId, log);
            //}

            // Format the exception for logging.
            var sb = new StringBuilder();
            sb.Append("[Request: ");
            sb.Append(context.Request.RequestUri);
            sb.Append("]");

            // Log to Common.Logging
            log.Error(sb.ToString(), context.Exception);
        }
    }
}