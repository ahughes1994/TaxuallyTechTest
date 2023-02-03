using System.Text;
using System.Xml.Serialization;
using Taxually.TechnicalTest.Clients;
using Taxually.TechnicalTest.Models;

namespace Taxually.TechnicalTest.Services
{
    public class VatRequestProcessingService : IVatRequestProcessingService
    {
        private readonly ITaxuallyHttpClient taxuallyHttpClient;
        private readonly ITaxuallyQueueClient taxuallyQueueClient;

        public VatRequestProcessingService(ITaxuallyHttpClient taxuallyHttpClient,
            ITaxuallyQueueClient taxuallyQueueClient)
        {
            this.taxuallyHttpClient = taxuallyHttpClient;
            this.taxuallyQueueClient = taxuallyQueueClient;
        }

        public async Task VatRegisterRequest(VatRegistrationRequest request)
        {
            switch (request.Country)
            {
                case "GB":
                    await VatRegisterGb(request);
                    break;
                case "FR":
                    await VatRegisterFr(request);
                    break;
                case "DE":
                    await VatRegisterDe(request);
                    break;
                default:
                    throw new Exception("Country not supported");
            }
        }

        private async Task VatRegisterGb(VatRegistrationRequest request)
        {
            // UK has an API to register for a VAT number
            await taxuallyHttpClient.PostAsync("https://api.uktax.gov.uk", request);
        }

        private async Task VatRegisterDe(VatRegistrationRequest request)
        {
            // Germany requires an XML document to be uploaded to register for a VAT number
            using var stringwriter = new StringWriter();
            var serializer = new XmlSerializer(typeof(VatRegistrationRequest));
            serializer.Serialize(stringwriter, request);
            var xml = stringwriter.ToString();
            // Queue xml doc to be processed
            await taxuallyQueueClient.EnqueueAsync("vat-registration-xml", xml);
        }

        private async Task VatRegisterFr(VatRegistrationRequest request)
        {
            // France requires an excel spreadsheet to be uploaded to register for a VAT number
            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("CompanyName,CompanyId");
            csvBuilder.AppendLine($"{request.CompanyName},{request.CompanyId}");
            var csv = Encoding.UTF8.GetBytes(csvBuilder.ToString());
            // Queue file to be processed
            await taxuallyQueueClient.EnqueueAsync("vat-registration-csv", csv);
        }
    }
}
