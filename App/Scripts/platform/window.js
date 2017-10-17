S.window = {
    w: 0, h: 0, scrollx: 0, scrolly: 0, z: 0, changed: true,

    pos: function (scrollOnly) {
        if (this.changed == false && !scrollOnly) { return this; }
        this.changed = false;
        var w = window;
        var e = document.documentElement;
        var b = document.body;

        //get window scroll x & y positions
        this.scrollx = w.scrollX;
        this.scrolly = w.scrollY;
        if (typeof this.scrollx == 'undefined') {
            this.scrollx = b.scrollLeft;
            this.scrolly = b.scrollTop;
            if (typeof this.scrollx == 'undefined') {
                this.scrollx = w.pageXOffset;
                this.scrolly = w.pageYOffset;
                if (typeof this.scrollx == 'undefined') {
                    this.z = GetZoomFactor();
                    this.scrollx = Math.round(e.scrollLeft / this.z);
                    this.scrolly = Math.round(e.scrollTop / this.z);
                }
            }
        }
        if (scrollOnly) { return this; } //no need to update width & height

        //get windows width & height
        this.w = w.innerWidth || e.clientWidth || b.clientWidth;
        this.h = w.innerHeight || e.clientHeight || b.clientHeight;
        return this;
    }
};