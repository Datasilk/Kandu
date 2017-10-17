S.scaffold = function (html, vars, tagStart, tagEnd) {
    //tagStart & tagEnd is optional, defines the symbols (#)
    //to use when searching for scaffold variable placeholders
    this.html = html;
    this.vars = vars;
    if (tagStart) {
        this.tagStart = tagStart;
    } else { this.tagStart = "#"; }
    if (tagEnd) {
        this.tagEnd = tagEnd;
    } else { this.tagEnd = "#"; }
}

S.scaffold.prototype.render = function () {
    var a = 0, b = 0, c = 0, d = 0;
    var tagslen = this.tagStart.length + this.tagEnd.length;
    var endlen = this.tagEnd.length;
    var htm = this.html;
    var ischanged = true;
    for (var key in this.vars) {
        ischanged = true;
        while (ischanged) {
            ischanged = false;
            //check for scaffold closing first
            a = htm.indexOf(this.tagStart + '/' + key + this.tagEnd);
            if (a >= 0) {
                //found a group of html to show or hide based on scaffold element boolean value
                b = a + tagslen + key.length + 1;
                c = htm.indexOf(this.tagStart + key);
                d = htm.indexOf(this.tagEnd, c + 1);
                if (c >= 0 && d > c) {
                    if (this.vars[key] === false) {
                        //hide group of html
                        htm = htm.substr(0, c) + htm.substr(b);
                        ischanged = true;
                    } else if (this.vars[key] === true) {
                        //show group of html
                        htm = htm.substr(0, c) + htm.substr(d + endlen, a - (d + endlen)) + htm.substr(b);
                        ischanged = true;
                    }
                    continue;
                }
            }
            //check for scaffold element to replace with a value
            if (ischanged == false) {
                if (htm.indexOf(this.tagStart + key + this.tagEnd) >= 0) {
                    htm = htm.replace(this.tagStart + key + this.tagEnd, this.vars[key]);
                    ischanged = true;
                }
            }
        }
    }
    return htm;
}