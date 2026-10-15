function ajaxcontroller() {

    function send(url, data, method) {
        var deferred = $.Deferred();

        $.ajax({
            type: method === null ? "GET" : method,
            url: url,
            dataType: "json",
            data: data,
            success: function (data) {
                deferred.resolve({
                    success: true,
                    data
                });
            },
            error: function (response) {
                var responseObject = jQuery.parseJSON(response.responseText);
                deferred.resolve({
                    success: false,
                    message: responseObject.message,
                    stacktrace: responseObject.StackTrace
                });
            }
        });

        return deferred.promise();
    }

    function get(url, data) {
        return send(url, data, "GET");
    }

    function post(url, data) {
        return send(url, data, "POST");
    }

    return {
        get: get,
        send: send,
        post: post
    };

}