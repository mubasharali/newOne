$(document).ready(function () {

    ko.bindingHandlers.dropzone = {
        init: function (element, valueAccessor) {
            var value = ko.unwrap(valueAccessor());

            var options = {
                maxFileSize: 15,
               // autoProcessQueue: false,
                uploadMultiple: true,
                parallelUploads: 100,
                maxFiles: 100,
                addRemoveLinks: true,
                clickable: '#dropzonePreview',
                init: function () {
                    var myDropzone = this;
                    this.on("success", function (file, serverFileName) {
                        fileList = [];
                        i = 1;
                        console.log(serverFileName);
                        var abc = $.map(serverFileName, function (item) { return (item); });
                        $.each(abc, function (index, value) {
                            fileList[i] = { "fileName": value, "fileId": i++ };
                        })
                        console.log(fileList);
                    });
                }
            };

            $.extend(options, value);

            $(element).addClass('dropzone');
            new Dropzone(element, options); // jshint ignore:line
        }
    };
});