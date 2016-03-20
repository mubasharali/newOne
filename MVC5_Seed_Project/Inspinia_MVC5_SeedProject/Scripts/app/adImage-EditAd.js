function adImages() {
    var adId = $("#adId").val();
    function Images(data, i) {
        var self = this;
        data = data || {};
        self.name = adId + "_" + i + data.extension;
        self.size = data.size;
    }
    var images = [];
    var myDropzone1;
    var loadImages = function () {
        $.ajax({
            url: '/api/Electronic/getAdImages?id=' + adId,
            dataType: "json",
            contentType: "application/json",
            cache: false,
            type: 'GET',
            success: function (data) {
                var i = 1;
                var mappedads = $.map(data, function (item) { return new Images(item, i++); });
                images = mappedads;

                if (images) {
                    for (i = 0; i < images.length; i++) {
                        myDropzone1.options.addedfile.call(myDropzone1, images[i]);
                        myDropzone1.options.thumbnail.call(myDropzone1, images[i], "/Images/Ads/" + images[i].name);
                    }
                }
            },
            error: function (jqXHR, status, thrownError) {
                toastr.error("failed to load images.Please refresh page", "Error");
            }
        });
    }

    loadImages();
    var fileList = [];
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
                    this.on("success", function (file, serverFileName) {
                        fileList = [];
                        i = 1;
                        var abc = $.map(serverFileName, function (item) { return (item); });
                        $.each(abc, function (index, value) {
                            fileList[i] = { "fileName": value, "fileId": i++ };
                        })
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
}

