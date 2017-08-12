function fileUploader(config)
{
    function loadFile(f, fileSrc, index) {
        var image = new Image;
        image.onload = function ()
        {
            var fileContainerDiv = $(document.createElement("DIV"));
            fileContainerDiv.attr("class", "file-preview-container col-lg-3 col-md-4 col-sm-6 col-xs-12");

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
											   '<button type="button" class="file-preview-delete btn btn-xs btn-default" title="' + config.deleteText + '" data-url="" data-index="' + index + '"><i class="glyphicon glyphicon-trash text-danger"></i></button>');

            fileContainer.append(img);
            fileThumbnailContainerDiv.append(fileContainer, imageInfo);
            fileContainerDiv.append(fileThumbnailContainerDiv);
            filesContainer.append(fileContainerDiv);
        };
        image.src = fileSrc;
    }

    function displayFiles(input)
    {
        if (input.files)
        {
            for (var i = 0; i < input.files.length; i++)
            {
                var file = input.files[i];
                if (file)
                {
                    var reader = new FileReader();
                    var filesContainer = $("div.upload-file-preview");
                    filesContainer.fadeIn();

                    reader.onload = (function (f)
                    {
                        return function (e) {
                            loadFile(f, e.target.result, i);
                        };
                    })(file);

                    reader.readAsDataURL(file);
                }
            }
        }
    }

    function init()
    {
        $("input.file-upload").change(function ()
        {
            displayFiles(this);
        });
    }

    return {
        init: init
    };

}