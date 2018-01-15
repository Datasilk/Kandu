S.head = {
    init: function () {
        $('.boards-menu .btn-new-board').on('click', S.boards.add.show);
        $('.btn-boards').on('click', S.head.boards.show);
        $('.bg-for-boards-menu').on('click', S.head.boards.hide);
        $('.btn-always-show').on('click', S.head.boards.alwaysShow);
    },

    boards: {
        show: function () {
            $('.boards-menu').removeClass('hide').addClass('show');
            $('.bg-for-boards-menu').removeClass('hide');
            $(window).on('resize', S.head.boards.resize);
            S.head.boards.resize();
        },

        resize: function () {
            //resizes bg for board menu to fit window
            var bg = $('.bg-for-boards-menu');
            var win = S.window.pos();
            bg.css({ width: win.w, height: win.h});
        },

        hide: function () {
            $('.boards-menu').removeClass('show').addClass('hide');
            $('.bg-for-boards-menu').addClass('hide');
            $(window).off('resize', S.head.boards.resize);
        },

        alwaysShow: function () {
            $('.boards-menu').addClass('always-show');
            $('.btn-always-show')
                .html('Don\'t keep this menu open')
                .off('click', S.head.boards.alwaysShow)
                .on('click', S.head.boards.cancelAlwaysShow);
            $('.body').css({ marginLeft: 250 });
            $('.bg-for-boards-menu').hide();
        },

        cancelAlwaysShow: function () {
            $('.boards-menu').removeClass('always-show');
            $('.btn-always-show')
                .html('Always keep this menu open')
                .off('click', S.head.boards.cancelAlwaysShow)
                .on('click', S.head.boards.alwaysShow);
            $('.body').css({ marginLeft: '' });
            $('.bg-for-boards-menu').css({ 'display': '' });
        }
    }
}

S.head.init();