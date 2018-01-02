using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace MCProductScraper
{
    class Program
    {
        static readonly string domain = System.Configuration.ConfigurationManager.AppSettings["sandbox"];
        static readonly string apiKey = System.Configuration.ConfigurationManager.AppSettings["apiKey"];
        static readonly string apiClient = System.Configuration.ConfigurationManager.AppSettings["apiClient"];
        static readonly string senderEmail = System.Configuration.ConfigurationManager.AppSettings["senderEmail"];
        static readonly string fileLocation = "";

        public static void Main(string[] args)
        {
            RequestProductData();
        }

        public static IRestResponse SendEMail(string messageBody)
        {

            var client = new RestClient(apiClient);
            client.Authenticator = new HttpBasicAuthenticator("api", apiKey);
            var request = new RestRequest(Method.POST);
            request.AddParameter("from", string.Format("MC Scraper <{0}>", senderEmail));
            
            //change the 'to' parameter whatever address you want
            request.AddParameter("to", "example@email.com");
            request.AddParameter("subject", "Open box from MicroCenter");
            request.AddParameter("text", messageBody);
            return client.Execute(request);
        }

        public static void RequestProductData()
        {
            HtmlAgilityPack.HtmlWeb web = new HtmlAgilityPack.HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = web.Load("http://www.microcenter.com/search/search_results.aspx?Ntk=all&sortby=match&prt=clearance&N=4294966937&myStore=true");

            var product = doc.DocumentNode.SelectNodes("//li[@class='product_wrapper']");
            string url = "";
            string cardName = "";

            Dictionary<string, string> productDictionary = new Dictionary<string, string>();

            foreach (var listing in product)
            {
                var productNode = listing.SelectSingleNode(".//a[@data-category='Video Cards']");

                if (productNode != null)
                {
                    url = string.Format("{0}{1}", "http://www.microcenter.com", productNode.Attributes["href"].Value);
                    var imgNode = productNode.SelectSingleNode(".//img[@class='SearchResultProductImage']");
                    cardName = imgNode.Attributes["alt"].Value;
                }

                var stock = listing.SelectSingleNode(".//div[@class='stock']");
                if (stock != null)
                {
                    var openBoxStockInfo = stock.ChildNodes["strong"].InnerText;
                    if (openBoxStockInfo.Contains("open box"))
                    {              
                        var openBoxPrice = listing.SelectSingleNode(".//div[@class='price-label compareTo']").InnerText;
                        string nameAndUrl = string.Format("{1} {0}: {3} {1} {2}", openBoxStockInfo, Environment.NewLine, url, openBoxPrice);
                        productDictionary.Add(cardName, nameAndUrl);
                    }
                }
            }

            var result = string.Join("\n\n", productDictionary.Select(m => m.Key + ":" + m.Value).ToArray());
            string previousMsg = System.IO.File.ReadAllText(@fileLocation);

            if (previousMsg != result)
            {
                //input the location of the file 
                System.IO.File.WriteAllText(@fileLocation, result);
                var x = SendEMail(result);
            }       
        }
    }
}
