S.ajax = {
    //class used to make simple web service posts to the server
    expire: new Date(), queue: [],

    post: function (url, data, callback, error, json) {
        this.expire = new Date();
        S.events.ajax.start();

        var options = {
            method: "POST",
            data: JSON.stringify(data),
            url: '/api/' + url,
            contentType: "text/plain; charset=utf-8",
            dataType:'html',
            success: function (d) {
                var txt = '';
                if (typeof d.responseText != 'undefined') { txt = d.responseText; } else { txt = d;}
                S.ajax.runQueue(); S.events.ajax.complete(txt); callback(txt);
            },
            error: function (xhr, status, err) {
                S.events.ajax.error(status, err);
                if (typeof error == 'function') { error(xhr, status, err); }
                S.ajax.runQueue();
            }
        }
        if (json == true) { options.dataType = 'json'; }
        S.ajax.queue.push(options);
        if (S.ajax.queue.length == 1) {
            $.ajax(options);
        }
    },

    inject: function (element, data) {
        var elem = $(element);
        if (elem.length > 0 && data.d.html != '') {
            switch (data.d.inject) {
                case 0: //replace
                    elem.html(data.d.html);
                    break;
                case 1: //append
                    elem.append(data.d.html);
                    break;
                case 2: //before
                    elem.before(data.d.html);
                    break;
                case 3: //after
                    elem.after(data.d.html);
                    break;
            }
        }

        //add any CSS to the page
        if (data.d.css && data.d.css != '') {
            S.util.css.add(data.d.cssid, data.d.css);
        }

        //finally, execute callback javascript
        if (data.d.javascript && data.d.javascript != '') {
            var js = new Function(data.d.javascript);
            js();
        }
    },

    runQueue: function () {
        S.ajax.queue.shift();
        if (S.ajax.queue.length > 0) {
            $.ajax(S.ajax.queue[0]);
        }
    }
};