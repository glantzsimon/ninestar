function fileUploader(config)
{
    var filesContainer = $("div.upload-file-preview");

    function deleteFilePreview(index)
    {
        var preview = $("div.file-preview-container[data-file-index=" + index + "]");
        preview.fadeOut(function() {

            preview.remove();
        });
    }

    function bindButtonEvents()
    {
        $("button.file-preview-delete").click(function ()
        {
            var index = $(this).attr("data-file-index");
            deleteFilePreview(index);
        });
    }

    function loadFile(f, fileSrc, index, total)
    {
        var image = new Image;
        image.onload = function ()
        {
            var fileContainerDiv = $(document.createElement("DIV"));
            fileContainerDiv.attr("class", "file-preview-container col-lg-3 col-md-4 col-sm-6 col-xs-12");
            fileContainerDiv.Attr("data-file-index", index);

            var fileThumbnailContainerDiv = $(document.createElement("DIV"));
            fileThumbnailContainerDiv.attr("class", "file-thumbnail-container");

            var fileContainer = $(document.createElement("DIV"));
            fileContainer.attr("class", "preview-thumbnail");

            var img = $(document.createElement("IMG"));
            if (image.height > image.width)
            {
                img.attr("class", "portrait");
            }
            img.attr("src", image.src);

            var imageInfo = $(document.createElement("DIV"));
            imageInfo.attr("class", "image-info");
            imageInfo.html("<p>" + f.name + "</p>" +
											   "<samp>(" + $.fn.formatBytes(f.size) + ")</samp>" +
											   '<button type="button" class="file-preview-delete btn btn-xs btn-default" title="' + config.deleteText + '" data-file-index="' + index + '"><i class="glyphicon glyphicon-trash text-danger"></i></button>');

            fileContainer.append(img);
            fileThumbnailContainerDiv.append(fileContainer, imageInfo);
            fileContainerDiv.append(fileThumbnailContainerDiv);
            filesContainer.append(fileContainerDiv);

            if (index === total - 1)
            {
                bindButtonEvents();
            }
        };
        image.src = fileSrc;
    }

    function loadFiles(input)
    {
        if (input.files)
        {
            for (var i = 0; i < input.files.length; i++)
            {
                var file = input.files[i];
                if (file)
                {
                    var reader = new FileReader();
                    filesContainer.fadeIn();

                    reader.onload = (function (f)
                    {
                        return function (e)
                        {
                            loadFile(f, e.target.result, i, input.files.length);
                        };
                    })(file);

                    reader.readAsDataURL(file);
                }
            }
        }
    }

    function initFileInputs()
    {
        $(':file').on('fileselect', function (event, numFiles, fileName)
        {
            var fileDescription = numFiles > 1 ? numFiles + ' ' + config.filesSelectedText : fileName;
            var textInput = $(this).parents('.input-group').find(':text.file-label');

            if (textInput.length)
            {
                textInput.val(fileDescription);
            }
        });

        $(document).on('change', ':file', function ()
        {
            var input = $(this);
            var numFiles = input.get(0).files ? input.get(0).files.length : 1;
            var fileName = input.val().replace(/\\/g, '/').replace(/.*\//, '');
            input.trigger('fileselect', [numFiles, fileName]);
        });
    }

    function init()
    {
        $("input.file-upload").change(function ()
        {
            loadFiles(this);
        });
        initFileInputs();
        bindButtonEvents();
    }

    return {
        init: init
    };

}