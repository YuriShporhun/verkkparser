using ClosedXML.Excel;
using DataTransferObjects;
using europarser.models;
using europarser.repository;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace europarser.controllers
{
    public class DataController : Controller
    {
        [Route("crawler/html")]
        [HttpGet]
        public IActionResult CrawlerHtml()
        {     
            return View("~/views/data/crawlerhtml.cshtml", new CrawlerModel { filters=new CrawlerFilters(), items=Repository.VerkkItems.Get() });
        }

        [Route("crawler/html")]
        [HttpPost]
        public IActionResult CrawlerHtml(CrawlerModel model)
        {
            var items = Repository.VerkkItems.Get();
            if(model.filters.SCategory != "all")
            {
                items = items.Where(i => i.Cathegory == model.filters.SCategory).ToList();
            }

            switch (model.filters.SDiscount)
            {
                case "discount":
                    items = items.Where(i => i.Discount == true).ToList();
                    break;               
                default:
                    break;
            }
            switch (model.filters.SLoaded)
            {
                case "loaded":
                    items = items.Where(i => i.Uploaded == true).ToList();
                    break;
                case "not_loaded":
                    items = items.Where(i => i.Uploaded == false).ToList();
                    break;
                default:
                    break;
            }
            return View("~/views/data/crawlerhtml.cshtml", new CrawlerModel { filters = new CrawlerFilters(), items = items });
        }

        [Route("crawler/excel")]
        public FileResult CrawlerExcel()
        {
            XLWorkbook workbook = new XLWorkbook();
            var sheet = workbook.AddWorksheet("Товары verkk");
            List<VerkkItem> items = Repository.VerkkItems.Get();

            sheet.Cell("A1").Value = "Артикул";
            sheet.Cell("B1").Value = "Категория";
            sheet.Cell("C1").Value = "Наименование";
            sheet.Cell("D1").Value = "URL";
            sheet.Cell("E1").Value = "Цена verkk";
            sheet.Cell("F1").Value = "Есть на Euromade";

            int i = 2;
            foreach (var item in items)
            {
                sheet.Cell($"A{i}").Value = "V" + item.VendorCode;
                sheet.Cell($"B{i}").Value = item.Cathegory;
                sheet.Cell($"C{i}").Value = item.Name;
                sheet.Cell($"D{i}").Value = item.ULR;
                sheet.Cell($"E{i}").Value = item.PriceEuro;
                sheet.Cell($"F{i}").Value = item.Uploaded;
                i++;
            }

            var mimeType = "application/vnd.ms-excel";
            MemoryStream ms = new MemoryStream();
            workbook.SaveAs(ms);
            return File(ms.ToArray(), mimeType, "crawler.xlsx");
        }

        [Route("parser/excel")]
        public FileResult ParserExcel()
        {
            XLWorkbook workbook = new XLWorkbook();
            var sheet = workbook.AddWorksheet("verkkokauppa");
            List<ParserItem> items = Repository.Parser.SelectAll();

            sheet.Cell("A1").Value = "Артикул";
            sheet.Cell("B1").Value = "Цена ЕВРО (Verkk)";
            sheet.Cell("C1").Value = "Наличие 1";
            sheet.Cell("D1").Value = "Наличие 2";
            sheet.Cell("E1").Value = "В продаже (Verkk)";
            sheet.Cell("F1").Value = "Длина";
            sheet.Cell("G1").Value = "Ширина";
            sheet.Cell("H1").Value = "Высота";
            sheet.Cell("I1").Value = "Есть скидка";
            sheet.Cell("J1").Value = "Размер скидки";
            sheet.Cell("K1").Value = "URL";
            sheet.Cell("L1").Value = "Наименование";
            sheet.Cell("M1").Value = "Выгружать на маркет";
            sheet.Cell("N1").Value = "Цена на EuroMade";
            sheet.Cell("O1").Value = "EAN";
            sheet.Cell("P1").Value = "Вес";
            sheet.Cell("Q1").Value = "Наличие 3";
            sheet.Cell("R1").Value = "Наличие Euromade";
            sheet.Cell("S1").Value = "Закупочная цена";
            sheet.Cell("T1").Value = "Длительность скидки дата (Хельсинки)";
            sheet.Cell("U1").Value = "Длительность скидки время (Хельсинки)";

            int i = 2;
            foreach (var item in items)
            {
                sheet.Cell($"A{i}").Value = item.VendorCode;
                sheet.Cell($"B{i}").Value = item.VerkkPrice;
                sheet.Cell($"C{i}").Value = item.AvailabilityCount1;
                sheet.Cell($"D{i}").Value = item.AvailabilityCount2;
                sheet.Cell($"E{i}").Value = item.AvailabilityVerkk;
                sheet.Cell($"F{i}").Value = item.Length;
                sheet.Cell($"G{i}").Value = item.Width;
                sheet.Cell($"H{i}").Value = item.Height;
                sheet.Cell($"I{i}").Value = item.Discount;
                sheet.Cell($"J{i}").Value = item.DiscountValue;
                sheet.Cell($"K{i}").Value = item.Uri;
                sheet.Cell($"L{i}").Value = item.Name;
                sheet.Cell($"M{i}").Value = item.UploadToMarket;
                sheet.Cell($"N{i}").Value = item.EuroMadePrice;
                sheet.Cell($"O{i}").Value = item.EAN;
                sheet.Cell($"P{i}").Value = item.Weight;
                sheet.Cell($"Q{i}").Value = item.AvailabilityCount3;
                sheet.Cell($"R{i}").Value = item.AvailabilityEuroMade;
                sheet.Cell($"S{i}").Value = item.OriginalPrice;
                sheet.Cell($"T{i}").Value = item.DiscountUntilDate;
                sheet.Cell($"U{i}").Value = item.DiscountUntilTime;
                i++;
            }

            var mimeType = "application/vnd.ms-excel";
            MemoryStream ms = new MemoryStream();
            workbook.SaveAs(ms);
            return File(ms.ToArray(), mimeType, "Parser.xlsx");
        }
    }
}
 