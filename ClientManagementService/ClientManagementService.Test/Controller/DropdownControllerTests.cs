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
        }

        [Test]
        public async Task GetPetTypes()
        {
            var controller = new DropdownController(_dropdownService.Object);

            var result = await controller.GetPetTypes();

            Assert.NotNull(result);
            Assert.AreEqual(typeof(OkObjectResult), result.GetType());

            var okObj = (OkObjectResult)result;

            Assert.AreEqual(okObj.StatusCode, 200);
        }

        [Test]
        public async Task GetVaccinesByPetType()
        {
            var controller = new DropdownController(_dropdownService.Object);

            var result = await controller.GetVaccineByPetType(1);

            Assert.NotNull(result);
            Assert.AreEqual(typeof(OkObjectResult), result.GetType());

            var okObj = (OkObjectResult)result;

            Assert.AreEqual(okObj.StatusCode, 200);
        }
    }
}
