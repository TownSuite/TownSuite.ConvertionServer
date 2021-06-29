using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TownSuite.ConversionServer.APISite.Models;
using TownSuite.ConversionServer.Interfaces.Common.Errors;
using TownSuite.ConversionServer.Interfaces.Utilities.Logging;

namespace TownSuite.ConversionServer.APISite.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class PdfConverterController : Controller
    {
        Interfaces.Utilities.Converters.IPdfToImageBytesConverter _converter;
        IResponseErrorModelFactory<Common.Models.Errors.ResponseErrorModel> _responseError;
        ISimpleLogger _logger;

        public PdfConverterController(Interfaces.Utilities.Converters.IPdfToImageBytesConverter converter,
            IResponseErrorModelFactory<Common.Models.Errors.ResponseErrorModel> responseError,
            ISimpleLogger logger)
        {
            _converter = converter;
            _responseError = responseError;
            _logger = logger;
        }
        [HttpPost]
        public async Task<ItemResponseModel<IEnumerable<byte[]>>> ToPng([FromBody] ItemRequestModel<byte[]> pdf)
        {
            try
            {
                return new ItemResponseModel<IEnumerable<byte[]>>()
                {
                    Data = await _converter.Convert(pdf?.Data)
                };
            } catch (Exception ex)
            {
                await _logger.LogError(ex);
                return new ItemResponseModel<IEnumerable<byte[]>>()
                {
                    Error = _responseError.Create(ex)
                };
            }
        }

    }
}
