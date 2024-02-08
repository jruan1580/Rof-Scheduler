using DatamartManagementService.Domain.Mappers.DTO;
using NUnit.Framework;

namespace DatamartManagementService.Test.Mapper
{
    [TestFixture]
    public class DTOMapperTest
    {
        [Test]
        public void ToDTOPetServices()
        {            
            var corePetService = ModelCreator.GetCorePetServices();

            var dtoPetService = PetServicesDTOMapper.ToDTOPetServices(corePetService);

            Assert.IsNotNull(dtoPetService);
            Assert.AreEqual(corePetService.Id, dtoPetService.Id);
            Assert.AreEqual(corePetService.ServiceName, dtoPetService.ServiceName);
        }

        public void ToDTOPayrollSummaryPerEmployee()
        {
            var corePayrollSummaryPerEmployee = ModelCreator.GetCorePayrollSummaryPerEmployee();

            var dtoPayrollSummaryPerEmployee = PayrollSummaryPerEmployeeDTOMapper.ToDTOPayrollSummaryPerEmployee(corePayrollSummaryPerEmployee);

            Assert.IsNotNull(dtoPayrollSummaryPerEmployee);
            Assert.AreEqual(corePayrollSummaryPerEmployee.FirstName, dtoPayrollSummaryPerEmployee.FirstName);
            Assert.AreEqual(corePayrollSummaryPerEmployee.LastName, dtoPayrollSummaryPerEmployee.LastName);
            Assert.AreEqual(corePayrollSummaryPerEmployee.TotalPay, dtoPayrollSummaryPerEmployee.TotalPay);
        }
    }
}
