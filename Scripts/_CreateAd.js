$(document).ready(function () {
   // Dropzone.autoDiscover = false;
    Dropzone.options.myAwesomeDropzone = {
        autoProcessQueue: false,
        uploadMultiple: true,
        parallelUploads: 100,
        maxFiles: 100,
        addRemoveLinks: true,
        autoDiscover : false,
        clickable: '#dropzonePreview',
        init: function () {
            var myDropzone = this;
            this.element.querySelector("button[type=submit]").addEventListener("click", function (e) {
                e.preventDefault();
                e.stopPropagation();
                myDropzone.processQueue();
            });
            this.on("sendingmultiple", function () {
            });
            this.on("successmultiple", function (files, response) {
            });
            this.on("errormultiple", function (files, response) {
            });
        }

    }
});