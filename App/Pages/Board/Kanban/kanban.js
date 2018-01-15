S.kanban = {
    init: function () {
        $('.btn-add-list').on('click', S.kanban.list.create.show);
        $('.kanban .lists .btn-close').on('click', S.kanban.list.create.cancel);
        $('.form-new-list > form').on('submit', function (e) {
            e.preventDefault(); S.kanban.list.create.submit(); return false;
        });
        //resize list height to fit window
        $(window).on('resize', S.kanban.list.resize);
        S.kanban.list.resize();
    },

    list: {
        create: {
            show: function () {
                $('.form-new-list').removeClass('hide').addClass('show');
            },

            cancel: function () {
                $('.form-new-list').removeClass('show').addClass('cancel');
                setTimeout(function () { $('.form-new-list').removeClass('cancel').addClass('hide');}, 500)
            },

            submit: function () {
                var data = {
                    boardId: S.board.id,
                    name: $('#newlist_name').val()
                };
                $('#newlist_name').val('');

                S.ajax.post('Lists/Create', data, function (d) {
                    if (d.indexOf('success') == 0) {
                        $('.lists .add-list').before(d.split('|')[1]);
                    }
                });
            }
        },

        resize: function () {
            var lists = $('.kanban .lists');
            var pos = lists.position();
            var win = S.window.pos();
            lists.css({ height: win.h - pos.top });
        }
    },

    card: {
        create: {
            show: function (listid) {
                var list = $('.list.id-' + listid);
                var form = list.find('.form-new-card');
                form.html($('#template_newcard').html());
                form.removeClass('hide').addClass('show');
                list.find('.btn-add-card').hide();
                list.find('.btn-close').on('click', function () { S.kanban.card.create.hide(listid); });
                list.find('form').on('submit', function (e) {
                    S.kanban.card.create.submit(listid);
                    e.preventDefault();
                    return false;
                });
            },

            hide: function (listid) {
                var list = $('.list.id-' + listid);
                list.find('.form-new-card').removeClass('show').addClass('cancel');
                setTimeout(function () {
                    list.find('.form-new-card').removeClass('cancel').addClass('hide').html('');
                    list.find('.btn-add-card').show();
                }, 310);
            },

            submit: function (listid) {
                var data = {
                    listId: listid,

                }
            }
        }
    }
};

S.kanban.init();