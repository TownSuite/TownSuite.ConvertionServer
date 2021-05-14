using System;
using System.Collections.Generic;
using System.Text;
using TownSuite.ConversionServer.Common.Models.Errors;
using TownSuite.ConversionServer.Interfaces.Common.Errors;

namespace TownSuite.ConversionServer.Common.Errors
{
    public class ResponseErrorModelFactory: IResponseErrorModelFactory<ResponseErrorModel>
    {
        public ResponseErrorModel Create(string message, string details)
        {
            return new ResponseErrorModel()
            {
                Message = message,
                Details = details
            };
        }

        public ResponseErrorModel Create(Exception ex)
        {
            return Create(ex, ex.Message);
        }

        public ResponseErrorModel Create(Exception ex, string messageOverride)
        {
            return Create(messageOverride, GetResponseDetails(ex));
        }

        private string GetResponseDetails(Exception ex)
        {
            if (ex == null) {
                return "{message: \"Exception is null\"}";
            }
            string message = ex.Message;
            string stacktrace = ex.StackTrace;
            string fullMessage = $"{{class: \"{ex.GetType().Name}\" message:\"{message}\", stacktrace:\"{stacktrace}\"";
            if (ex.InnerException != null) fullMessage += $", innerException: {GetResponseDetails(ex.InnerException)}";
            fullMessage += "}";
            return fullMessage;
        }
    }
}
