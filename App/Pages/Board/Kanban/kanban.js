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

        //add callback for header boards popup
        S.head.boards.callback.add('kanban', S.kanban.list.resize);

        //add click & drag capabilities to cards
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
            return S.util.element.getClassId(elem, 'id-');
        },

        details: function (e) {
            if (S.kanban.card.drag.dragging == true) { return; }
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
            dragging: false, timer: null,
            geometry: { lists: null },
            current: { listId: null, list:null, cardId: null, card:null, below: true, special: null },

            init: function () {
                $('.lists .item').each(function (item) {
                    var cardElem = $(item);
                    S.drag.add(cardElem, cardElem,
                        //onStart  /////////////////////////////////////////////////////////////////////////////////
                        function (item) {
                            var self = this;
                            self.dragging = true;
                            item.elem.addClass('dragging');
                            $('.board').addClass('dragging');

                            //reset classes
                            $('.list .item.hovering').removeClass('hovering').parent().removeClass('hovering upward downward');

                            //update geometry for lists & cards
                            self.getGeometryForLists();
                        },
                        //onDrag /////////////////////////////////////////////////////////////////////////////////
                        function (item) { 
                            //detect where to drop the card
                            if (this.dragging == false) { return;}
                            var current = this.current;
                            var geo = this.geometry;
                            var bounds = { top: item.cursor.y - 5, right: item.cursor.x, bottom: item.cursor.y + 5, left: item.cursor.x };

                            //first, detect which list the cursor is over
                            var found = false;
                            var hovering = $('.list .item.hovering, .list .items.hovering');
                            for (var x = 0; x < geo.lists.length; x++) {
                                var list = geo.lists[x];
                                if (S.math.intersect(list, bounds) == true) {
                                    //next, detect which card in the list that the cursor is over
                                    var listId = S.util.element.getClassId(list.elem, 'id-');
                                    var changed = false;
                                    $('.list:not(.id-' + listId + ')').find('.hovering').removeClass('hovering').parent().removeClass('hovering upward downward');
                                    if (list.cards.length > 0) {
                                        for (var y = 0; y < list.cards.length; y++) {
                                            var card = list.cards[y];
                                            if (S.math.intersect(card, bounds)) {
                                                var elem = $(card.elem);
                                                if ((current.cardId == null || !elem.hasClass('id-' + current.cardId)) && !elem.hasClass('dragging')) {
                                                    //found list belonging to card
                                                    $('.list .item.hovering').removeClass('hovering').parent().removeClass('hovering upward downward');
                                                    current.listId = listId;
                                                    current.list = list.elem;
                                                    current.cardId = S.util.element.getClassId(card.elem, 'id-');
                                                    current.card = elem;
                                                    elem.addClass('hovering');
                                                    elem.parent().addClass('hovering');
                                                    changed = true;
                                                }
                                                var pos = elem.offset();
                                                var parent = elem.parent();
                                                if (bounds.top - pos.top < elem.height() / 2) {
                                                    //upward drop
                                                    if (!parent.hasClass('upward')) {
                                                        parent.addClass('upward').removeClass('downward');
                                                        S.drag.alteredDOM();
                                                        current.below = false;
                                                    }
                                                } else {
                                                    //downward drop
                                                    if (!parent.hasClass('downward')) {
                                                        parent.addClass('downward').removeClass('upward');
                                                        S.drag.alteredDOM();
                                                        current.below = true;
                                                    }
                                                }
                                                found = true;
                                            }
                                        }
                                    } else {
                                        //list contains no cards
                                        if (list.elem.find('.item').length == 0) {
                                            if (current.listId != listId) {
                                                current.listId = listId;
                                                current.list = list.elem;
                                                current.cardId = null;
                                                current.card = null;
                                                changed = true;
                                                list.elem.find('.items').addClass('hovering');
                                            }
                                            found = true;
                                        }
                                    }

                                    if (changed == true) {
                                        this.getGeometryForLists();
                                    }
                                    break;
                                }
                            }
                            if (found == false && hovering.length > 0) {
                                hovering.removeClass('hovering');
                                hovering.parent().removeClass('hovering upward downward');
                                this.current = { listId: null, cardId: null, card: null, below: true };
                                S.drag.alteredDOM();
                            }
                        },
                        //onStop  /////////////////////////////////////////////////////////////////////////////////
                        function (item) { 
                            item.elem.removeClass('dragging');
                            $('.board .lists .list .item').removeClass('hovering').parent().removeClass('hovering upward downward');
                            $('.board').removeClass('dragging');
                            item.elem.css({ top: 0, left: 0 });

                            //move card in DOM to drop area
                            if (this.current.listId != null) {
                                if (this.current.cardId != null) {
                                    if (this.current.below == true) {
                                        //append below card
                                        this.current.card.parent().after(item.elem.parent());
                                    } else {
                                        //append above card
                                        this.current.card.parent().before(item.elem.parent());
                                    }
                                } else {
                                    //drop card into empty list
                                    this.current.list.find('.items').append(item.elem.parent());
                                }

                                //send update to server via ajax
                                var list = item.elem.parents('.list');
                                var listId = S.util.element.getClassId(list);
                                var cards = [];
                                var cardlist = list.find('.item');
                                cardlist.each(function (card) {
                                    cards.push(S.util.element.getClassId(card));
                                });

                                S.ajax.post('Card/Kanban/Move', { boardId: S.board.id, listId: listId, cardId: S.util.element.getClassId(item.elem), cardIds: cards });
                            }
                            setTimeout(function () { S.kanban.card.drag.dragging = false; }, 100);
                        },
                        //onClick  /////////////////////////////////////////////////////////////////////////////////
                        function (item) {
                            S.kanban.card.details({ target: item.elem });
                        },
                        //options
                        { hideArea:true, hideAreaOffset:7, speed:1000 / 30, callee: S.kanban.card.drag }
                    )
                });
            },

            getGeometryForLists: function () {
                var geo = { lists: [] };
                var lists = $('.lists .list');
                var cardelem = S.kanban.card.drag.current.card;
                if (cardelem != null) { cardelem = cardelem[0]; }
                lists.each(function (list) {
                    list = $(list);
                    var pos = list.offset();
                    geo.lists.push({
                        elem: list,
                        top: pos.top,
                        left: pos.left,
                        right: pos.left + list.width(),
                        bottom: pos.top + list.height(),
                        width: list.width(),
                        height: list.height(),
                        cards: list.find('.item').map(function (index, card) {
                            if (card == S.drag.item.element[0]) {
                                return { elem: card, top: 0, left: 0, right: 0, bottom: 0, width: 0, height: 0 };
                            } else {
                                card = $(card);
                                var parent = card.parent();
                                var cpos = parent.offset();
                                return {
                                    elem: card[0],
                                    top: cpos.top,
                                    left: cpos.left,
                                    right: cpos.left + parent.width(),
                                    bottom: cpos.top + parent.height(),
                                    width: parent.width(),
                                    height: parent.height()
                                }
                            }
                        })
                    });
                });
                S.kanban.card.drag.geometry = geo;
            }
        }
    }
};

S.kanban.init();