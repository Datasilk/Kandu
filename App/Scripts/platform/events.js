S.events = {

    doc: {
        load: function () {
        },

        ready: function () {
            S.events.doc.resize.trigger();
        },

        mousedown: {
            trigger: function (target) {
 
            }
        },

        click: {
            type: '',

            trigger: function (target) {
                this.callback.execute(target, this.type);
                return this.type;
            },

            callback: {
                //register & execute callbacks when the user clicks anywhere on the document
                items: [],

                add: function (elem, onClick) {
                    this.items.push({ elem: elem, onClick: onClick });
                },

                remove: function (elem) {
                    for (var x = 0; x < this.items.length; x++) {
                        if (this.items[x].elem == elem) { this.items.splice(x, 1); x--; }
                    }
                },

                execute: function (target, type) {
                    if (this.items.length > 0) {
                        for (var x = 0; x < this.items.length; x++) {
                            if (typeof this.items[x].onClick == 'function') {
                                this.items[x].onClick(target, type);
                            }
                        }
                    }
                }
            }
        },

        scroll: {
            ticking: false,
            last: { scrollx: 0, scrolly: 0 },

            trigger: function () {
                if (!S.events.doc.scroll.ticking) {
                    S.events.doc.scroll.last.scrollx = window.scrollX;
                    S.events.doc.scroll.last.scrolly = window.scrollY;
                    S.window.scrollx = S.events.doc.scroll.last.scrollx;
                    S.window.scrolly = S.events.doc.scroll.last.scrolly;
                    S.events.doc.scroll.callback.execute('onGo');
                    S.events.doc.scroll.ticking = true;
                    requestAnimationFrame(S.events.doc.scroll.animate);
                }
            },

            start: function () {
                this.ticking = false;
                this.callback.execute('onStart');
            },

            animate: function () {
                var self = S.events.doc.scroll;
                S.events.doc.scroll.ticking = false;
                self.callback.execute('onAnimate');
            },

            stop: function () {
                S.events.doc.scroll.last.scrollx = window.scrollX;
                S.events.doc.scroll.last.scrolly = window.scrollY;
                S.window.scrollx = S.events.doc.scroll.last.scrollx;
                S.window.scrolly = S.events.doc.scroll.last.scrolly;
                S.events.doc.scroll.callback.execute('onStop');
            },

            callback: {
                //register & execute callbacks when the window resizes
                items: [],

                add: function (elem, onStart, onGo, onAnimate, onStop) {
                    this.items.push({ elem: elem, onStart: onStart, onGo: onGo, onAnimate: onAnimate, onStop: onStop });
                },

                remove: function (elem) {
                    for (var x = 0; x < this.items.length; x++) {
                        if (this.items[x].elem == elem) { this.items.splice(x, 1); x--; }
                    }
                },

                execute: function (type) {
                    if (this.items.length > 0) {
                        switch (type) {
                            case '': case null: case 'onStart':
                                for (var x = 0; x < this.items.length; x++) {
                                    if (typeof this.items[x].onStart == 'function') {
                                        this.items[x].onStart();
                                    }
                                } break;

                            case 'onGo':
                                //first, go (calculate stuff)
                                for (var x = 0; x < this.items.length; x++) {
                                    if (typeof this.items[x].onGo == 'function') {
                                        this.items[x].onGo();
                                    }
                                }
                                break;

                            case 'onAnimate':
                                for (var x = 0; x < this.items.length; x++) {
                                    if (typeof this.items[x].onAnimate == 'function') {
                                        this.items[x].onAnimate();
                                    }
                                } break;

                            case 'onStop':
                                for (var x = 0; x < this.items.length; x++) {
                                    if (typeof this.items[x].onStop == 'function') {
                                        this.items[x].onStop();
                                    }
                                } break;

                        }
                    }
                }
            }


        },

        resize: {
            timer: { started: false, fps: 60, timeout: 250, date: new Date(), callback: null },

            trigger: function () {
                this.timer.date = new Date();
                if (this.timer.started == false) { this.start(); S.window.changed = true; S.window.pos(); }
            },

            start: function () {
                if (this.timer.started == true) { return; }
                clearInterval(this.timer.callback);
                this.timer.date = new Date();
                this.timer.started = true;
                this.callback.execute('onStart');
                this.timer.callback = setInterval(function () { S.events.doc.resize.go(); }, 1000 / this.timer.fps);
                this.go();
            },

            go: function () {
                if (this.timer.started == false) { return; }
                S.window.changed = true; S.window.pos();
                this.callback.execute('onGo');
                if (new Date() - this.timer.date > this.timer.timeout) {
                    this.stop();
                    return;
                }
            },

            stop: function () {
                if (this.timer.started == false) { return; }
                clearInterval(this.timer.callback);
                this.timer.started = false;
                this.callback.execute('onStop');
            },

            callback: {
                //register & execute callbacks when the window resizes
                items: [],

                add: function (elem, onStart, onGo, onStop) {
                    this.items.push({ elem: elem, onStart: onStart, onGo: onGo, onStop: onStop });
                },

                remove: function (elem) {
                    for (var x = 0; x < this.items.length; x++) {
                        if (this.items[x].elem == elem) { this.items.splice(x, 1); x--; }
                    }
                },

                execute: function (type, lvl) {
                    if (this.items.length > 0) {
                        switch (type) {
                            case 'onStart':
                                for (var x = 0; x < this.items.length; x++) {
                                    if (this.items[x].onStart) {this.items[x].onStart();}
                                } break;

                            case 'onGo':
                                for (var x = 0; x < this.items.length; x++) {
                                    if (this.items[x].onGo) {this.items[x].onGo();}
                                } break;

                            case 'onStop':
                                for (var x = 0; x < this.items.length; x++) {
                                    if (this.items[x].onStop) {this.items[x].onStop();}
                                } break;
                        }
                    }
                }
            }
        }
    },

    iframe: {
        loaded: function () {

        }
    },

    ajax: {
        //register & execute callbacks when ajax makes a post
        loaded: true,

        start: function () {
            this.loaded = false;
            $(document.body).addClass('wait');

        },

        complete: function () {
            S.events.ajax.loaded = true;
            $(document.body).removeClass('wait');
            S.window.changed = true;
        },

        error: function (status, err) {
            S.events.ajax.loaded = true;
            $(document.body).removeClass('wait');
        },

        callback: {
            items: [],

            add: function (elem, onStart, onComplete, onError) {
                this.items.push({ elem: elem, onStart: onStart, onComplete: onComplete, onError: onError });
            },

            remove: function (elem) {
                for (var x = 0; x < this.items.length; x++) {
                    if (this.items[x].elem == elem) { this.items.splice(x, 1); x--; }
                }
            },

            execute: function (type) {
                if (this.items.length > 0) {
                    switch (type) {
                        case '': case null: case 'onStart':
                            for (var x = 0; x < this.items.length; x++) {
                                if (typeof this.items[x].onStart == 'function') {
                                    this.items[x].onStart();
                                }
                            } break;

                        case 'onComplete':
                            for (var x = 0; x < this.items.length; x++) {
                                if (typeof this.items[x].onComplete == 'function') {
                                    this.items[x].onComplete();
                                }
                            } break;

                        case 'onError':
                            for (var x = 0; x < this.items.length; x++) {
                                if (typeof this.items[x].onError == 'function') {
                                    this.items[x].onError();
                                }
                            } break;

                    }
                }
            }
        }
    },

    url: {
        change: function (e) {
            if (typeof e.state == 'string') {
                if (S.events.url.callback.execute(e) == false) { return false; }
                S.url.load(e.state, 1);
            }
        },

        //register & execute callbacks when the url changes
        callback: {
            items: [],

            add: function (elem, onCallback) {
                this.items.push({ elem: elem, onCallback: onCallback });
            },

            remove: function (elem) {
                for (var x = 0; x < this.items.length; x++) {
                    if (this.items[x].elem == elem) { this.items.splice(x, 1); x--; }
                }
            },

            execute: function (e) {
                if (this.items.length > 0) {
                    for (var x = 0; x < this.items.length; x++) {
                        if (typeof this.items[x].onCallback == 'function') {
                            if (this.items[x].onCallback(e) == false) { return false; }
                        }
                    }
                }
                return true;
            }
        }
    }
};