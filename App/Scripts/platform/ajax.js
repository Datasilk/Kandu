S.ajax = {
    //class used to make simple web service posts to the server
    expire: new Date(), queue: [],

    post: function (url, data, callback, error) {
        this.expire = new Date();
        S.events.ajax.start();
        var d = data;

        var options = {
            method: "POST",
            data: JSON.stringify(d),
            url: '/api/' + url,
            contentType: "text/plain; charset=utf-8",
            success: function (d) { S.ajax.runQueue(); S.events.ajax.complete(d); callback(d); },
            error: function (xhr, status, err) {
                S.events.ajax.error(status, err);
                if (typeof error == 'function') { error(); }
                S.ajax.runQueue();
            }
        }
        S.ajax.queue.push(options);
        if (S.ajax.queue.length == 1) {
            $.ajax(options);
        }
    },

    runQueue: function () {
        S.ajax.queue.shift();
        if (S.ajax.queue.length > 0) {
            $.ajax(S.ajax.queue[0]);
        }
    }
};