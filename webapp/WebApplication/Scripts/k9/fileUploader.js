function fileUploader(config) {
    function getFilesContainer(el) {
        return $(el).closest("form").find(".upload-file-preview");
    }

    function deleteFilePreview(el, id) {
        var container = getFilesContainer(el);
        var preview = container.find("div.file-preview-container[data-file-id=" + id + "]");
        if (preview) {
            preview.fadeOut(50, function () {
                preview.remove();
            });
        }
    }

    function removeNewFiles(el) {
        var container = getFilesContainer(el);
        container.find("div.file-preview-new").remove();
    }

    function deleteUploadedFile(el, id) {
        var container = getFilesContainer(el);
        var uploadedFile = container.find("input.uploaded-file-deleted[data-file-id=" + id + "]");
        uploadedFile.val(true);
    }

    function getUploadedFileWithSameName(el, fileName) {
        var container = getFilesContainer(el);
        var htmlEncodedFileName = $.fn.htmlEncode(fileName);
        var uploadedFile = container.find("input.uploaded-file[value='" + htmlEncodedFileName + "']");
        var isDeleted = container.find("input.uploaded-file-deleted[data-file-name='" + htmlEncodedFileName + "'][value=true]").length;
        return uploadedFile.length && !isDeleted ? uploadedFile : false;
    }

    function bindButtonEvents() {
        $("button.file-preview-delete").click(function () {
            var el = $(this);
            var id = el.attr("data-file-id");

            deleteFilePreview(el, id);
            deleteUploadedFile(el, id);
        });
    }

    function displayFile(input, file, type, data) {
        var filesContainer = getFilesContainer(input);
    
        var fileContainerDiv = $(document.createElement("DIV"));
        fileContainerDiv.attr("class", "file-preview-container col-lg-3 col-md-4 col-sm-6 col-xs-12 file-preview-new");

        var fileId = $.fn.createGuid();
        fileContainerDiv.attr("data-file-id", fileId);

        var fileThumbnailContainerDiv = $(document.createElement("DIV"));
        fileThumbnailContainerDiv.attr("class", "file-thumbnail-container");

        var fileContainer = $(document.createElement("DIV"));
        fileContainer.attr("class", "preview-thumbnail");

        var docInfo = $(document.createElement("DIV"));
        docInfo.attr("class", "doc-info");
        docInfo.html("<p>" + file.name + "</p>" +
            "<samp>(" + $.fn.formatBytes(file.size) + ")</samp>" +
            '<i class="glyphicon glyphicon-upload file-preview-upload"></i>');

        if (type === "image") {
            var img = $(document.createElement("IMG"));
            if (data.height > data.width) {
                img.attr("class", "portrait");
            }
            img.attr("src", data.src);

            fileContainer.append(img);
            fileThumbnailContainerDiv.append(fileContainer, docInfo);
        } else {
            var documentDiv = $(document.createElement("DIV"));
            documentDiv.attr("class", "preview-document fa fa-file-o");
            fileContainer.append(documentDiv);
            fileThumbnailContainerDiv.append(fileContainer, docInfo);
        }
        
        fileContainerDiv.append(fileThumbnailContainerDiv);
        filesContainer.append(fileContainerDiv);

        bindButtonEvents();
    }

    function loadFile(input, f, fileSrc, index) {
        var uploadedFileWithSameName = getUploadedFileWithSameName(input, f.name);
        if(uploadedFileWithSameName) {
            var fileId = uploadedFileWithSameName.attr("data-file-id");
            deleteUploadedFile(input, fileId);
            deleteFilePreview(input, fileId);
        }

        if ($.fn.isImage(f.name)) {
            var image = new Image;
            image.onload = function() {
                displayFile(input, f, "image", image);
            };
            image.src = fileSrc;
        } else {
            displayFile(input, f, "file");
        }
    }

    function loadFiles(input) {
        var container = getFilesContainer(input);
        if (input.files) {
            removeNewFiles(container);
            var fileCount = input.files.length;
            for (var i = 0; i < fileCount; i++) {
                var file = input.files[i];
                if (file) {
                    var reader = new FileReader();
                    container.fadeIn();

                    reader.onload = (function (inp, f, index) {
                        return function (e) {
                            loadFile(inp, f, e.target.result, index);
                        };
                    })(input, file, i);

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
            var input = $(this);
            getFilesContainer(input).show();
            loadFiles(this);
        });
        initFileInputs();
        bindButtonEvents();
    }

    return {
        init: init
    };

}