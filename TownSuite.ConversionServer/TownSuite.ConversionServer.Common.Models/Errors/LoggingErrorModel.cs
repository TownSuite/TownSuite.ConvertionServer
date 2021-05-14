using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TownSuite.ConversionServer.Common.Models.Errors
{
    public class LoggingErrorModel
    {
        /// <summary>
        /// Represents the unique Id of the error.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The error level of the program.
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// The error message.
        /// </summary>
        /// <remarks>
        /// The Message property of an Exception can be used for this entry.
        /// </remarks>
        public string Message { get; set; }

        public string MessageTemplate { get; set; }

        /// <summary>
        /// The time of the Error
        /// </summary>
        /// <remarks>
        /// <see cref="DateTime.UtcNow"/> will usually be sufficent.
        /// </remarks>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// An exception causing the error.
        /// </summary>
        /// <remarks>
        /// Calling <see cref="Exception.ToString"/> can be used for this
        /// entry.
        /// </remarks>
        public string Exception { get; set; }

        public string Properties { get; set; }

        public string LicenseKey { get; set; }

        public string AptType { get; set; }

        public string MachineName { get; set; }
    }
}
