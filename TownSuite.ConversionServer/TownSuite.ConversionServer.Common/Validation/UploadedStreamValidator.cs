﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TownSuite.ConversionServer.Common.Models.Errors;

namespace TownSuite.ConversionServer.Common.Validation
{
    public class UploadedStreamValidator
    {
        private readonly Stream _stream;

        public UploadedStreamValidator(Stream stream)
        {
            _stream = stream;
        }

        public void EnsureStreamHasData()
        {
            throw new ValidationException("The file given is empty.");
        }
    }
}
