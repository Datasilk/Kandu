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

        //add click & drag capabilities to lists & cards
        S.kanban.list.menu.init();
        S.kanban.list.drag.init();
        S.kanban.card.drag.init();

        //add events for message popup
        $('.board .message').on('DOMSubtreeModified', S.kanban.list.resize);
        $('.board .message .btn-close').on('click', () => { $('.board .message').addClass('hide').hide(); });

        //add event for horizontal scrollbar
        $('.kanban > .scroller .scrollbar').on('mousedown', S.kanban.scroll.start);
    },

    scroll: {
        selected: null,

        resize: function () {
            //check horizonal scrollbar
            const lists = $('.kanban .lists');
            const scroller = $('.kanban > .scroller');
            const scrollbar = $('.kanban > .scroller .scrollbar');
            const win = S.window.pos();
            let w = 0;
            let scrollW = scroller.width();
            let pos = lists.position();
            pos.width = lists.width();
            //get total width of all lists
            $('.lists .columns > div').each((e) => {
                w += $(e).width();
            });
            if (w > win.w) {
                //lists wider than window
                scroller.removeClass('hide');
                scrollbar.css({ width: (scrollW / w) * win.w });
            } else {
                //lists smaller than window width
                scroller.addClass('hide');
            }
        },

        start: function (e) {
            e.cancelBubble = true;
            e.stopPropagation();
            e.preventDefault();
            const lists = $('.kanban .lists');
            const scroller = $('.kanban > .scroller');
            const scrollbar = $('.kanban > .scroller .scrollbar');
            const win = S.window.pos();
            let w = 0;
            let scrollW = scroller.width();
            let pos = lists.position();
            pos.width = lists.width();
            //get total width of all lists
            $('.lists .columns > div').each((e) => {
                w += $(e).width();
            });

            S.kanban.scroll.selected = {
                scrollbar: scrollbar,
                columns: lists.find('.columns'),
                width: win.w,
                barWidth: (scrollW / w) * win.w,
                listsW: w,
                cursorX: e.clientX,
                currentX: e.clientX,
                barX: scrollbar.offset().left
            };
            $('body').on('mousemove', S.kanban.scroll.move);
            $('body').on('mouseup', S.kanban.scroll.stop);
            S.kanban.scroll.animate.call(S.kanban.scroll);
        },
        move: function (e) {
            S.kanban.scroll.selected.currentX = e.clientX;
        },
        animate: function () {
            const scroll = S.kanban.scroll.selected;
            if (scroll == null) { return; }
            const curr = scroll.currentX - scroll.cursorX - (10 - scroll.barX);
            let perc = (100 / (scroll.width - scroll.barWidth)) * curr;
            if (perc > 100) { perc = 100; }
            if (perc < 0) { perc = 0; }
            scroll.scrollbar.css({ left: ((scroll.width - scroll.barWidth - 20) / 100) * perc });
            scroll.columns.css({ left: -1 * (((scroll.listsW - scroll.width) / 100) * perc) });
            requestAnimationFrame(() => {
                S.kanban.scroll.animate.call(S.kanban.scroll);
            });
        }, 
        stop: function (e) {
            $('body').off('mousemove', S.kanban.scroll.move);
            $('body').off('mouseup', S.kanban.scroll.stop);
            S.kanban.scroll.selected = null;
        }
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
                    $('.lists .add-list').before(d);
                    var lists = $('.lists .list');
                    var list = lists[lists.length - 1];
                    S.kanban.list.menu.init(list);
                    S.kanban.list.drag.init(list);
                });
            }
        },

        getId: function (elem) {
            let e = $(elem);
            if (!e.hasClass('list')) {
                e = e.parents('.list');
                if (e.length > 0) {
                    e = $(e[0]);
                } else {
                    return null;
                }
            }
            const c = e.get().className.split(' ');
            for (let x = 0; x < c.length; x++) {
                if (c[x].indexOf('id-') == 0) {
                    return parseInt(c[x].replace('id-', ''));
                }
            }
            return null;
        },

        resize: function () {
            const id = typeof arguments[0] == 'string' ? arguments[0] : null;
            const lists = $('.kanban .lists');
            const pos = lists.position();
            const win = S.window.pos();
            let h = win.h - pos.top - 85;

            //check horizontal scrollbar
            S.kanban.scroll.resize();

            lists.css({ height: win.h - pos.top });
            //display scrollbars on lists
            $('.lists .list' + (id ? (' .id-' + id) : '')).each(function (e) {
                const list = $(e);
                const items = list.find('.items');
                const foot = list.find('.form-new-card');
                const foot2 = list.find('.btn-add-card');
                const footH = foot.height() + foot2.height();
                const scrollable = list.find('.scrollable');
                const scrollbar = list.find('.scrollbar');
                scrollable.css({ maxHeight: h - footH });
                const itemsH = items.height();
                if (itemsH > h - footH) {
                    //show scrollbar
                    if (!scrollable.hasClass('scroll')) {
                        scrollable.addClass('scroll');
                        //add scroll bar mouse events
                        scrollbar.on('mousedown', S.kanban.list.scroll.start);
                    }
                    list.find('.scroller').css({ height: h - footH - 7 });
                    //update scrollbar height
                    scrollbar.css({ height: ((h - footH - 7) / itemsH) * (h - footH) });
                    //check scrollTo position
                } else {
                    //hide scrollbar
                    if (scrollable.hasClass('scroll')) {
                        scrollable.removeClass('scroll');
                        //remove scroll bar mouse events
                        scrollbar.off('mousedown', S.kanban.list.scroll.start);
                    }
                }
            });
        },

        scroll: {
            selected: { scrollable: null, height: null, itemsH: null },
            start: function (e) {
                e.cancelBubble = true;
                e.stopPropagation();
                e.preventDefault();
                const win = S.window.pos();
                const lists = $('.kanban .lists');
                const pos = lists.position();
                const list = $(e.target).parents('.list');
                const scrollbar = list.find('.scrollbar');
                const scroller = list.find('.scroller');
                const items = list.find('.items');
                const foot = list.find('.form-new-card');
                const foot2 = list.find('.btn-add-card');
                const footH = foot.height() + foot2.height();
                const height = win.h - pos.top - footH - 85;
                items.addClass('scrolling');
                S.kanban.list.scroll.selected = {
                    scrollbar: scrollbar,
                    height: height,
                    barHeight: ((height) / items.height()) * height,
                    items: items,
                    itemsH: items.height(),
                    offsetY: scroller.offset().top,
                    cursorY: e.clientY,
                    currentY: e.clientY,
                    barY: scrollbar.offset().top
                };
                $('body').on('mousemove', S.kanban.list.scroll.move);
                $('body').on('mouseup', S.kanban.list.scroll.stop);
                S.kanban.list.scroll.animate.call(S.kanban.list.scroll);
            },
            move: function (e) {
                S.kanban.list.scroll.selected.currentY = e.clientY;
            },

            animate: function () {
                const scroll = S.kanban.list.scroll.selected;
                if (scroll == null) { return;}
                const curr = scroll.currentY - scroll.cursorY - (scroll.offsetY - scroll.barY);
                let perc = (100 / (scroll.height - scroll.barHeight)) * curr;
                if (perc > 100) { perc = 100; }
                if (perc < 0) { perc = 0;}
                scroll.scrollbar.css({ top: ((scroll.height - scroll.barHeight) / 100) * perc });
                scroll.items.css({ top: -1 * (((scroll.itemsH - scroll.height) / 100) * perc) });
                requestAnimationFrame(() => {
                    S.kanban.list.scroll.animate.call(S.kanban.list.scroll);
                });
            },

            stop: function () {
                $('body').off('mousemove', S.kanban.list.scroll.move);
                $('body').off('mouseup', S.kanban.list.scroll.stop);
                S.kanban.list.scroll.selected.items.removeClass('scrolling');
                S.kanban.list.scroll.selected = null;
            }
        },

        drag: {
            dragging: false, timer: null,
            geometry: { lists: null },
            current: { listId: null, list: null, side: 'left', pos:0},

            init: function (elems) {
                var selector = elems || '.lists .list';
                $(selector).find('.list-head').each(function (item) {
                    var listElem = $(item);
                    S.drag.add(listElem, listElem.parent(),
                        //onStart  /////////////////////////////////////////////////////////////////////////////////
                        function (item) {
                            this.dragging = true;
                            item.elem.addClass('dragging');
                            $('.board').addClass('dragging');

                            //reset classes
                            $('.list .list.hovering').removeClass('hovering').parent().removeClass('hovering leftside rightside');

                            //check the index position of the currectly selected list that is being dragged
                            var lists = item.elem.parent().parent().children();
                            this.current.pos = 0;
                            for (var x = 0; x < lists.length; x++) {
                                if (lists[x] == item.elem.parent()[0]) {
                                    this.current.pos = x;
                                }
                            }

                            //update geometry for lists & cards
                            this.getGeometryForLists();
                        },
                        //onDrag /////////////////////////////////////////////////////////////////////////////////
                        function (item) {
                            //detect where to drop the card
                            if (this.dragging == false) { return; }
                            var current = this.current;
                            var geo = this.geometry;
                            var bounds = { top: item.cursor.y - 5, right: item.cursor.x, bottom: item.cursor.y + 5, left: item.cursor.x };

                            //first, detect which list the cursor is over
                            var found = false;
                            var hovering = $('.list.hovering');
                            var dragged = false; //determines if dragged element comes before or after hovered element

                            for (var x = 0; x < geo.lists.length; x++) {
                                var list = geo.lists[x];
                                if (list.elem[0] == item.elem[0]) {
                                    dragged = true;
                                    item.elem.css({ 'margin-left': -262 * current.pos, 'margin-right': 262 * (current.pos - 1) + 12 });
                                    continue;
                                }
                                var pos = list.elem.offset();
                                pos.width = list.elem.width();
                                if (list.elem.hasClass('rightside')) {
                                    pos.rightside = 262;
                                    pos.leftside = 0;
                                } else if (list.elem.hasClass('leftside')) {
                                    pos.leftside = 262;
                                    pos.rightside = 0;
                                } else {
                                    pos.leftside = 0;
                                    pos.rightside = 0;
                                }
                                if (bounds.left <= pos.left + pos.width + pos.rightside - 20) {
                                    found = true;
                                    var listId = S.util.element.getClassId(list.elem, 'id-');
                                    var changed = false;
                                    if (current.listId != listId) {
                                        $('.list.hovering').removeClass('hovering leftside rightside');
                                        current.listId = listId;
                                        current.list = list.elem;
                                        list.elem.addClass('hovering');
                                        changed = true;
                                    }
                                    if (bounds.left < (pos.left + pos.leftside + pos.width)) {
                                        current.side = 'left';
                                        list.elem.removeClass('rightside').addClass('leftside');
                                        changed = true;
                                    } else {
                                        current.side = 'right';
                                        list.elem.removeClass('leftside').addClass('rightside');
                                        changed = true;
                                    }
                                    if (changed == true) { S.drag.alteredDOM(); }
                                    if (dragged == false) {
                                        item.elem.css({ 'margin-left': -262 * (current.pos + 1), 'margin-right': 262 * (current.pos) + 12 });
                                    }
                                    break;
                                }
                            }
                        },
                        //onStop  /////////////////////////////////////////////////////////////////////////////////
                        function (item) {
                            item.elem.removeClass('dragging');
                            $('.lists .list').removeClass('hovering leftside rightside after-hover').css({ 'margin-left': '', 'margin-right':'', left: '', top: '' });
                            $('.board').removeClass('dragging');
                            item.elem.css({ top: 0, left: 0 });

                            //move card in DOM to drop area
                            if (this.current.listId != null) {
                                if (this.current.side == 'left') {
                                    this.current.list.before(item.elem.parent());
                                } else {
                                    this.current.list.after(item.elem.parent());
                                }
                                this.current.listId = '';
                                this.current.list = null;
                                
                                //send update to server via ajax
                                var list = item.elem;
                                var listId = S.util.element.getClassId(list);
                                var lists = $('.lists .list');
                                var sort = [];
                                lists.each(function (currlist) {
                                    sort.push(S.util.element.getClassId(currlist));
                                });

                                S.ajax.post('List/Kanban/Move', { boardId: S.board.id, listIds: sort });
                            }
                            setTimeout(function () { S.kanban.list.drag.dragging = false; }, 100);
                        },
                        //onClick  /////////////////////////////////////////////////////////////////////////////////
                        function (item) {},
                        //options
                        { hideAreaOffset: 7, speed: 1000 / 30, callee: S.kanban.list.drag, offsetX:-19, offsetY:-15 }
                    )
                });
            },

            getGeometryForLists: function () {
                var geo = { lists: [] };
                var lists = $('.lists .list');
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
                        height: list.height()
                    });
                });
                S.kanban.list.drag.geometry = geo;
            }
        },

        menu: {
            selected: null,

            init: function (elems) {
                const selector = elems || '.lists .list';
                $(selector).find('.list-head .more').off().on('click', S.kanban.list.menu.show);
            },

            show: function (e) {
                //remove existing menu
                $('.list-menu').remove();
                //add new menu instance
                const menu_button = $(e.target);
                const listid = S.kanban.list.getId(menu_button);
                S.kanban.list.menu.selected = listid;
                $('body').append($('#template_listmenu').html());
                const menu = $('.list-menu');
                //reposition menu
                const pos = menu_button.offset();
                $('.list-menu').css({ left: pos.left, top: pos.top + 25 });
                //add menu click events
                menu.find('.btn-close').on('click', function () { menu.remove(); });
                menu.find('.archive-list').on('click', S.kanban.list.archive);
                //add body click to hide menu
                $('body').on('click', S.kanban.list.menu.bodyClick);
            },

            bodyClick: function () {
                $('body').off('click', S.kanban.list.menu.bodyClick);
                $('.list-menu').remove();
            }
        },

        archive: function () {
            const listid = S.kanban.list.menu.selected;
            S.ajax.post('Lists/Archive', { boardId: S.board.id, listId: listid },
                function (d) {
                    $('.list.id-' + listid).remove();
                },
                function () {
                    S.message.show('.board .message', "error", S.message.error.generic);
                }
            );
        }
    },

    card: {
        selected: null,

        create: {
            show: function (listid) {
                var list = $('.list.id-' + listid);
                var form = list.find('.form-new-card');
                form.html($('#template_newcard').html());
                form.css({ height: '0px' });
                form.removeClass('hide').addClass('show');
                list.find('.btn-add-card').hide();
                list.find('.btn-close').on('click', function () { S.kanban.card.create.hide(listid); });
                list.find('form').on('submit', function (e) {
                    S.kanban.card.create.submit(listid);
                    e.preventDefault();
                    return false;
                });
                list.find('.new-card-label')[0].focus();
                var field = form.find('textarea');
                field.on('change, keyup', function (e) {
                    return S.kanban.card.create.change(e, listid);
                });

                form.animate({ height: 80 }, {
                    duration: 3000,
                    progress: () => {
                        S.kanban.list.resize(listid);
                    },
                    complete: () => {
                        form.removeAttr('style');
                    }
                });
            },

            change: function (e, listid) {
                if (e.keyCode && e.keyCode == 13) {
                    S.kanban.card.create.submit(listid);
                    e.preventDefault();
                    return false;
                }
                var field = $(e.target);
                //resize field
                var clone = field.parent().find('.textarea-clone > div');
                clone.html(field.val().replace(/\n/g, '<br/>') + '</br>');
                field.css({ height: clone.height() });
                field.scrollTop = 0;
                S.kanban.list.resize(listid);
            },

            hide: function (listid) {
                var list = $('.list.id-' + listid);
                list.find('.form-new-card').removeClass('show').addClass('cancel');
                S.kanban.list.resize(listid);
                setTimeout(function () {
                    list.find('.form-new-card').removeClass('cancel').addClass('hide').html('');
                    list.find('.btn-add-card').show();
                    S.kanban.list.resize(listid);
                }, 310);
            },

            submit: function (listid) {
                var list = $('.list.id-' + listid);
                var text = list.find('.new-card-label');
                var data = {
                    boardId: S.board.id,
                    listId: listid,
                    name: text.val().replace('\n', '').replace('\r', '')
                };
                text.val('');
                S.ajax.post('Cards/Create', data,
                    function (d) {
                        var items = list.find('.items')
                        items.append(d);
                        var item = items.children().last();
                        var h = item.height();
                        item.css({ height: 0 });
                        item.animate({ height: h }, {
                            duration: 1000,
                            easing: 'ease-in-out',
                            progress: () => {
                                S.kanban.list.resize(listid);
                            },
                            complete: function () {
                                item.css({ height:''});
                            }
                        });
                        var nodes = items.children();
                        S.kanban.card.drag.init($(nodes[nodes.length - 1]).children()[0]);
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
            S.kanban.card.selected = {
                id: id,
                listId: S.util.element.getClassId(elem.parents('.list'), 'id-'),
                elem: $('.board .list .item.id-' + id)
            };
            S.popup.show("", S.loader(), { width: 350 });
            S.ajax.post('Card/Kanban/Details', data,
                function (d) {
                    var card = d.split('|');
                    S.popup.show(card[0], card[1], { width: '90%', maxWidth: 750 });
                    $('.popup .btn-archive a').off('click').on('click', S.kanban.card.archive);
                    $('.popup .btn-restore a').off('click').on('click', S.kanban.card.restore);
                    $('.popup .btn-delete a').off('click').on('click', S.kanban.card.delete);
                    $('.popup .description-link a').off('click').on('click', S.kanban.card.description.edit);
                    $('.popup .field-description .btn-cancel').off('click').on('click', S.kanban.card.description.cancel);
                    $('.popup .field-description form').off('submit').on('submit', S.kanban.card.description.update);
                    S.kanban.card.description.markdown();
                },
                function () {
                    S.message.show('.board .message', "error", S.message.error.generic);
                }
            );

        },

        drag: { //also handles card onClick
            dragging: false, timer: null,
            geometry: { lists: null },
            current: { listId: null, list:null, cardId: null, card:null, below: true, special: null },

            init: function (elems) {
                var selector = elems || '.lists .item';
                $(selector).each(function (item) {
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
                            item.elem.removeClass('dragging').css({ 'margin-left': ''});
                            $('.board .lists .list .item').removeClass('hovering').parent().removeClass('hovering upward downward');
                            $('.board .lists .items').removeClass('hovering');
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
                                S.kanban.list.resize(listId);

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
        },

        archive: function () {
            var data = {
                boardId: S.board.id,
                cardId: S.kanban.card.selected.id
            };
            S.ajax.post('Cards/Archive', data,
                function (d) {
                    if (d == 'success') {
                        //hide archive button & show restore and delete buttons
                        $('.popup .btn-archive').addClass('hide');
                        $('.popup .btn-restore').removeClass('hide');
                        $('.popup .btn-delete').removeClass('hide');

                        //remove card from list
                        S.kanban.card.selected.elem.remove();
                        S.kanban.list.resize(S.kanban.card.selected.listId);

                    } else {
                        S.message.show('.board .message', "error", S.message.error.generic);
                    }
                },
                function () {
                    S.message.show('.board .message', "error", S.message.error.generic);
                }
            );
        },

        restore: function () {
            var data = {
                boardId: S.board.id,
                cardId: S.kanban.card.selected.id
            };
            S.ajax.post('Cards/Restore', data,
                function (d) {
                    if (d != '') {
                        //hide restore and delete buttons, then show archive button
                        $('.popup .btn-restore').addClass('hide');
                        $('.popup .btn-delete').addClass('hide');
                        $('.popup .btn-archive').removeClass('hide');

                        //add card to list
                        $('.board .list.id-' + S.kanban.card.selected.listId + ' .items').append(d);
                        S.kanban.list.resize(S.kanban.card.selected.listId);

                    } else {
                        S.message.show('.board .message', "error", S.message.error.generic);
                    }
                },
                function () {
                    S.message.show('.board .message', "error", S.message.error.generic);
                }
            );
        },

        delete: function () {
            var data = {
                boardId: S.board.id,
                cardId: S.kanban.card.selected.id
            };
            S.ajax.post('Cards/Delete', data,
                function (d) {
                    if (d == 'success') {
                        S.message.show('.board .message', "alert", 'The selected card has been permanently deleted from this board');
                        S.popup.hide();
                    } else {
                        S.message.show('.board .message', "error", S.message.error.generic);
                    }
                },
                function () {
                    S.message.show('.board .message', "error", S.message.error.generic);
                }
            );
        },

        description: {
            cached: null,
            edit: function () {
                S.kanban.card.description.cached = $('#card_description').val().trim();
                $('.popup .field-description').removeClass('hide');
                $('.popup .new-description').addClass('hide');
                $('.popup .description').addClass('hide');
            },

            cancel: function () {
                $('#card_description').val(S.kanban.card.description.cached);
                S.kanban.card.description.markdown();
                S.kanban.card.description.hide();
            },

            hide: function () {
                $('.popup .field-description').addClass('hide');
                if ($('.popup .description .markdown').html().trim() != '') {
                    $('.popup .description').removeClass('hide');
                } else {
                    $('.popup .new-description').removeClass('hide');
                }  
            },

            markdown: function () {
                var text = $('#card_description').val().trim();
                if (text == '' || text == null) { return;}
                var markdown = new Remarkable({
                    highlight: function (str, lang) {
                        var language = lang || 'javascript';
                        if (language && hljs.getLanguage(language)) {
                            try {
                                return hljs.highlight(language, str).value;
                            } catch (err) { }
                        }
                        try {
                            return hljs.highlightAuto(str).value;
                        } catch (err) { }
                        return '';
                    },
                    breaks: true,
                    linkify: true
                });

                $('.popup .description .markdown').html(
                    markdown.render(text.trim())
                        .replace('<code>', '<code class="hljs">') //bug fix
                );
            },

            update: function (e) {
                var data = {
                    boardId: S.board.id,
                    cardId: S.kanban.card.selected.id,
                    description: $('#card_description').val()
                };
                S.ajax.post('Cards/UpdateDescription', data,
                    function (d) {
                        if (d != '') {
                            //add card to list
                            $('.board .list .item.id-' + S.kanban.card.selected.id).before(d).remove();

                            //update description markdown
                            S.kanban.card.description.markdown();
                            S.kanban.card.description.hide();
                        } else {
                            S.message.show('.board .message', "error", S.message.error.generic);
                        }
                    },
                    function () {
                        S.message.show('.board .message', "error", S.message.error.generic);
                    }
                );
                e.preventDefault(); return false;
            }
        }
    }
};

S.kanban.init();