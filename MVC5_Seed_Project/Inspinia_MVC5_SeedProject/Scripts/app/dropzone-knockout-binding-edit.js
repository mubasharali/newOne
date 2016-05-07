ko.bindingHandlers.dropzone = {
    init: function (element, valueAccessor) {
        var value = ko.unwrap(valueAccessor());

        var options = {
            maxFileSize: 15,
            uploadMultiple: true,
            parallelUploads: 100,
            maxFiles: 30,
            addRemoveLinks: false,
            acceptedFiles: ".jpeg,.jpg,.png,.gif",
            init: function () {
                var myDropzone = this;
                myDropzone1 = this;
                this.on("addedfile", function (file) {
                    isFilesUploading = true;
                });
                this.on("success", function (file, serverFileName) {
                    fileList = [];
                    i = 1;
                    var abc = $.map(serverFileName, function (item) { return (item); });
                    $.each(abc, function (index, value) {
                        fileList[i] = { "fileName": value, "fileId": i++ };
                    });
                    isFilesUploading = false;
                });
                if (images) {
                    for (i = 0; i < images.length; i++) {
                        myDropzone.emit("addedfile", images[i]);
                        myDropzone.emit("thumbnail", images[i], "/Images/Ads/");
                        myDropzone.emit("complete", images[i]);
                    }
                }
            }
        };

        $.extend(options, value);

        $(element).addClass('dropzone');
        new Dropzone(element, options);
    }
};