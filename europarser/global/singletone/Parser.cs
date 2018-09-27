using AngleSharp;
using AngleSharp.Dom;
using DataTransferObjects;
using europarser.global.statuses;
using europarser.repository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace europarser.global.singletone
{
    public class Parser
    {
        Parser() { }
        static Parser instance = new Parser();

        public static Parser Instance => instance;

        ParserStatus status = ParserStatus.Ready;
        public ParserStatus Status => status;

        public int TotalObjectCount = 0;
        public int CurrentObject = 0;

        public async Task Run()
        {
            try
            {
                List<IdPrice> prices = Repository.Parser.GetAllPricesPrev().ToList();
                prices.Sort((p1, p2) => p1.ObjectID.CompareTo(p2.ObjectID));


                if (status != ParserStatus.Ready)
                {
                    return;
                }

                status = ParserStatus.Parsing;

                List<int> objects = Repository.EuroMade.GetAvailableObjects();
                List<int> downloaded = Repository.Parser.GetDownloadedObjects();
                List<int> objectsToCheck = new List<int>();

                foreach (var item in objects)
                {
                    if (!downloaded.Contains(item))
                    {
                        objectsToCheck.Add(item);
                    }
                }

                TotalObjectCount = objects.Count;
                int processedObject = downloaded.Count();

                foreach (var id in objectsToCheck)
                {
                    processedObject++;
                    CurrentObject = processedObject;
                    EuroItem euroItem = null;
                    try
                    {
                        euroItem = Repository.EuroMade.GetEuroItem(id);
                    }
                    catch(Exception e)
                    {

                    }

                    if (euroItem != null)
                    {
                        ParserItem parserItem = new ParserItem();

                        parserItem.EuroMadePrice = euroItem.EuroMadePrice;
                        parserItem.Uri = euroItem.Uri;
                        parserItem.UploadToMarket = euroItem.UploadToMarket;
                        parserItem.Name = euroItem.Name;
                        parserItem.ObjectID = euroItem.ObjectID;
                        parserItem.AvailabilityEuroMade = euroItem.Available;
                        parserItem.VendorCode = euroItem.VendorCode;

                        decimal prevPrice = 0;

                        try
                        {
                            prevPrice = prices.Where(e => e.ObjectID == parserItem.ObjectID).First().Price;
                        }
                        catch(InvalidOperationException)
                        {

                        }

                        parserItem.OriginalPrice = prevPrice;

                        parserItem.AvailabilityVerkk = false;
                        parserItem.AvailabilityCount1 = 0;
                        parserItem.AvailabilityCount2 = 0;
                        parserItem.Discount = false;
                        parserItem.DiscountValue = 0;
                        parserItem.Height = 0;
                        parserItem.Weight = 0;
                        parserItem.Length = 0;
                        parserItem.Width = 0;
                        parserItem.EAN = "0";
                        parserItem.VerkkPrice = 0;
                        parserItem.DiscountUntilDate = null;
                        parserItem.DiscountUntilTime = null;

                        var config = Configuration.Default.WithDefaultLoader();
                        IDocument document = null;

                        try
                        {
                            document = await BrowsingContext.New(config).OpenAsync(euroItem.Uri);
                        }
                        catch (Exception)
                        {

                        }

                        var cellSelector = ".product-basic-details .add-to-cart .vk-button";
                        var button = document.QuerySelector(cellSelector);

                        if (button == null)
                        {
                            continue;
                        }

                        if (document.QuerySelector("#avail-0-content") == null)
                        {
                            try
                            {
                                Repository.Parser.Insert(parserItem);
                            }
                            catch (Exception e)
                            {

                            }
                            continue;
                        }
                        else
                        {
                            parserItem.AvailabilityVerkk = true;
                        }

                        IElement parameters = null;

                        try
                        {
                            parameters = document.QuerySelectorAll(".product-details__category")?[1];
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            parameters = null;
                        }

                        string size = null;
                        if (parameters != null)
                        {
                            try
                            {
                                size = parameters?.QuerySelectorAll(".product-details-row__value")?[0]?.TextContent;
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                size = null;
                            }
                        }

                        string weight = null;
                        if (parameters != null)
                        {
                            try
                            {
                                weight = parameters?.QuerySelectorAll(".product-details-row__value")?[1]?.TextContent;
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                weight = null;
                            }
                        }

                        string ean = null;

                        try
                        {
                            ean = document.QuerySelectorAll(".product-details__category")?[0]
                                ?.QuerySelectorAll(".product-details-row__value")?[2]
                                ?.TextContent;
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            ean = null;
                        }

                        if (ean == null)
                        {
                            parserItem.EAN = "0";
                        }
                        else
                        {
                            parserItem.EAN = ean;
                        }

                        if (weight == null)
                        {
                            parserItem.Weight = 0;
                        }
                        else
                        {
                            if (double.TryParse(weight.Replace("kg", string.Empty).Trim(), out double weightValue))
                            {
                                parserItem.Weight = weightValue;
                            }
                            else
                            {
                                parserItem.Weight = 0;
                            }
                        }

                        if (size == null)
                        {
                            parserItem.Height = 0;
                            parserItem.Width = 0;
                            parserItem.Length = 0;
                        }
                        else
                        {
                            List<string> sizes = size
                                .Replace("cm", string.Empty)
                                .Split('x')
                                .ToList();

                            if (sizes.Count == 3)
                            {
                                if (int.TryParse(sizes[0].Trim(), out int width))
                                {
                                    parserItem.Width = width;
                                }
                                else
                                {
                                    parserItem.Width = 0;
                                }

                                if (int.TryParse(sizes[1].Trim(), out int height))
                                {
                                    parserItem.Height = height;
                                }
                                else
                                {
                                    parserItem.Height = 0;
                                }

                                if (int.TryParse(sizes[2].Trim(), out int length))
                                {
                                    parserItem.Length = Convert.ToInt32(sizes[2].Trim());
                                }
                                else
                                {
                                    parserItem.Length = 0;
                                }
                            }
                            else
                            {
                                parserItem.Height = 0;
                                parserItem.Width = 0;
                                parserItem.Length = 0;
                            }

                        }

                        var discountValue = document.QuerySelector(".price-tag-discount__amount");

                        if (discountValue != null)
                        {
                            parserItem.Discount = true;
                            string discountString = discountValue.TextContent
                                .Replace("€", string.Empty)
                                .Replace(",", ".");

                            if (decimal.TryParse(discountString, NumberStyles.AllowParentheses | NumberStyles.Float, CultureInfo.InvariantCulture, out decimal discountOutValue))
                            {
                                parserItem.DiscountValue = Math.Abs(discountOutValue);
                            }
                            else
                            {
                                parserItem.DiscountValue = 0;
                            }
                        }
                        else
                        {
                            parserItem.Discount = false;
                            parserItem.DiscountValue = 0;
                        }

                        var price = document.QuerySelector(".price-tag-content__price-tag-price--current .price-tag-price__euros")
                            ?.GetAttribute("content");

                        if (price != null)
                        {
                            if (decimal.TryParse(price, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal priceVerkk))
                            {
                                parserItem.VerkkPrice = priceVerkk;
                            }
                            else
                            {
                                parserItem.VerkkPrice = 0;
                            }
                        }
                        else
                        {
                            parserItem.VerkkPrice = 0;
                        }

                        var available1 = document.QuerySelector(".stock-indicator__link")
                            ?.TextContent
                            ?.Replace("yli", string.Empty)
                            ?.Trim();

                        var available2 = document.QuerySelector(".product-info-row__location")
                            ?.TextContent
                            ?.Replace("yli", string.Empty)
                            ?.Trim();

                        IHtmlCollection<IElement> available3row = null;
                        string available3 = null;

                        try
                        {
                            available3row = document.QuerySelectorAll(".product-info-row");

                            foreach (var item in available3row)
                            {
                                var name = item.QuerySelector(".product-info-row__name")?.TextContent ?? string.Empty;
                                if (name == "Vantaan varasto")
                                {
                                    available3 = document.QuerySelector(".product-info-row__location")
                                        ?.TextContent
                                        ?.Replace("yli", string.Empty)
                                        ?.Trim();
                                }
                            }
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            available3row = null;
                        }

                        if (available1 == null)
                        {
                            parserItem.AvailabilityCount1 = 0;
                        }
                        else
                        {
                            if (int.TryParse(available1, out int available1value))
                            {
                                parserItem.AvailabilityCount1 = available1value;
                            }
                            else
                            {
                                parserItem.AvailabilityCount1 = 0;
                            }
                        }

                        if (available2 == null)
                        {
                            parserItem.AvailabilityCount2 = 0;
                        }
                        else
                        {
                            if (int.TryParse(available2, out int available2value))
                            {
                                parserItem.AvailabilityCount2 = available2value;
                            }
                            else
                            {
                                parserItem.AvailabilityCount2 = 0;
                            }
                        }

                        if (available3 == null)
                        {
                            parserItem.AvailabilityCount3 = 0;
                        }
                        else
                        {
                            if (int.TryParse(available3, out int available3value))
                            {
                                parserItem.AvailabilityCount3 = available3value;
                            }
                            else
                            {
                                parserItem.AvailabilityCount3 = 0;
                            }
                        }

                        var discountDate = document.QuerySelector(".discount-info-details em")?.TextContent;

                        if (discountDate != null)
                        {
                            Regex dateReg = new Regex(@"(\d+\.\d+\.\d+)");
                            Regex timeReg = new Regex(@"( \d+\.\d+ )");

                            Match dateMatch = dateReg.Match(discountDate);
                            if (dateMatch.Success)
                            {
                                try
                                {
                                    parserItem.DiscountUntilDate = DateTime.ParseExact(dateMatch.Value, @"d.M.yyyy", CultureInfo.InvariantCulture);
                                }
                                catch (FormatException e)
                                {

                                }
                            }

                            Match timeMatch = timeReg.Match(discountDate);
                            if (timeMatch.Success)
                            {
                                try
                                {
                                    DateTime dt = new DateTime();
                                    dt = DateTime.ParseExact(timeMatch.Value.Trim(), @"H.m", CultureInfo.InvariantCulture);
                                    parserItem.DiscountUntilTime = new TimeSpan(0, dt.Hour, dt.Minute, dt.Second, 0);
                                }
                                catch (FormatException e)
                                {

                                }
                            }
                        }

                        try
                        {
                            Repository.Parser.Insert(parserItem);
                        }
                        catch (Exception e)
                        {

                        }
                    }
                }

                List<IdPrice> pricesParser = Repository.Parser.GetAllPricesParser().ToList();
                prices.Sort((p1, p2) => p1.ObjectID.CompareTo(p2.ObjectID));

                Repository.Parser.TruncatePrices();
                Repository.Parser.SavePrices(pricesParser);

                status = ParserStatus.Done;
            }
            catch(Exception e)
            {

            }
        }
    }
}
