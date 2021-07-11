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