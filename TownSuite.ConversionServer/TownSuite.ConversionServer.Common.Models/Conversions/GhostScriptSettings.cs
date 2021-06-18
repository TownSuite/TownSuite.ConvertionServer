using System;
using System.Collections.Generic;
using System.Text;

namespace TownSuite.ConversionServer.Common.Models.Conversions
{
    public class GhostScriptSettings
    {
        public decimal MaxGigabytesUpload { get; set; }
        public double MaxJobDurationMinutes { get; set; }
        public string ExecutableLocation { get; set; }
    }
}
