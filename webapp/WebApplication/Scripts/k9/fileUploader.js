function fileUploader(config) {
    var filesContainer = $("div.upload-file-preview");
    
    function deleteFilePreview(index) {
        var preview = $("div.file-preview-container[data-file-index=" + index + "]");
        if (preview) {
            preview.fadeOut(50, function () {
                preview.remove();
            });
        }
    }

    function removeNewFiles() {
        $("div.file-preview-new").remove();
    }

    function deleteUploadedFile(index) {
        var uploadedFile = $("input.uploaded-file[data-file-index=" + index + "]");
        uploadedFile.remove();
    }

    function uploadedFileExists(fileName) {
        var uploadedFiles = $(".uploaded-file");
        for (var i = 0; i < uploadedFiles.length; i++) {
            var uploadedFile = uploadedFiles[i];
            if (uploadedFile && $(uploadedFile).val() === fileName) {
                return true;
            }
        }
    }

    function bindButtonEvents() {
        $("button.file-preview-delete").click(function () {
            var el = $(this);
            var index = el.attr("data-file-index");

            deleteFilePreview(index);
            deleteUploadedFile(index);
        });
    }

    function loadFile(f, fileSrc, index, fileCount) {
        if (uploadedFileExists(f.name)) {
            return;
        }

        var image = new Image;
        var displayIndex = parseInt($(".uploaded-file-count").val()) + index;
        image.onload = function () {
            var fileContainerDiv = $(document.createElement("DIV"));
            fileContainerDiv.attr("class", "file-preview-container col-lg-3 col-md-4 col-sm-6 col-xs-12 file-preview-new");
            fileContainerDiv.attr("data-file-index", displayIndex);

            var fileThumbnailContainerDiv = $(document.createElement("DIV"));
            fileThumbnailContainerDiv.attr("class", "file-thumbnail-container");

            var fileContainer = $(document.createElement("DIV"));
            fileContainer.attr("class", "preview-thumbnail");

            var img = $(document.createElement("IMG"));
            if (image.height > image.width) {
                img.attr("class", "portrait");
            }
            img.attr("src", image.src);

            var imageInfo = $(document.createElement("DIV"));
            imageInfo.attr("class", "image-info");
            imageInfo.html("<p>" + f.name + "</p>" +
											   "<samp>(" + $.fn.formatBytes(f.size) + ")</samp>" +
											   '<i class="glyphicon glyphicon-upload file-preview-upload"></i>');

            fileContainer.append(img);
            fileThumbnailContainerDiv.append(fileContainer, imageInfo);
            fileContainerDiv.append(fileThumbnailContainerDiv);
            filesContainer.append(fileContainerDiv);

            bindButtonEvents();
        };
        image.src = fileSrc;
    }

    function loadFiles(input) {
        if (input.files) {
            removeNewFiles();
            var fileCount = input.files.length;
            for (var i = 0; i < fileCount; i++) {
                var file = input.files[i];
                if (file) {
                    var reader = new FileReader();
                    filesContainer.fadeIn();

                    reader.onload = (function (f, index, count) {
                        return function (e) {
                            loadFile(f, e.target.result, index, count);
                        };
                    })(file, i, fileCount);

                    reader.readAsDataURL(file);
                }
            }
        }
    }

    function initFileInputs() {
        $(':file').on('fileselect', function (event, numFiles, fileName) {
            var fileDescription = numFiles > 1 ? numFiles + ' ' + config.filesSelectedText : fileName;
            var textInput = $(this).parents('.input-group').find(':text.file-label');

            if (textInput.length) {
                textInput.val(fileDescription);
            }
        });

        $(document).on('change', ':file', function () {
            var input = $(this);
            var numFiles = input.get(0).files ? input.get(0).files.length : 1;
            var fileName = input.val().replace(/\\/g, '/').replace(/.*\//, '');
            input.trigger('fileselect', [numFiles, fileName]);
        });
    }

    function init() {
        $("input.file-upload").change(function () {
            filesContainer.show();
            loadFiles(this);
        });
        initFileInputs();
        bindButtonEvents();
    }

    return {
        init: init
    };

}