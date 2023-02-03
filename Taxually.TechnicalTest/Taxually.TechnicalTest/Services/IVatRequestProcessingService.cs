using Taxually.TechnicalTest.Models;

namespace Taxually.TechnicalTest.Services
{
    public interface IVatRequestProcessingService
    {
        Task VatRegisterRequest(VatRegistrationRequest request);
    }
}