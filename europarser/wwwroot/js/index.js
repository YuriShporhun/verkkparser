(function () {
    "use strict";

    $(function () {
        window.setInterval(function () {
            checkAppState();
        }, 2000);

        const checkAppState = function () {
            $.ajax({
                url: "/Ajax/CheckAppState",
                method: "POST"
            }).done(function (response) {
                $('#crawler-status-cell').text(response.crawlerState);
                $('#crawler-html-cell').html(response.isCrawlerHtmlReady);
                $('#parser-excel-cell').html(response.isParserExcelReady);
                $('#crawler-excel-cell').html(response.isCrawlerExcelReady);
                $('#loader-status-cell').text(response.loaderState);
                $('#parser-progress-cell').html(response.parserProgress);
                $('#parser-status-cell').text(response.parserState);
                $('#loader-progress-cell').html(response.loaderProgress);
            })
        } 

        function ShowModal(title, body) {
            $('#response-modal .modal-title').html(title)
            $('#response-modal .modal-body p').html(body)
            $('#response-modal').modal('show', { backdrop: 'static' });
        }

        function StartCrawler() {
            $.ajax({
                url: "/Ajax/StartCrawler",
                method: "POST"
            }).done(function (response) {
                ShowModal("Ответ от кроулера", response.value.text);
            })
        }

        function StartLoader() {
            $.ajax({
                url: "/Ajax/StartLoader",
                method: "POST"
            }).done(function (response) {
                ShowModal("Ответ от загрузчика", response.value.text);
            })
        }

        function StartParser() {
            $.ajax({
                url: "/Ajax/StartParser",
                method: "POST"
            }).done(function (response) {
                ShowModal("Ответ от парсера", response.value.text);
            })
        }

        $("#start-crawler-button").click(function () {
            StartCrawler();
        })

        $("#start-loader-button").click(function () {
            StartLoader();
        })

        $("#start-parser-button").click(function () {
            StartParser();
        })

        $(document).on("click", "#show-crawler-excel", function () {
            location.href = "/crawler/excel"
        })

        $(document).on("click", "#show-crawler-data", function () {
            location.href = "/crawler/html"
        })

        $(document).on("click", "#show-parser-excel", function () {
            location.href = "/parser/excel"
        })
    })
})();