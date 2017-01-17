using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BandToFitbit;

namespace BandToFitbitTests
{
    [TestClass]
    public class ManualBandDataProviderTest
    {        
        [TestMethod]
        public void Test()
        {
            BandDataProvider provider = new BandDataProvider();
            var result = provider.GetSteps().Result;
            Assert.IsNotNull(result);
        }
    }
}