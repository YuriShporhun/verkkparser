using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using DataTransferObjects;
using europarser.global.statuses;
using europarser.models;
using europarser.repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace europarser.controllers
{
    public class LoaderController : Controller
    {
        [HttpPost]
        public IActionResult Upload(IFormFile file, string selectedAction)
        {
            if(!UploaderState.Instance.TryStart())
            {
                return Ok(Json(new
                {
                    text = "Загрузчик уже работает. Дождитесь завершения загрузки."
                }));
            }

            List<string> BadVendorCodes = new List<string>();

            switch (selectedAction)
            {
                case "marketSPB":
                case "marketMSK":
                case "availability":
                case "archive":
                case "price":
                case "addPrice":
                    break;

                case "default":
                    {
                        UploaderState.Instance.Reset();
                        return StatusCode(501, Json(new
                        {
                            text = "Выберите какое поле будете обновлять."
                        }));
                    }

                default:
                    {
                        UploaderState.Instance.Reset();
                        return StatusCode(501);
                    }
            }

            var workbook = new XLWorkbook(file.OpenReadStream());
            var ws1 = workbook.Worksheet(1);

            UploaderState.Instance.Excel();
            List<ExcelCodeValue> cells = new List<ExcelCodeValue>();

            for (int i = 2; i < int.MaxValue; i++)
            {
                string vendorCode = ws1.Cell($"A{i}").GetString();
                string cell = ws1.Cell($"B{i}").GetString();

                if (string.IsNullOrWhiteSpace(vendorCode) || string.IsNullOrWhiteSpace(cell))
                {
                    break;
                }

                if(vendorCode.ToLower() == "v00")
                {
                    continue;
                }

                vendorCode = vendorCode.Trim();
                cells.Add(new ExcelCodeValue
                {
                    VendorCode = vendorCode,
                    Value = cell
                });
            }

            UploaderState.Instance.Download();
            List<UMI_ObjectProperty> codes = new List<UMI_ObjectProperty>();
            try
            {
                codes = Repository.EuroMade.GetAllCodesWithObjId().ToList();
            }
            catch (MySqlException e)
            {
                UploaderState.Instance.Reset();
                return StatusCode(501, Json(new
                {
                    text = "Отправьте следующее сообщение разработчику: При загрузке артикулов случилось - " + e.Message
                }));
            }

            List<UMI_ObjectProperty> properties = new List<UMI_ObjectProperty>();
            foreach (var item in cells)
            {
                List<UMI_ObjectProperty> sameCodes = codes
                    .Where(c => c.Value.ToLower() == item.VendorCode.ToLower())
                    .ToList();

                if (!sameCodes.Any())
                {
                    BadVendorCodes.Add(item.VendorCode);
                }
                else
                {
                    properties.Add(new UMI_ObjectProperty
                    {
                        ObjectID = sameCodes.First().ObjectID,
                        Value = item.Value
                    });
                }
            }

            if (properties.Any())
            {
                UploaderState.Instance.Update();
                try
                {
                    Repository.EuroMade.UpdateFields(properties, selectedAction);
                }
                catch (MySqlException e)
                {
                    UploaderState.Instance.Reset();
                    return StatusCode(501, Json(new
                    {
                        text = "Отправьте следующее сообщение разработчику:" + e.Message
                    }));
                }
            }
            else
            {
                UploaderState.Instance.Success();
                return Ok(Json(new
                {
                    text = "Нет объектов свойства которых можно загрузить"
                }));
            }

            if (!BadVendorCodes.Any())
            {
                UploaderState.Instance.Success();
                return Ok(Json(new
                {
                    text = "Загрузка и обновление успешно завершены"
                }));
            }
            else
            {
                UploaderState.Instance.Success();
                string response = string.Join(" ", BadVendorCodes);
                return Ok(Json(new
                {
                    text = $"Загрузка и обновление завершены не полностью. Не загружены - {response}"
                }));
            }

            UploaderState.Instance.Reset();
        }
    }
}