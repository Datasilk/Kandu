S.validate = {
    alphaNumeric: function (str, allowedChars) {
        if (str != null && str != '') {
            if (str.match(/^[a-zA-Z0-9]+$/)) { return true; }
            if (allowedChars) {
                if (Array.isArray(allowedChars)) {
                    var a = '';
                    for(var y = 0; y < str.length;y++){
                        a = str[y];
                        if (!a.match(/^[a-zA-Z0-9]+$/)) {
                            //check for allowed chars
                            var valid = false;
                            for (var x = 0; x < allowedChars.length; x++) {
                                if (a == allowedChars[x]) { valid = true; break; }
                            }
                            if (!valid) { return false; }
                        }
                    }
                    return true;
                }
            }
        }
        return false;
    },

    text: function (str, excludedChars) {
        if (str != null && str != '') {
            if (excludedChars) {
                if (Array.isArray(excludedChars)) {
                    excludedChars.forEach(function (a) {
                        if (str.indexOf(a) >= 0) { return false;}
                    });
                }
            }
            return true;
        }
        return false;
    }
}