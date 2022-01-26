//utilities for Kandu dashboard
S.util.url = {
    hash: {
        params: function () {
            var hash = window.location.hash;
            if (hash != '') { hash = hash.replace('#', ''); }
            var arr = [];
            var arrhash = hash.split('&');
            for (var x = 0; x < arrhash.length; x++) {
                var kv = arrhash[x].split('=', 2);
                if (kv.length == 2) {
                    arr.push({ key: kv[0], value: kv[1] });
                }
            }
            return arr;
        }
    }
};

S.util.message = function (elem, type, msg) {
    if (!elem || elem == '') { elem = '.messages:nth-child(1)'; }
    var container = $(elem);
    var div = document.createElement('div');
    div.className = 'message' + (type != null ? ' ' + type : '');
    div.innerHTML = template_message.innerHTML.replace('##text##', msg);
    container.removeClass('hide').append(div);
    $(div).find('.close-btn').on('click', (e) => {
        $(div).remove();
        if (container.children().length == 0) {
            container.addClass('hide');
        }
    });
};