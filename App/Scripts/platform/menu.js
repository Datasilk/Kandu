S.menu = {
    click: function (e) {
        var sub = this.parentNode.parentNode.querySelector('ul.menu');
        if (sub) {
            $(sub).toggleClass('expanded');
        }
        e.preventDefault();
        return false;
    },

    select: function(selector){
        $(selector + ' li > .row.hover.selected').removeClass('selected');
        //find the correct menu item to select
        var items = $(selector + ' li > .row.hover');
        for (var x = 0; x < items.length; x++) {
            var e = items.get(x);
            var a = $(e).find('a');
            if (a.length > 0) {
                if (window.location.href.indexOf(a.get().href) >= 0) {
                    $(e).addClass('selected');
                    break;
                }
            }
        }
    },

    addListener: function (name, selector) {
        //listen for menu clicks
        $(selector + ' li a').on('click', S.menu.click);

        //listen for url changes
        S.events.url.callback.add(name, null, function () {
            S.menu.select(selector);
        });
    }
} 