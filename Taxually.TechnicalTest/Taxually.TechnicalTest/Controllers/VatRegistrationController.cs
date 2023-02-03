using Microsoft.AspNetCore.Mvc;
using Taxually.TechnicalTest.Models;
using Taxually.TechnicalTest.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Taxually.TechnicalTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VatRegistrationController : ControllerBase
    {
        private readonly IVatRequestProcessingService vatRequestProcessingService;

        public VatRegistrationController(IVatRequestProcessingService vatRequestProcessingService)
        {
            this.vatRequestProcessingService = vatRequestProcessingService;
        }

        /// <summary>
        /// Registers a company for a VAT number in a given country
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] VatRegistrationRequest request)
        {
            try
            {
                await vatRequestProcessingService.VatRegisterRequest(request);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }

            return Ok();
        }
    }
}
