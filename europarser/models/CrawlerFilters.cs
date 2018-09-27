using europarser.global;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace europarser.models
{
    public class CrawlerFilters
    {
        public List<SelectListItem> Categories = new List<SelectListItem>();
        public List<SelectListItem> Loaded = new List<SelectListItem>();
        public List<SelectListItem> Discount = new List<SelectListItem>();

        public string SCategory { get; set; }
        public string SLoaded { get; set; }
        public string SDiscount { get; set; }

        public CrawlerFilters()
        {
            Categories.Add(new SelectListItem
            {
                Text = "Все",
                Value = "all"
            });
            foreach (var item in Variables.CategoryList)
            {
                Categories.Add(new SelectListItem
                {
                    Text = item.RusName,
                    Value = item.RusName,
                });
            }
            Categories[0].Selected = true;

            Loaded.AddRange(new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Все",
                    Value = "all",
                    Selected = true
                },
                new SelectListItem
                {
                    Text = "Загружены на сайт",
                    Value = "loaded"
                },
                new SelectListItem
                {
                    Text = "Без загруженных на сайт",
                    Value = "not_loaded"
                }
            });

            Discount.AddRange(new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Все товары",
                    Value = "all",
                    Selected = true
                },
                new SelectListItem
                {
                    Text = "Товары с акциями",
                    Value = "discount"
                }
            });
        }
    }
}
