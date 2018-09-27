using europarser.global.singletone;
using europarser.models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace europarser.global
{
    public static class Variables
    {
        public static List<Category> CategoryList => new List<Category>
        {
            new Category {
                FinName = "Audio ja hifi",
                RusName = "Аудио",
                URL = new Uri("https://www.verkkokauppa.com/fi/catalog/28a/Audio-ja-hifi/products/")
            },
            new Category {
                FinName="Kodinkoneet",
                RusName="Бытовая техника",
                URL = new Uri("https://www.verkkokauppa.com/fi/catalog/19a/Kodinkoneet/products/")
            },
            new Category {
                FinName = "Kellot",
                RusName = "Часы",
                URL = new Uri("https://www.verkkokauppa.com/fi/catalog/58a/Kellot/products/")
            },
            new Category {
                FinName = "Kamerat",
                RusName = "Фото",
                URL = new Uri("https://www.verkkokauppa.com/fi/catalog/23a/Kamerat/products/")
            },
            new Category {
                FinName = "Kaapelit",
                RusName = "Кабели",
                URL = new Uri("https://www.verkkokauppa.com/fi/catalog/25a/Kaapelit/products/")
            },
            new Category {
                FinName = "Grillaus-ja-kokkaus",
                RusName = "Бытовые приборы",
                URL = new Uri("https://www.verkkokauppa.com/fi/catalog/31a/Grillaus-ja-kokkaus/products/")
            },
            new Category {
                FinName = "Komponentit",
                RusName = "ПК комплектующие",
                URL = new Uri("https://www.verkkokauppa.com/fi/catalog/6a/Komponentit/products/")
            },
            new Category {
                FinName = "Koti ja valaistus",
                RusName = "Дом и освещение",
                URL = new Uri("https://www.verkkokauppa.com/fi/catalog/20a/Koti-ja-valaistus/products/")
            },
            new Category {
                FinName = "Laukut ja matkailu",
                RusName = "Сумки и чемоданы",
                URL = new Uri("https://www.verkkokauppa.com/fi/catalog/56a/Laukut-ja-matkailu/products/")
            },
            new Category {
                FinName = "Lelut",
                RusName = "Игрушки",
                URL = new Uri("https://www.verkkokauppa.com/fi/catalog/16a/Lelut/products/")
            },
            new Category {
                FinName = "Lemmikit",
                RusName = "Для животных",
                URL = new Uri("https://www.verkkokauppa.com/fi/catalog/59a/Lemmikit/products/")
            },
            new Category {
                FinName = "Musiikki",
                RusName = "Музыка",
                URL = new Uri("https://www.verkkokauppa.com/fi/catalog/26a/Musiikki/products/")
            },
            new Category {
                FinName = "Oheislaitteet",
                RusName = "Офис",
                URL = new Uri("https://www.verkkokauppa.com/fi/catalog/3a/Oheislaitteet/products/")
            },
            new Category {
                FinName = "Ohjelmistot",
                RusName = "ПО",
                URL = new Uri("https://www.verkkokauppa.com/fi/catalog/9a/Ohjelmistot/products/")
            },
            new Category {
                FinName = "Pelit ja viihde",
                RusName = "Игры",
                URL = new Uri("https://www.verkkokauppa.com/fi/catalog/24a/Pelit-ja-viihde/products/")
            },
            new Category {
                FinName = "Pienkoneet",
                RusName = "Кухня",
                URL = new Uri("https://www.verkkokauppa.com/fi/catalog/27a/Pienkoneet/products/")
            },
            new Category {
                FinName = "Puhelimet",
                RusName = "Телефоны",
                URL=new Uri("https://www.verkkokauppa.com/fi/catalog/22a/Puhelimet/products/")
            },
            new Category {
                FinName = "Ruoka ja juoma",
                RusName = "Еда",
                URL=new Uri("https://www.verkkokauppa.com/fi/catalog/30a/Ruoka-ja-juoma/products/")
            },
            new Category {
                FinName = "Tarvike ja toimisto",
                RusName = "Офисные аксессуары",
                URL=new Uri("https://www.verkkokauppa.com/fi/catalog/21a/Tarvike-ja-toimisto/products/")
            },
            new Category {
                FinName = "Tietokoneet",
                RusName = "Ноутбуки",
                URL=new Uri("https://www.verkkokauppa.com/fi/catalog/5a/Tietokoneet/products/")
            },
            new Category {
                FinName = "TV ja video",
                RusName = "Телевизоры",
                URL=new Uri("https://www.verkkokauppa.com/fi/catalog/18a/TV-ja-video/products/")
            },
            new Category {
                FinName = "Urheilu",
                RusName = "Велосипеды",
                URL=new Uri("https://www.verkkokauppa.com/fi/catalog/14a/Urheilu/products/")
            },
            new Category {
                FinName = "Vauvat ja perhe",
                RusName = "Семья",
                URL=new Uri("https://www.verkkokauppa.com/fi/catalog/55a/Vauvat-ja-perhe/products/")
            },
            new Category {
                FinName = "Verkko",
                RusName = "Сеть",
                URL=new Uri("https://www.verkkokauppa.com/fi/catalog/10a/Verkko/products/")
            },
        };

        public static Queue<Category> CategoryQueue
        {
            get
            {
                Queue<Category> queue = new Queue<Category>();
                CategoryList.ForEach(category => queue.Enqueue(category));
                return queue;
            }
        }

        public static Crawler Crawler => Crawler.Instance as Crawler;
        public static Loader Loader => Loader.Instance;
        public static Parser Parser => Parser.Instance;
    }
}
