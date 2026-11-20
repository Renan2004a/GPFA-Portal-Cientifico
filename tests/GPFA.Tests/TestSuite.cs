using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GPFA.Tests
{
    [TestClass]
    public static class TestSuite
    {
        [AssemblyInitialize]
        public static void Inicializar(TestContext contexto)
        {
            TestDatabase.Preparar();
        }
    }
}
