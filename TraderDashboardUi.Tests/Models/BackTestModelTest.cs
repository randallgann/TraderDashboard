using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using TraderDashboardUi.Models;

namespace TraderDashboardUi.Tests.Models
{
    [TestClass]
    public class BackTestModelTest
    {
        [TestMethod]
        public void BackTestModelTest_GetInstrumentsTest()
        {
            // arrange
            List<SelectListItem> stringList = new List<SelectListItem>();
            SelectListItem item1 = new SelectListItem { Text = "USD_JPY", Value = "USD_JPY", Selected = false };
            stringList.Add(item1);
            DateTime startDate = DateTime.Parse("01/23/1984");
            DateTime endDate = DateTime.Parse("02/25/1987");
            var model = new BackTestModel
            {
                Instruments = stringList,
                BackTestStartDate = startDate,
                BackTestEndDate = endDate
            };

            // act

            // assert
            Assert.IsNotNull(model);
        }
    }
}
