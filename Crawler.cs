/// <summary>
/// Copyright by Tin Trinh @ 2020
/// Project: Web Automation using Selenium WebDriver (ChromeDriver)
/// Background knowledge: Need to have
/// - Basic C# programming skill
/// - Skill to process data using Regular Expressions in C#
/// Guide:
/// - First, please update Nuget packages (Selenium WebDriver, Selenium Chrome Driver), Google Chrome to the latest version
/// - Run the demo code below to make sure everything works fine.
/// - Contact me at trinhtrongtinpp@gmail.com
/// </summary>

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace WebAutomation
{
    public class Crawler
    {
        //Browser
        private IWebDriver browser;
        private string pdLinks;
        //Constructor
        public Crawler()
        { }

        /// <summary>
        /// Run crawler
        /// </summary>
        /// <param name="productLinks">List of product links need to be crawled</param>
        /// <returns></returns>
        public List<Models.Product> Run()
        {
            //Create new browser instance using IWebDriver
            browser = new ChromeDriver();

            //Create list of product for saving crawled data
            List<Models.Product> listProducts = new List<Models.Product>();

            //Get linkproduct 
            WebDriverWait wait = new WebDriverWait(this.browser, TimeSpan.FromSeconds(60));
            browser.Navigate().GoToUrl("https://binhminhdigital.com/chan-may-anh/?p=1");

            List<string> pdLinks = new List<string>();

            //Category page

            for (var i = 1; i < 5 ; i++) //
            {
                wait.Until((x) =>
                {
                    return ((IJavaScriptExecutor)this.browser).ExecuteScript("return document.readyState").Equals("complete");
                });
                //var elementCat = browser.FindElement(By.CssSelector("[class=\"pro-item\"]")); // Next page button
                string htmlCourse = browser.PageSource;
                var sideBar = Regex.Matches(htmlCourse, @"pro-name(.*?)>", RegexOptions.Singleline); // search link products in each page
                foreach (var course in sideBar)
                {
                    string plink = Regex.Match(course.ToString(), @"href(.*?)title").Value.Replace("\" title", "").Replace("href=\"", "");
                    plink = "https://binhminhdigital.com/" + plink; // parse string
                    pdLinks.Add(plink);

                }
                string page = "?p=" + (i + 1); // Click on Next page
                string urlpage = "https://binhminhdigital.com/chan-may-anh/?p=1";
                string pagelink = urlpage.Replace("?p=1", page);
                browser.Navigate().GoToUrl(pagelink);
            }



            //Get product information
            //  Iterate though each product link to get product information
            foreach (var link in pdLinks)
            {
                if (link != "")
                {
                    var product = GetProductInformation(link);
                    listProducts.Add(product);
                }
            }

            //Close browser
            browser.Close();

            return listProducts;
        }

        static public bool verify(IWebDriver browser, string elementName)
        {
            try
            {
                bool isElementDisplayed = browser.FindElement(By.CssSelector(elementName)).Displayed;
                return true;
            }
            catch
            {
                return false;
            }
            return false;
        }



        /// <summary>
        /// Get product information
        /// </summary>
        /// <param name="productLink">Example
        private Models.Product GetProductInformation(string productLink)
        {
            //Create product to save crawled data
            Models.Product product = new Models.Product();

            //Redirect to site by URL
            //browser.Navigate().GoToUrl()
            System.Threading.Thread.Sleep(new Random().Next(2) * 1000); //Sleep random from 1-5 seconds
            WebDriverWait wait = new WebDriverWait(this.browser, TimeSpan.FromSeconds(120));

            browser.Navigate().GoToUrl(productLink);
            //Select elements by CSS Selector (easiest way)
            //You can also select element by ID, Class, Name, XPath,...

            //Select elements by CSS Selector (easiest way)
            //You can also select element by ID, Class, Name, XPath,...
            //Get brand by CSS Attribute Selectors (https://www.w3schools.com/css/css_attribute_selectors.asp)

            wait.Until((x) =>
            {
                return ((IJavaScriptExecutor)this.browser).ExecuteScript("return document.readyState").Equals("complete");
            });


            //Get SKU
            bool test;
            if (test = (verify(browser, "[itemprop = \"sku\"]") == true))
            {
                var element = browser.FindElement(By.CssSelector("[itemprop = \"sku\"]"));
                product.SKU = element.GetAttribute("innerHTML");
            }

            //Get Name
            if (test = (verify(browser, "[itemprop =\"url\"]") == true))
            {
                var element = browser.FindElement(By.CssSelector("[itemprop =\"url\"]"));
                product.Name = element.GetAttribute("innerHTML");
            }

            //Get Regular price
            if (test = (verify(browser, "[class=\"ngachngang\"]") == true))
            {
                var element = browser.FindElement(By.CssSelector("[class=\"ngachngang\"]"));
                var RegularPrice = element.GetAttribute("innerHTML");
                RegularPrice = RegularPrice.Replace(" VNĐ", ""); // Remove Currency
                double finalRegularPrice = Double.Parse(RegularPrice); // Parse to Double
                product.RegularPrice = finalRegularPrice;
            }

            //Get Sale Price
            if (test = (verify(browser, "[class=\"price_sale\"]") == true))
            {
                var element = browser.FindElement(By.CssSelector("[class=\"price_sale\"]"));
                var SalePrice = element.GetAttribute("innerHTML");
                if (SalePrice != "Liên hệ")
                {
                    SalePrice = SalePrice.Replace(" VNĐ", ""); // Remove Currency
                    double finalSalePrice = Double.Parse(SalePrice); // Parse to Double
                    product.SalePrice = finalSalePrice;
                }
                else
                {
                    SalePrice = SalePrice.Replace("Liên hệ", "9999999"); // Remove Currency
                    double finalSalePrice = Double.Parse(SalePrice); // Parse to Double
                    product.SalePrice = finalSalePrice;
                }
            }

            //Get Description
            if (test = (verify(browser, "[class=\"view-content\"]") == true))
            {
                var element = browser.FindElement(By.CssSelector("[class=\"view-content\"]"));
                var Description = element.GetAttribute("innerText");
                product.Description = Description.Replace("tại BinhMinhDigital", "").Replace("/s", "").Replace("\r", "").Replace("\n", "");

            }

            //Get Image

            //if (test = (verify(browser, "[itemprop=\"image\"]") == true))
            //{
            //    var element = browser.FindElement(By.CssSelector("[itemprop=\"image\"]"));
            //    product.Images = element.GetAttribute("src");
            //}

            // RegularExpression
            //var eelement = browser.FindElement(By.XPath("//picture"));
            //var ex = eelement.GetAttribute("innerHTML");
            //product.Images = Regex.Match(ex.ToString(), @"img src(.*?)alt").Value.Replace("img src=\"", "").Replace("\" alt", "");


            //Get Brand
            if (test = (verify(browser, "[itemprop=\"brand\"]") == true))
            {
                var element = browser.FindElement(By.CssSelector("[itemprop=\"brand\"]"));
                product.Attribute1Value = element.GetAttribute("innerHTML");
            }

            //Get AdvanceInfor
            if (test = (verify(browser, "[class=\"view-content\"]") == true))
            {
                var element = browser.FindElement(By.CssSelector("[class=\"product-recap\"]"));
                var AdvanceInfor = element.GetAttribute("innerText");
                product.Attribute2Value = AdvanceInfor = AdvanceInfor.Replace("TÍNH NĂNG NỔI BẬT", "").Replace("/s", "").Replace("\r", "").Replace("\n", "");

            }


            //Attribute Price
            if (product.SalePrice <= 1000000) { product.Attribute3Value = "<= 1 000 000"; };
            if (product.SalePrice > 1000000 && product.SalePrice <= 5000000) { product.Attribute3Value = "1 000 000 - 5 000 000"; };
            if (product.SalePrice > 5000000 && product.SalePrice <= 20000000) { product.Attribute3Value = "5 000 000 - 20 000 000"; };
            if (product.SalePrice > 2000000) { product.Attribute3Value = "> 20 000 000"; };

            //Get category - brand, thuộc tính category, brand > thuộc tính category
            product.Categories = "Chân Camera" + ">" + product.Attribute1Value;


            //Get Images Galleries
            string list = "";
            string htmlpage = browser.PageSource;
            var ListImages = Regex.Matches(htmlpage, @"data-standard(.*?)jpg", RegexOptions.Singleline);
            foreach (var course in ListImages)
            {
                string Gallery = Regex.Match(course.ToString(), @"data-standard(.*?)jpg").Value.Replace("data-standard=\"/", "https://binhminhdigital.com/");
                list += Gallery + ",";
            }

            product.Images = list.TrimEnd(',');



            //----------------------------------------------

            return product;
        }
    }
}