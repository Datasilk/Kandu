S.kanban = {
    init: function () {
        $('.btn-add-list').on('click', S.kanban.list.create.show);
        $('.kanban .lists .btn-close').on('click', S.kanban.list.create.cancel);
        $('.form-new-list > form').on('submit', function (e) {
            e.preventDefault(); S.kanban.list.create.submit(); return false;
        });
        //resize list height to fit window
        $(window).on('resize', S.kanban.list.resize);
        $(window).on('scroll', S.kanban.list.resize);
        S.kanban.list.resize();

        //init cards events
        S.kanban.cards.init();

        //add callback for header boards popup
        S.head.boards.callback.add('kanban', S.kanban.list.resize);

        //add drag capabilities to cards
        S.kanban.card.drag.init();
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
            var h = lists.children().first().height();
            lists.css({ height: win.h - pos.top + win.scrolly });
            if (h + pos.top >= win.h && (S.head.boards.boardsMenuBg.hasClass('hide') || (!S.head.boards.boardsMenuBg.hasClass('hide') && $('.boards-menu').hasClass('always-show')))) {
                lists.css({ marginBottom: h - win.h + pos.top - win.scrolly });
            } else {
                lists.css({ marginBottom: '' });
            }
            
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
                list.find('.new-card-label')[0].focus();
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
                var list = $('.list.id-' + listid);
                var text = list.find('.new-card-label');
                var data = {
                    boardId: S.board.id,
                    listId: listid,
                    name: text.val()
                };
                text.val('');
                S.ajax.post('Cards/Create', data,
                    function (d) {
                        if(d.indexOf('success|') == 0) {
                            var items = list.find('.items')
                            items.append(d.split('|', 2)[1]);
                            var item = items.children().last();
                            var h = item.height();
                            item.css({ height: 0 });
                            item.animate({ height: h }, {
                                duration: 1000,
                                easing: 'ease-in-out',
                                complete: function () {
                                    item.css({ height:'auto'});
                                }
                            });

                        }
                        S.kanban.cards.init();
                    },
                    function () {
                        S.message.show('.board .message', "error", S.message.error.generic);
                    }
                );
            }
        },

        getId: function (elem) {
            if (!elem.hasClass('item')) { elem = elem.parents('.item').first(); }
            return elem[0].className.split('id-')[1].split(' ')[0];
        },

        details: function (e) {
            if (S.kanban.card.drag.dragging == true) { return;}
            var elem = $(e.target);
            var id = S.kanban.card.getId(elem);
            var data = {
                boardId: S.board.id,
                cardId: id
            };
            S.popup.show("", S.loader(), { width: 350 });
            S.ajax.post('Card/Kanban/Details', data,
                function (d) {
                    var card = d.split('|');
                    S.popup.show(card[0], card[1], { width: '90%', maxWidth: 750 });
                },
                function () {
                    S.message.show('.board .message', "error", S.message.error.generic);
                }
            );

        },

        drag: {
            dragging:false,
            init: function () {
                $('.lists .item').each(function (item) {
                    var elem = $(item);
                    S.drag.add(elem, elem,
                        function (item) { //onStart
                            //angle card slightly clockwise
                            item.elem.addClass('dragging');
                            S.kanban.card.drag.dragging = true;
                            $('.board').addClass('dragging');
                        },
                        function (item) { //onDrag
                            //
                        },
                        function (item) { //onStop
                            item.elem.removeClass('dragging');
                            setTimeout(function () { S.kanban.card.drag.dragging = true; }, 100);
                            $('.board').removeClass('dragging');
                            item.elem.css({ top: 0, left: 0 });
                        },
                        //options
                        {hideArea:true, hideAreaOffset:7}
                    )
                });
            }
        }
    },

    cards: {
        init: function () {
            $('.board .lists .item').off('click').on('click', S.kanban.card.details);
        }
    }
};

S.kanban.init();