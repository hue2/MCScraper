using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MCProductScraper {
    public class WebClient {

        public HtmlDocument GetPage(string url) {
            try {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";

                //Add the store cookie so filtering will work
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(new Uri(url), new Cookie("storeSelected", "155"));

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var stream = response.GetResponseStream();

                using (var reader = new StreamReader(stream)) {
                    string html = reader.ReadToEnd();
                    var doc = new HtmlDocument();
                    doc.LoadHtml(html);
                    return doc;
                }
            }
            catch (Exception ex) {
                throw ex.InnerException;
            }
        }
    }
}
