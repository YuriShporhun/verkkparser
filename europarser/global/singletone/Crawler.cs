using AngleSharp;
using DataTransferObjects;
using europarser.global.statuses;
using europarser.interfaces;
using europarser.models;
using europarser.repository;
using EuroRepository.models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static europarser.global.Variables;

namespace europarser.global.singletone
{
    /// <summary>
    /// Отвечает за предоставление услуги кроулера
    /// </summary>
    /// <remarks>
    /// Синглтон
    /// </remarks>
    public class Crawler : IIPOwner
    {
        Crawler() { }
        static IIPOwner instance = new Crawler();
        Queue<Category> CategoryQueue = new Queue<Category>();
        public static IIPOwner Instance => instance;

        CrawlerStatus status = CrawlerStatus.Ready;
        public CrawlerStatus Status => status;

        public async void Run()
        {
            if (Status != CrawlerStatus.Ready)
                return;

            List<string> codesList = Repository.EuroMade.GetAllVendorCodes().ToList();
            codesList = codesList.Select(x => x.ToLower()).ToList();

            HashSet<string> codes = new HashSet<string>(codesList);

            status = CrawlerStatus.Run;

            CategoryQueue = Variables.CategoryQueue;
            const string domainName = "https://www.verkkokauppa.com";

            while (CategoryQueue.Any())
            {
                Category currentCategory = CategoryQueue.Dequeue();
                var pageIndex = 1;

                var currentURL = currentCategory.URL.ToString() + pageIndex;
                var config = Configuration.Default.WithDefaultLoader();
                var document = await BrowsingContext.New(config).OpenAsync(currentURL);

                var pageSelector = ".page-selector__page-count-indicator";
                var pagesCountString = document.QuerySelector(pageSelector)
                    .TextContent
                    .Replace("/", string.Empty)
                    .Trim();

                if(!int.TryParse(pagesCountString, out int pagesCountValue))
                {
                    continue;
                }

                for(pageIndex = 1; pageIndex <= pagesCountValue; pageIndex++)
                {
                    currentURL = currentCategory.URL.ToString() + pageIndex;
                    config = Configuration.Default.WithDefaultLoader();
                    document = await BrowsingContext.New(config).OpenAsync(currentURL);

                    var cellSelector = ".list-product";
                    var cells = document.QuerySelectorAll(cellSelector);
                    var siteItems = new List<VerkkItem>();

                    foreach (var c in cells)
                    {
                        List<string> problems = new List<string>();

                        int vendorCode = -1;
                        string name = "error_value";
                        Uri url = new Uri("https://www.ERROR_URL.com");
                        bool discount = false;
                        Decimal price = new decimal(-1);

                        try
                        {
                            bool isConverted = int.TryParse(c.QuerySelectorAll(".image__product-id").Select(e => e.TextContent).First(), out int number);
                            if (!isConverted)
                            {
                                problems.Add("Артикул не является числом.");
                            }
                            vendorCode = number;
                        }
                        catch (InvalidOperationException)
                        {
                            problems.Add("Артикул не найден.");
                        }

                        try
                        {
                            name = c.QuerySelectorAll(".list-product-link__name")
                                .Select(e => e.TextContent)
                                .First();
                        }
                        catch (InvalidOperationException)
                        {
                            problems.Add("Наименование товара не найдено");
                        }

                        try
                        {
                            string partialUrl = c.QuerySelectorAll(".data__list-product-link")
                                .Select(e => e.Attributes.GetNamedItem("href").Value)
                                .First();

                            url = new Uri(domainName + partialUrl);
                        }
                        catch (InvalidOperationException)
                        {
                            problems.Add("URL не найдено");
                        }
                        catch (UriFormatException)
                        {
                            problems.Add("Не удается преобразовать URL");
                        }

                        string itemUrl = "https://www.verkkokauppa.com" + c.QuerySelector(".data__list-product-link")?.GetAttribute("href");
                        bool uploaded = false;

                        if(vendorCode != -1)
                        {
                            if (codes.Contains("v" + vendorCode))
                            {
                                uploaded = true;
                            }
                        }

                        var goodsPage = await BrowsingContext.New(config).OpenAsync(itemUrl);

                        var discountList = goodsPage.QuerySelector(".price-tag-discount__amount");

                        if (discountList != null)
                        {
                            discount = true;
                        }
                        else
                        {
                            discount = false;
                        }

                        try
                        {
                            var priceVerkk = goodsPage
                                .QuerySelector(".price-tag-content__price-tag-price--current .price-tag-price__euros")
                                ?.GetAttribute("content");

                            bool isConverted = decimal.TryParse(
                                priceVerkk,
                                NumberStyles.AllowDecimalPoint, 
                                CultureInfo.InvariantCulture, 
                                out decimal priceVal);

                            if (!isConverted)
                            {
                                problems.Add("Цена не является числом.");
                            }
                            price = priceVal;
                        }
                        catch (InvalidOperationException)
                        {
                            problems.Add("Цена не найдена.");
                        }

                        if (problems.Any())
                        {
                            Repository.Logger.Log(ErrorFrom.Crawler, problems, new Uri(currentURL));
                        }
                        else
                        {
                            siteItems.Add(new VerkkItem
                            {
                                Discount = discount,
                                Name = name,
                                PriceEuro = price,
                                ULR = url,
                                VendorCode = vendorCode,
                                Cathegory = currentCategory.RusName,
                                Uploaded = uploaded
                            });
                        }
                    }

                    if (siteItems.Any())
                    {
                        Repository.VerkkItems.Insert(siteItems);
                    }
                }               
            }

            status = CrawlerStatus.Done;
        }
    }
}
