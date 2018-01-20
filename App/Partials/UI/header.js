S.head = {
    init: function () {
        $('.boards-menu .btn-new-board').on('click', S.boards.add.show);
        $('.btn-boards').on('click', S.head.boards.show);
        $('.bg-for-boards-menu').on('click', S.head.boards.hide);
        $('.btn-always-show').on('click', S.head.boards.alwaysShow);
        $('.boards-menu .scroller').on('scroll', S.head.scroller.scrolling);
    },

    boards: {
        boardsMenuBg: $('.bg-for-boards-menu'),

        show: function () {
            $('.boards-menu').removeClass('hide').addClass('show');
            $('.bg-for-boards-menu').removeClass('hide');
            $(window).on('resize', S.head.boards.resize);
            $(window).on('scroll', S.head.boards.resize);
            S.head.boards.resize();
            S.head.boards.callback.execute(true, false);
        },

        resize: function () {
            //resizes bg for board menu to fit window
            var bg = S.head.boards.boardsMenuBg;
            var win = S.window.pos();
            var menu = $('.boards-menu');
            var scroller = $('.boards-menu .scroller');
            bg.css({ width: win.w, height: win.h + win.scrolly });
            if (menu.hasClass('always-show')) {
                if (scroller.height() > win.h - 44) {
                    menu[0].style.minHeight = scroller.height() + 'px';
                } else {
                    menu.css({ minHeight: 'calc(100% - 44px)' });
                }
            } else {
                menu.css({ minHeight: '' });
            }
        },

        hide: function () {
            $('.boards-menu').removeClass('show').addClass('hide');
            $('.bg-for-boards-menu').addClass('hide');
            $(window).off('resize', S.head.boards.resize);
            $(window).off('scroll', S.head.boards.resize);
            S.head.boards.callback.execute(false, false);
        },

        alwaysShow: function () {
            var bars = S.head.scroller.sectionbars;
            bars.removeClass('locked').css({ top: '' });
            $('.boards-menu').addClass('always-show');
            $('.boards-menu .scroller').addClass('no-scroll');
            $('.btn-always-show')
                .html('Don\'t keep this menu open')
                .off('click', S.head.boards.alwaysShow)
                .on('click', S.head.boards.cancelAlwaysShow);
            $('.body').css({ marginLeft: 250 });  
            $('.bg-for-boards-menu').addClass('hide');
            S.head.boards.callback.execute(true, true);

        },

        cancelAlwaysShow: function () {
            $('.boards-menu .scroller').removeClass('no-scroll');
            $('.boards-menu').removeClass('always-show');
            $('.btn-always-show')
                .html('Always keep this menu open')
                .off('click', S.head.boards.cancelAlwaysShow)
                .on('click', S.head.boards.alwaysShow);
            $('.body').css({ marginLeft: '' });
            $('.bg-for-boards-menu').removeClass('hide');
            S.head.boards.callback.execute(true, false);
        },

        callback: {
            //register & execute callbacks when the user clicks anywhere on the document
            items: [],

            add: function (elem, onShow) {
                this.items.push({ elem: elem, onShow: onShow });
            },

            remove: function (elem) {
                for (var x = 0; x < this.items.length; x++) {
                    if (this.items[x].elem == elem) { this.items.splice(x, 1); x--; }
                }
            },

            execute: function (shown, alwaysShown) {
                if (this.items.length > 0) {
                    for (var x = 0; x < this.items.length; x++) {
                        if (typeof this.items[x].onShow == 'function') {
                            this.items[x].onShow(shown, alwaysShown);
                        }
                    }
                }
            }
        }
    },

    scroller: {
        sectionbars: $('.boards-menu .section-bar'),
        scroller: $('.boards-menu .scroller'),

        scrolling: function () {
            var bars = S.head.scroller.sectionbars;
            var scrollerPos = S.head.scroller.scroller.offset();
            var itemPos = S.head.scroller.scroller.find('.items').offset();
            var offsetTop = (itemPos.top - scrollerPos.top) * -1;
            var above = -1;
            for (var x = 0; x < bars.length; x++) {
                var bar = $(bars[x]);
                var pos = bar.offset();
                pos.top = pos.top - scrollerPos.top;
                if (pos.top <= 0 || bar.position().top > 0) {
                    above = x;
                } else { break; }
            }
            if (above >= 0) {
                var bar = $(bars[above]);
                var barTop = bar.parent().position().top;
                var pos = bar.position();
                var h = bar.parent().height() - bar.parent().position().top;
                console.log([bar.parent().position().top, offsetTop, h]);
                if (offsetTop - barTop <= h) {
                    if (!bar.hasClass('locked')) {
                        bars.removeClass('locked');
                        bar.addClass('locked');
                    }
                    bar.css({ top: offsetTop - barTop });
                }
            }
        }
    }
}

S.head.init();