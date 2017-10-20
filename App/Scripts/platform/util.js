S.util = {
    js: {
        load: function (file, id, callback) {
            //add javascript file to DOM
            if (document.getElementById(id)) { if(callback){callback();}return false;}
            var head = document.getElementsByTagName('head')[0];
            var script = document.createElement('script');
            script.type = 'text/javascript';
            script.src = file;
            script.id = id;
            script.onload = callback;
            head.appendChild(script);
        }
    },
    css: {
        load: function (file, id) {
            //download CSS file and load onto the page
            if (document.getElementById(id)) { return false; }
            var head = document.getElementsByTagName('head')[0];
            var link = document.createElement('link');
            link.rel = 'stylesheet';
            link.type = 'text/css';
            link.src = file;
            link.id = id;
            head.appendChild(link);
        },

        add: function (id, css) {
            //add raw CSS to the page inside a style tag
            $('#' + id).remove();
            $('head').append('<style id="' + id + '" type="text/css">' + css + "</style>");
        },
    }
}

S.util.str = {
    isNumeric: function (str) {
        return !isNaN(parseFloat(str)) && isFinite(str);
    }
}

S.math = {
    intersect: function (a, b) {
        //checks to see if rect (a) intersects with rect (b)
        if (b.left < a.right && a.left < b.right && b.top < a.bottom){
            return a.top < b.bottom;
        }else{
            return false;
        }
    }
}

S.util.color = {
    rgbToHex: function (color) {
        function toHex(num) {
            return ('0' + parseInt(num).toString(16)).slice(-2);
        }
        var hex = '';
        if (color.indexOf('rgb') >= 0) {
            var c = color.replace('rgb', '').replace('a', '').replace('(', '').replace(')', '').replace(/\s/g, '').split(',');
            hex = "#" + toHex(c[0]) + toHex(c[1]) + toHex(c[2]);
        } else { hex = color; }
        return hex;
    }
}