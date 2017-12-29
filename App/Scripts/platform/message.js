S.message = {
    show: function(element, type, msg, fadein) {
        var types = 'error warning alert';
        var el = $(element);
        if (type != '' && type != null) {
            el.removeClass(types).addClass(type);
        } else {
            el.removeClass(types);
        }
        el.find('span').html(msg);
        if (fadein !== false) {
            el.css({ opacity: 0, overflow:'hidden' }).show();
            var h = el.height();
            el.css({ height: 0, marginTop: 10, marginBottom: 10, paddingTop:0, paddingBottom:0 });
            el.animate({ opacity: 1, height: h + 7 + 7, marginTop: 10, marginBottom: 10, paddingTop: 7, paddingBottom: 7 },
                { duration: 333 }); //, easing: 'easeInSine' });
        } else {
            el.css({ opacity: 1, height:'auto' }).show();
        }
        
    },

    error: {
        generic:'An error has occurred. Please contact support.'
    }
}