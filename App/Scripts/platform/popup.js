S.popup = {
    elem: null, options: null,

    show: function (title, html, options) {
        if (options == null) { options = {}; }
        var opts = {
            width: options.width != null ? options.width : 300,
            padding: options.padding != null ? options.padding : 0,
            offsetHeight: options.offsetHeight != null ? options.offsetHeight : 0,
            offsetTop: options.offsetTop != null ? options.offsetTop : 0,
            position: options.position != null ? options.position : 'center',
            close: options.close != null ? options.close : true,
            className: options.className != null ? options.className : ''
        };
        this.options = opts;

        var win = S.window.pos();
        var div = document.createElement('div');
        var forpopup = $('body > .for-popup');
        var popup = $(div);
        div.className = 'popup box ' + opts.className;

        popup.css({ width: opts.width });
        popup.addClass(opts.position);
        if (opts.offsetHeight > 0) {
            popup.css({ Marginbottom: opts.offsetHeight });
        }
        if (opts.offsetTop.toString().indexOf('%') > 0) {
            popup.css({ top: opts.offsetTop });
        } else if (Number(opts.offsetTop) == opts.offsetTop) {
            if (opts.offsetTop > 0) {
                popup.css({ top: win.scrolly + ((win.h - 300) / 3) + opts.offsetTop });
            }
        }
        if (opts.padding > 0) {
            forpopup.css({ padding: opts.padding });
        }

        var htm = '<div class="row">';

        if (title != '') {
            htm += '<div class="col pad-sm"><h4>' + title + '</h4></div>';
        }
        if (opts.close == true) {
            //add close button to top of page
            htm += $('#template_popup_close').html();
        }
        htm += '</div>';

        popup.html(htm + html);
        this.elem = popup;

        $('body > .for-popup .popup').remove();
        forpopup.removeClass('hide').append(div);

        //set up events
        S.events.doc.resize.callback.add('popup', S.popup.resize, S.popup.resize, S.popup.resize);
        S.events.doc.scroll.callback.add('popup', S.popup.resize, S.popup.resize, S.popup.resize);

        if (opts.close == true) {
            $('.popup .btn-close a').on('click', function () {
                S.popup.hide();
            });
        }

        S.popup.resize();
    },

    hide: function(){
        //remove events
        $('body > .for-popup').addClass('hide');
        S.events.doc.resize.callback.remove('popup');
    },

    bg: function (e) {
        if (e.target == $('.bg.for-popup')[0]) { S.popup.hide();}
    },

    resize: function () {
        var win = S.window.pos();
        var pos = S.popup.elem.position();
        pos.height = S.popup.elem.height();
        S.popup.elem.css({ maxHeight: win.height - (S.popup.options.padding * 2), top: S.popup.options.offsetTop.toString().indexOf('%') > 0 ? S.popup.options.offsetTop : win.scrolly + ((win.h - pos.height) / 3) + S.popup.options.offsetTop });
    }
}