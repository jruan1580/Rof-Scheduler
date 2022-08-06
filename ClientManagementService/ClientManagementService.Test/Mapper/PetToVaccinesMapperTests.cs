using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VaccineEntity = ClientManagementService.Infrastructure.Persistence.Entities.Vaccine;
using VaccineCore = ClientManagementService.Domain.Models.Vaccine;
using ClientManagementService.Domain.Mappers.Database;
using ClientManagementService.Infrastructure.Persistence.Entities;
using ClientManagementService.Domain.Models;

namespace ClientManagementService.Test.Mapper
{
    [TestFixture]
    public class PetToVaccinesMapperTests
    {
        [Test]
        public void ToCoreVaxTest()
        {
            var entity = new VaccineEntity()
            {
                Id = 1,
                VaxName = "Bordetella"
            };

            var coreVax = PetToVaccineMapper.ToCoreVax(entity);

            Assert.IsNotNull(coreVax);
            Assert.AreEqual(1, coreVax.Id);
            Assert.AreEqual("Bordetella", coreVax.VaxName);
        }

        [Test]
        public void ToVaccineStatusTest()
        {
            var entity = new PetToVaccine()
            {
                Id = 1,
                VaxId = 1,
                Inoculated = true,
                Vax = new VaccineEntity()
                {
                    Id = 1,
                    VaxName = "Bordetella"
                }
            };

            var vaccineStatus = PetToVaccineMapper.ToVaccineStatus(new List<PetToVaccine> { entity });

            Assert.IsNotNull(vaccineStatus);
            Assert.AreEqual(1, vaccineStatus.Count);

            var vax = vaccineStatus[0];
            Assert.AreEqual(1, vax.Id);
            Assert.AreEqual(1, vax.PetToVaccineId);
            Assert.AreEqual("Bordetella", vax.VaxName);
            Assert.IsTrue(vax.Inoculated);
        }

        [Test]
        public void FromVaccineStatusTest()
        {
            var vaccineStatus = new List<VaccineStatus>()
            {
                new VaccineStatus()
                {
                    Id = 1,
                    PetToVaccineId = 1,
                    VaxName = "Bordetella",
                    Inoculated = true
                }
            };

            var entity = PetToVaccineMapper.ToPetToVaccine(1, vaccineStatus);

            Assert.IsNotNull(entity);
            Assert.AreEqual(1, entity.Count);

            var vax = entity[0];
            Assert.AreEqual(1, vax.Id);
            Assert.AreEqual(1, vax.VaxId);
            Assert.AreEqual(1, vax.PetId);
            Assert.IsTrue(vax.Inoculated);
        }
    }
}
