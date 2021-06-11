using Microsoft.VisualStudio.TestTools.UnitTesting;
using static VaccinationsProgram.SimulationMethods;

namespace VaccinationProgramTests
{
    [TestClass]
    public class TestSimulationMethods
    {
        [TestMethod]
        public void GetDoseQuantity_ZeroDosesEntered_EnterHigherNumber()
        {
            GetDoseQuantity();
        }
    }
}
