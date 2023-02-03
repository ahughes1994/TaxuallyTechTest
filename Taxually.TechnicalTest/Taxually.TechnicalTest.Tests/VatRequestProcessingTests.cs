using Moq;
using System.Text;
using System.Xml.Serialization;
using Taxually.TechnicalTest.Clients;
using Taxually.TechnicalTest.Models;
using Taxually.TechnicalTest.Services;

namespace Taxually.TechnicalTest.Tests
{
    public class VatRequestProcessingTests
    {
        public Mock<ITaxuallyHttpClient> mockHttpClient;
        public Mock<ITaxuallyQueueClient> mockQueueClient;

        [SetUp]
        public void Setup()
        {
            mockHttpClient = new Mock<ITaxuallyHttpClient>();
            mockQueueClient = new Mock<ITaxuallyQueueClient>();
        }

        [Test]
        public async Task VatRegisterRequest_CalledWithGbRequest_PostsOnHttpClient()
        {
            // Arrange
            mockHttpClient.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<VatRegistrationRequest>()))
                .Returns(Task.CompletedTask);

            var sut = GetSut();
            var request = new VatRegistrationRequest
            {
                CompanyId = "Test_Id",
                CompanyName = "Test_Company",
                Country = "GB"
            };

            // Act
            await sut.VatRegisterRequest(request);

            // Assert
            mockHttpClient.Verify(x => x.PostAsync("https://api.uktax.gov.uk", request), Times.Once());
        }

        [Test]
        public async Task VatRegisterRequest_CalledWithDeRequest_EnqueuesRequestInXmlFormat()
        {
            // Arrange
            mockQueueClient.Setup(x => x.EnqueueAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var sut = GetSut();
            var request = new VatRegistrationRequest
            {
                CompanyId = "Test_Id",
                CompanyName = "Test_Company",
                Country = "DE"
            };

            using var stringwriter = new StringWriter();
            var serializer = new XmlSerializer(typeof(VatRegistrationRequest));
            serializer.Serialize(stringwriter, request);
            var xml = stringwriter.ToString();

            // Act
            await sut.VatRegisterRequest(request);

            // Assert
            mockQueueClient.Verify(x => x.EnqueueAsync("vat-registration-xml", xml), Times.Once());
        }

        [Test]
        public async Task VatRegisterRequest_CalledWithFrRequest_EnqueuesRequestInCsvFormat()
        {
            // Arrange
            mockQueueClient.Setup(x => x.EnqueueAsync(It.IsAny<string>(), It.IsAny<byte[]>()))
                .Returns(Task.CompletedTask);

            var sut = GetSut();
            var request = new VatRegistrationRequest
            {
                CompanyId = "Test_Id",
                CompanyName = "Test_Company",
                Country = "FR"
            };

            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("CompanyName,CompanyId");
            csvBuilder.AppendLine($"{request.CompanyName},{request.CompanyId}");
            var csv = Encoding.UTF8.GetBytes(csvBuilder.ToString());

            // Act
            await sut.VatRegisterRequest(request);

            // Assert
            mockQueueClient.Verify(x => x.EnqueueAsync("vat-registration-csv", csv), Times.Once());
        }

        private VatRequestProcessingService GetSut()
        {
            return new VatRequestProcessingService(mockHttpClient.Object, 
                mockQueueClient.Object);
        }
    }
}