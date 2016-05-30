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
                fileList = [];
                i = 1;
                this.on("addedfile", function (file) {
                    isFilesUploading = true;
                });
                this.on("success", function (file, serverFileName) {
                    isFilesUploading = false;
                    //var tempp = file.xhr.responseText;
                    //tempp = tempp.trim('"');
                    //if( tempp== "Error" ){
                    //    var node, _i, _len, _ref, _results;
                    //    var message = file.msg // modify it to your error message
                    //    file.previewElement.classList.add("dz-error");
                    //    _ref = file.previewElement.querySelectorAll("[data-dz-errormessage]");
                    //    _results = [];
                    //    for (_i = 0, _len = _ref.length; _i < _len; _i++) {
                    //        node = _ref[_i];
                    //        _results.push(node.textContent = message);
                    //    }
                    //    return _results;
                    //}
                    if (file.code == 403) {  //  error
                        // below is from the source code too
                        var node, _i, _len, _ref, _results;
                        var message = file.msg // modify it to your error message
                        file.previewElement.classList.add("dz-error");
                        _ref = file.previewElement.querySelectorAll("[data-dz-errormessage]");
                        _results = [];
                        for (_i = 0, _len = _ref.length; _i < _len; _i++) {
                            node = _ref[_i];
                            _results.push(node.textContent = message);
                        }
                        return _results;
                    }
                    var abc = $.map(serverFileName, function (item) { return (item); });
                    $.each(abc, function (index, value) {
                        fileList[i] = { "fileName": value, "fileId": i++ };
                    })
                });
                this.on("error", function (file, response) {
                    isFilesUploading = false;
                    // below is from the source code too
                    var node, _i, _len, _ref, _results;
                    var message = file.msg // modify it to your error message
                    file.previewElement.classList.add("dz-error");
                    _ref = file.previewElement.querySelectorAll("[data-dz-errormessage]");
                    _results = [];
                    for (_i = 0, _len = _ref.length; _i < _len; _i++) {
                        node = _ref[_i];
                        _results.push(node.textContent = message);
                    }
                    return _results;

                });

                //success: function(file, response){
                //    if(response.code == 501){ // succeeded
                //        return file.previewElement.classList.add("dz-success"); // from source
                //    }else if (response.code == 403){  //  error
                //        // below is from the source code too
                //        var node, _i, _len, _ref, _results;
                //        var message = response.msg // modify it to your error message
                //        file.previewElement.classList.add("dz-error");
                //        _ref = file.previewElement.querySelectorAll("[data-dz-errormessage]");
                //        _results = [];
                //        for (_i = 0, _len = _ref.length; _i < _len; _i++) {
                //            node = _ref[_i];
                //            _results.push(node.textContent = message);
                //        }
                //        return _results;
                //    }
                //}
            }
        };

        $.extend(options, value);

        $(element).addClass('dropzone');
        new Dropzone(element, options);
    }
};
$(function () {
    var w = Math.max(document.documentElement.clientWidth, window.innerWidth || 0);
    var h = Math.max(document.documentElement.clientHeight, window.innerHeight || 0);
    if (w < 786) {
        $("#dropzonePreview").removeClass("dz-message");
    }
});
