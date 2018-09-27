using europarser.global.statuses;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using static europarser.global.Variables;
using europarser.models;

namespace europarser.controllers
{
    public class AjaxController : Controller
    {
        [HttpPost]
        public IActionResult CheckAppState() => Json(AppState.GetState());

        [HttpPost]
        public IActionResult CheckLoaderState() => Json(new
        {
            status = UploaderState.Instance.Status.ToAppStateName()
        });

        [HttpPost]
        public IActionResult StartParser()
        {
            switch (Parser.Status)
            {
                case ParserStatus.Ready:
                    {
                        Task.Run(() => Parser.Run());

                        return Ok(Json(new
                        {
                            text = "Парсер запущен."
                        }));
                    }
                case ParserStatus.Parsing:
                    {
                        return Ok(Json(new
                        {
                            text = "Парсер уже был запущен ранее. Воспользуйтесь командами остановки или паузы."
                        }));
                    }
                case ParserStatus.Done:
                    return null;
                default:
                    return null;
            }
        }

        [HttpPost]
        public IActionResult StartLoader()
        {
            switch (Loader.Status)
            {
                case LoaderStatus.Ready:
                    {
                        Task.Run(() => Loader.Run());

                        return Ok(Json(new
                        {
                            text = "Загрузчик запущен."
                        }));
                    }
                case LoaderStatus.Disconnected:
                    {
                        Loader.ResetState();
                        Task.Run(() => Loader.Run());
                        return Ok(Json(new
                        {
                            text = "Загрузчик перезапущен."
                        }));
                    }
                case LoaderStatus.Download:
                    return null;
                case LoaderStatus.Upload:
                    return null;
                case LoaderStatus.Done:
                    return null;
                default:
                    return null;
            }
        }

        [HttpPost]
        public IActionResult StartCrawler()
        {
            switch(Crawler.Status)
            {
                case CrawlerStatus.Ready:
                    {
                        Task.Run(() =>  Crawler.Run());

                        return Ok(Json(new
                        {
                            text = "Кроулер запущен."
                        }));
                    }
                case CrawlerStatus.Run:
                    {
                        return Ok(Json(new
                        {
                            text = "Кроулер уже был запущен ранее. Воспользуйтесь командами остановки или паузы."
                        }));
                    }
                case CrawlerStatus.Done:
                    {
                        return Ok(Json(new
                        {
                            text = "Кроулер завершил работу и ожидает скачивания результата. " +
                            "Для возможности запуска нажмите перезапустить."
                        }));
                    }
                default: return StatusCode(501);
            }
        }
    }
}