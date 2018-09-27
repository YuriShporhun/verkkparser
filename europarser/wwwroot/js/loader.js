$(function myfunction() {
    window.setInterval(function () {
        checkLoaderState();
    }, 2000);

    const checkLoaderState = function () {
        $.ajax({
            url: "/Ajax/CheckLoaderState",
            method: "POST"
        }).done(function (response) {
            $('#current-status').text(response.status);
        })
    } 

    function ShowModal(title, body) {
        $('#response-modal .modal-title').html(title)
        $('#response-modal .modal-body p').html(body)
        $('#response-modal').modal('show', { backdrop: 'static' });
    }

    var previewNode = document.getElementById("preview-template");
    previewNode.id = "";
    var previewTemplate = previewNode.parentNode.innerHTML;
    previewNode.parentNode.removeChild(previewNode);

    $("#excel-loader").dropzone({
        url: "/loader/upload",
        addRemoveLinks: false,
        maxFilesize: 15,
        clickable: false,
        paramName: "file",
        acceptedFiles: ".xlsx",
        createImageThumbnails: false,
        previewTemplate: previewTemplate,

        init: function () {
            this.on("sending", function (file, xhr, formData) {
                formData.append('selectedAction', $('#select-action').find(":selected").val());
            });
        },
        success: function (file, response) {
            ShowModal("Результат", response.value.text);
        },
        error: function (file, errorMessage, xhr) {
            ShowModal("Ошибка " + xhr.status, errorMessage.value.text);
        },
    });
})

