S.head = {
    init: function () {
        $('.boards-menu .btn-new-board').on('click', S.boards.add.show);
        $('.boards-menu .btn-always-show').on('click', S.head.boards.alwaysShow);
        $('.boards-menu .btn-all-color').on('click', S.head.allColor);
        $('.boards-menu .btn-import-trello').on('click', S.head.import.trello.show);
        $('.btn-boards').on('click', S.head.boards.show);
        $('.bg-for-boards-menu').on('click', S.head.boards.hide);
        S.scrollbar.add($('.boards-menu'), {footer: S.head.boards.scroll.footer});
    },

    boards: {
        boardsMenuBg: $('.bg-for-boards-menu'),

        show: function () {
            if (!$('.boards-menu').hasClass('always-show')) {
                $('.boards-menu').removeClass('hide').addClass('show');
                $('.bg-for-boards-menu').removeClass('hide');
                $(window).on('resize', S.head.boards.resize);
                $(window).on('scroll', S.head.boards.resize);
                S.head.boards.callback.execute(true, false);
                S.head.boards.resize();
                S.scrollbar.update($('.boards-menu'));
            }
        },

        resize: function () {
            //resizes bg for board menu to fit window
            const bg = S.head.boards.boardsMenuBg;
            const win = S.window.pos();
            const menu = $('.boards-menu');
            const movable = $('.boards-menu .movable');
            const h = movable.height();
            bg.css({ width: win.w, height: win.h + win.scrolly });
            if (menu.hasClass('always-show')) {
                if (h > win.h - 44) {
                    menu[0].style.maxHeight = (win.h - 44) + 'px';
                } else {
                    menu.css({ maxHeight: 'calc(100% - 44px)' });
                }
            } else {
                menu.css({ maxHeight: '' });
            }
        },

        scroll: {
            footer: function () {
                const win = S.window.pos();
                const listitems = $('.boards-menu');
                const h = listitems.height();
                const pos = listitems[0].getBoundingClientRect();
                return win.h - pos.top - h;
            }
        },

        hide: function () {
            $('.boards-menu').removeClass('show').addClass('hide');
            $('.bg-for-boards-menu').addClass('hide');
            $(window).off('resize', S.head.boards.resize);
            $(window).off('scroll', S.head.boards.resize);
            S.head.boards.callback.execute(false, false);
        },

        alwaysShow: function (init) {
            $('header .btn-boards').hide();
            $('.boards-menu').addClass('always-show');
            $('.boards-menu .scroller').addClass('no-scroll');
            $('.btn-always-show')
                .html('Don\'t keep this menu open')
                .off('click', S.head.boards.alwaysShow)
                .on('click', S.head.boards.cancelAlwaysShow);
            $('.body').css({ marginLeft: 250 });  
            $('.bg-for-boards-menu').addClass('hide');
            if (init !== true) { S.ajax.post('Boards/KeepMenuOpen', { keepOpen: true }); }
            S.head.boards.callback.execute(true, true);
            S.head.boards.resize();
            S.scrollbar.update($('.boards-menu'));
        },

        cancelAlwaysShow: function () {
            $('header .btn-boards').show();
            $('.boards-menu .scroller').removeClass('no-scroll');
            $('.boards-menu').removeClass('always-show');
            $('.btn-always-show')
                .html('Always keep this menu open')
                .off('click', S.head.boards.cancelAlwaysShow)
                .on('click', S.head.boards.alwaysShow);
            $('.body').css({ marginLeft: '' });
            $('.bg-for-boards-menu').removeClass('hide');
            S.ajax.post('Boards/KeepMenuOpen', { keepOpen: false });
            S.head.boards.callback.execute(true, false);
            S.head.boards.resize();
            S.scrollbar.update($('.boards-menu'));
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

    allColor: function () {
        var all = true;
        if ($('.board').hasClass('all-color')) {
            all = false;
            $('.btn-all-color').html('Colorize Lists');
            $('.board').removeClass('all-color');
        } else {
            $('.btn-all-color').html('Grey Lists');
            $('.board').addClass('all-color');
            S.util.css.load('/css/pages/board/all-color.css', 'css_allcolor');
        }
        S.ajax.post('Boards/AllColor', { allColor: all });
    },

    import: {
        trello: {
            file:'',
            show: function () {
                S.popup.show('Import from Trello', $('#template_import_trello').html(), { width: 400 });
                $('.popup .btn-upload').on('click', S.head.import.trello.submit);
            },

            submit: function () {
                var iframe = S.iframe('.popup iframe');
                iframe.document.getElementsByTagName('form')[0].submit();
                $('.popup .btn-upload').hide();
            },

            uploaded: function () {
                location.reload();
            }
        }
    }
}

S.head.init();