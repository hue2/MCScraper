using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MCProductScraper.Tests {
    [TestClass]
    public class ProgramTests {

        private static string url = "http://www.microcenter.com/search/search_results.aspx?Ntk=all&sortby=match&prt=clearance&N=4294966937&myStore=true";
        private WebClient client;

        [TestInitialize]
        public void TestInitialize() {
            client = new WebClient();
        }

        [TestMethod]
        public void GetPage_NoExceptionThrown() {
            Action act = () => client.GetPage(url);
            act.ShouldNotThrow();
        } 

        [TestMethod]
        public void GetPage_ExceptionForNullUrl() {
            Action act = () => client.GetPage(null);
            act.ShouldThrow<ArgumentNullException>();
        }


        [TestMethod]
        public void GetProductDictionary_CorrectType() {
            HtmlNodeCollection product = null;
            Action act = () => Program.GetProductDictionary(product);
            act.ShouldThrow<NullReferenceException>();
        }

    }
}
