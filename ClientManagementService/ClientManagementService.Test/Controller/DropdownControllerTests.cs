using ClientManagementService.API.Controllers;
using ClientManagementService.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClientManagementService.Test.Controller
{
    [TestFixture]
    public class DropdownControllerTests
    {

        private Mock<IDropdownService> _dropdownService;

        [SetUp]
        public void Setup()
        {
            _dropdownService = new Mock<IDropdownService>();

            _dropdownService.Setup(d => d.GetPetTypes())
                .ReturnsAsync(new List<Domain.Models.PetType>()
                {
                    new Domain.Models.PetType()
                    {
                        Id = 1,
                        PetTypeName = "Dog"
                    }
                });

            _dropdownService.Setup(d => d.GetVaccinesByPetType(It.IsAny<short>()))
                .ReturnsAsync(new List<Domain.Models.Vaccine>()
                {
                    new Domain.Models.Vaccine()
                    {
                        Id = 1,
                        VaxName = "Bordetella"
                    }
                });

            _dropdownService.Setup(d => d.GetBreedsByPetType(It.IsAny<short>()))
                .ReturnsAsync(new List<Domain.Models.Breed>()
                {
                    new Domain.Models.Breed()
                    {
                        Id = 1,
                        BreedName = "Golden Retriever"
                    }
                });

            _dropdownService.Setup(d => d.GetClients())
                .ReturnsAsync(new List<Domain.Models.Client>()
                {
                    new Domain.Models.Client()
                    {
                        Id = 1,
                        FullName = "Test User"
                    }
                });
        }

        [Test]
        public async Task GetClientTest()
        {
            var controller = new DropdownController(_dropdownService.Object);

            var result = await controller.GetClients();

            Assert.NotNull(result);
            Assert.AreEqual(typeof(OkObjectResult), result.GetType());

            var okObj = (OkObjectResult)result;

            Assert.AreEqual(okObj.StatusCode, 200);
        }

        [Test]
        public async Task GetPetTypesTest()
        {
            var controller = new DropdownController(_dropdownService.Object);

            var result = await controller.GetPetTypes();

            Assert.NotNull(result);
            Assert.AreEqual(typeof(OkObjectResult), result.GetType());

            var okObj = (OkObjectResult)result;

            Assert.AreEqual(okObj.StatusCode, 200);
        }

        [Test]
        public async Task GetVaccinesByPetTypeTest()
        {
            var controller = new DropdownController(_dropdownService.Object);

            var result = await controller.GetVaccineByPetType(1);

            Assert.NotNull(result);
            Assert.AreEqual(typeof(OkObjectResult), result.GetType());

            var okObj = (OkObjectResult)result;

            Assert.AreEqual(okObj.StatusCode, 200);
        }

        [Test]
        public async Task GetBreedByPetTypeTest()
        {
            var controller = new DropdownController(_dropdownService.Object);

            var result = await controller.GetBreedsByPetType(1);

            Assert.NotNull(result);
            Assert.AreEqual(typeof(OkObjectResult), result.GetType());

            var okObj = (OkObjectResult)result;

            Assert.AreEqual(okObj.StatusCode, 200);
        }
    }
}
