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

namespace WebAutomation
{
    class Program
    {
        static void Main(string[] args)
        {
            //Prepare list of product links

            //Run crawler
            Crawler crawler = new Crawler();
            List<Models.Product> listProduct = crawler.Run();

            //Generate WooCommerce Product Import template
            WooCommerceGenerator generator = new WooCommerceGenerator();
            generator.GenerateProductImportTemplate(listProduct, "D:\\Camerafoot.txt");
        } 
    }
}


