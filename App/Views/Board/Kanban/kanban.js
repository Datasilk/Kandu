S.kanban = {
    init: function () {
        if ($('.body > .kanban').length == 0) { return; }
        $('.btn-add-list').on('click', S.kanban.list.create.show);
        $('.kanban .lists .btn-close').on('click', S.kanban.list.create.cancel);
        $('.form-new-list > form').on('submit', function (e) {
            e.preventDefault(); S.kanban.list.create.submit(); return false;
        });

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
        $('.kanban > .lists').on('touchstart', S.kanban.scroll.touchstart);
        $('.kanban > .lists').on('touchmove', S.kanban.scroll.touchmove);
        $('.kanban > .lists').on('touchend', S.kanban.scroll.touchend);

        //add event for list scrollbars
        S.kanban.list.resize();
        S.scrollbar.add('.kanban .list-items', {
            footer: S.kanban.list.scroll.footer,
            touch: true,
            touchStart: S.kanban.scroll.touchListStart,
            touchEnd: S.kanban.scroll.touchListEnd,
        });

        //resize list height to fit window
        $(window).on('resize', S.kanban.list.resize);
        $(window).on('scroll', S.kanban.list.resize);
        S.kanban.list.resize();

        //load card if card is in URL hash
        var hash = S.util.url.hash.params();
        console.log(hash);
        if (hash.length > 0) {
            var param = hash.filter(a => a.key == 'card');
            if (param.length > 0) {
                param = param[0];
                var e = { target: $('.item.id-' + param.value)[0] };
                console.log(e);
                S.kanban.card.details(e);
            }
        }
    },

    scroll: {
        selected: null,
        disabled: false,

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
            $('.lists .columns > div').each((i, e) => {
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

        start: function (e, istouch) {
            const lists = $('.kanban .lists');
            const scroller = $('.kanban > .scroller');
            const scrollbar = $('.kanban > .scroller .scrollbar');
            const win = S.window.pos();
            let w = 0;
            let scrollW = scroller.width();
            let pos = lists.position();
            pos.width = lists.width();
            //get total width of all lists
            $('.lists .columns > div').each((i, e) => {
                w += $(e).width();
            });

            var anim = S.kanban.scroll.selected == null;

            S.kanban.scroll.selected = {
                scrollbar: scrollbar,
                columns: lists.find('.columns'),
                width: win.w,
                barWidth: (scrollW / w) * win.w,
                listsW: w,
                cursorX: e.clientX,
                currentX: e.clientX,
                lastX: e.clientX,
                speedSteps: [],
                speed: 0,
                speedStart: 0,
                coastStart: null,
                coasting:false,
                barX: scrollbar.offset().left,
                diff: (1 / win.w) * ((scrollW / w) * win.w)
            };
            if (!istouch === true) {
                e.cancelBubble = true;
                e.stopPropagation();
                e.preventDefault();
                $('body').on('mousemove', S.kanban.scroll.move);
                $('body').on('mouseup', S.kanban.scroll.stop);
            }
            if (anim) {
                S.kanban.scroll.animate.call(S.kanban.scroll);
            }
        },

        move: function (e) {
            S.kanban.scroll.selected.currentX = e.clientX;
        },

        animate: function () {
            let sel = S.kanban.scroll.selected;
            if (sel == null) { return; }

            //update speed steps
            if (sel.speedSteps.length >= 5) { S.kanban.scroll.selected.speedSteps.shift(); }
            S.kanban.scroll.selected.speedSteps.push(sel.lastX - sel.currentX);
            S.kanban.scroll.selected.lastX = sel.currentX || 0;

            //animate scrollbar & columns
            const curr = sel.currentX - sel.cursorX - (10 - sel.barX);
            let perc = (100 / (sel.width - sel.barWidth)) * curr;
            if (perc > 100) { perc = 100; }
            if (perc < 0) { perc = 0; }
            sel.scrollbar.css({ left: ((sel.width - sel.barWidth) / 100) * perc });
            sel.columns.css({ left: -1 * (((sel.listsW - sel.width) / 100) * perc) });
            requestAnimationFrame(() => {
                S.kanban.scroll.animate.call(S.kanban.scroll);
            });
        }, 

        stop: function (e) {
            $('body').off('mousemove', S.kanban.scroll.move);
            $('body').off('mouseup', S.kanban.scroll.stop);
            S.kanban.scroll.selected = null;
        },

        touchstart: function (e) {
            if (S.kanban.scroll.disabled == true) { return;}
            S.kanban.scroll.start(e.touches[0], true);
        },

        touchmove: function (e) {
            let sel = S.kanban.scroll.selected;
            if (sel == null || S.kanban.scroll.disabled == true) { return; }
            let newX = sel.cursorX + ((sel.cursorX - e.touches[0].clientX) * sel.diff);
            S.kanban.scroll.selected.currentX = newX;
        },

        touchend: function (e) {
            //coast animation
            let sel = S.kanban.scroll.selected;
            if (sel == null || S.kanban.scroll.disabled == true) { return; }
            if (sel.speedSteps.length == 0) {
                S.kanban.scroll.selected = null;
                return;
            }
            S.kanban.scroll.selected.speed = (sel.speedSteps.reduce((a, b, c, d) => b + d[c]) / sel.speedSteps.length) * sel.diff * -1 * 10;
            S.kanban.scroll.selected.speedStart = S.kanban.scroll.selected.speed;
            S.kanban.scroll.selected.coastStart = Date.now();
            S.kanban.scroll.selected.coasting = true;
            requestAnimationFrame(() => {
                S.kanban.scroll.coast.call(S.kanban.scroll);
            });
        },

        coast: function () {
            let sel = S.kanban.scroll.selected;
            if (sel == null || sel.coasting == false || S.kanban.scroll.disabled == true) { return;}
            let oldspeed = sel.speed;
            S.kanban.scroll.selected.currentX = sel.currentX + (sel.speed);
            var newspeed = sel.speedStart - (sel.speedStart * ((1 / 500) * (Date.now() - sel.coastStart)));
            S.kanban.scroll.selected.speed = newspeed;
            if ((oldspeed > 0 && newspeed <= 0) || (oldspeed < 0 && newspeed >= 0) || oldspeed == 0) {
                S.kanban.scroll.selected = null;
                return;
            }
            requestAnimationFrame(() => {
                S.kanban.scroll.coast.call(S.kanban.scroll);
            });
        },

        touchListStart: function (e, options) {
            //S.kanban.scroll.disabled = true;
        },

        touchListEnd: function (e, options) {
            //S.kanban.scroll.disabled = false;
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
            $('.lists .list' + (id ? (' .id-' + id) : '')).each(function (i, e) {
                const list = $(e);
                const foot = list.find('.form-new-card');
                const foot2 = list.find('.btn-add-card');
                const footH = foot.height() + foot2.height();
                const listitems = list.find('.list-items');
                listitems.css({ maxHeight: h - footH });
            });
        },

        scroll: {
            footer: function (list) {
                //returns height of footer to offset scrollbar height
                const foot = list.parent().find('.form-new-card');
                const foot2 = list.parent().find('.btn-add-card');
                return foot.height() + foot2.height() + 38;
            }
        },

        drag: {
            dragging: false, timer: null,
            geometry: { lists: null },
            current: { listId: null, list: null, side: 'left', pos:0},

            init: function (elems) {
                var selector = elems || '.lists .list';
                $(selector).find('.list-head').each(function (i, item) {
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
                                lists.each(function (i, currlist) {
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
                lists.each(function (i, list) {
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
                    S.util.message('.board .message', "error", S.message.error.generic);
                }
            );
        }
    },

    card: {
        selected: null,
        boardId: null,

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
                field[0].scrollTo(0, 0);
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
                        S.util.message('.board .message', "error", S.message.error.generic);
                    }
                );
            }
        },

        getId: function (elem) {
            if (!elem.hasClass('item')) { elem = elem.parents('.item').first(); }
            return S.util.element.getClassId(elem, 'id-');
        },

        getBoardId: function (elem) {
            if (!elem.hasClass('item')) { elem = elem.parents('.item').first(); }
            return elem.attr('data-board-id');
        },

        details: function (e, callback) {
            if (S.kanban.card.drag.dragging == true) { return; }
            var elem = $(e.target);

            var id = S.kanban.card.getId(elem);
            var boardId = S.kanban.card.getBoardId(elem);
            S.kanban.card.boardId = boardId;

            var data = {
                boardId: boardId,
                cardId: id
            };
            S.kanban.card.selected = {
                id: id,
                listId: S.util.element.getClassId(elem.parents('.list'), 'id-'),
                elem: $('.board .list .item.id-' + id)
            };
            var popup = S.popup.show("", S.loader(), {width: 350});
            S.ajax.post('Card/Kanban/Details', data,
                function (d) {
                    var card = d.split('|', 2);
                    S.popup.hide(popup);
                    S.popup.show(card[0], card[1], {
                        width: '90%', maxWidth: 750,
                        onClose: function () {
                            if (callback) { callback(); }
                        }
                    });
                    $('.popup .card-field-title').on('click', S.kanban.card.title.edit);
                    $('.popup .btn-archive a').off('click').on('click', S.kanban.card.archive);
                    $('.popup .btn-restore a').off('click').on('click', S.kanban.card.restore);
                    $('.popup .btn-delete a').off('click').on('click', S.kanban.card.delete);
                    $('.popup .description-link a').off('click').on('click', S.kanban.card.description.edit);
                    $('.popup .field-description .btn-cancel').off('click').on('click', S.kanban.card.description.cancel);
                    $('.popup .field-description form').off('submit').on('submit', S.kanban.card.description.update);
                    S.kanban.card.description.markdown();
                    S.kanban.card.title.edit();
                    S.kanban.card.title.cancel();
                    S.popup.resize();
                },
                function () {
                    S.util.message('.board .message', "error", S.message.error.generic);
                }
            );

        },

        drag: { //also handles card onClick
            dragging: false, timer: null,
            geometry: { lists: null },
            current: { listId: null, list:null, cardId: null, card:null, below: true, special: null },

            init: function (elems) {
                var selector = elems || '.lists .item';
                $(selector).each(function (i, item) {
                    var cardElem = $(item);
                    S.drag.add(cardElem, cardElem,
                        //onStart  /////////////////////////////////////////////////////////////////////////////////
                        function (item) {
                            this.dragging = true;
                            item.elem.addClass('dragging');
                            $('.board').addClass('dragging');
                            this.headerHeight = $('header').height();

                            //clone card for visual representation
                            let clone = $(item.elem[0].cloneNode(true));
                            clone.addClass('clone');
                            item.elem.addClass('hide');
                            $('.kanban > .list .items').prepend(clone);
                            this.elem = $(S.drag.item.elem);
                            this.listId = S.util.element.getClassId(this.elem.parents('.list').first(), 'id-');
                            S.drag.item.elem = clone;
                            
                            //reset classes
                            $('.list .item.hovering').removeClass('hovering').parent().removeClass('hovering upward downward');

                            //update geometry for lists & cards
                            this.getGeometryForLists();
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
                                                var ownerList = false;
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
                                                    if (listId == this.listId) {
                                                        ownerList = true;
                                                    }
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
                                                //check if card is above dragging card in same list
                                                if (ownerList == true) {
                                                    const ownlist = $('.list.id-' + this.listId + ' .item');
                                                    let foundcard = false;
                                                    let founddrag = false;
                                                    for (var z = 0; z < ownlist.length; z++) {
                                                        if (ownlist[z].className.indexOf('id-' + current.cardId) >= 0) {
                                                            foundcard = true;
                                                            break;
                                                        } else if (ownlist[z] == this.elem[0]) {
                                                            founddrag = true;
                                                        }
                                                    }
                                                    if ((foundcard == true && founddrag == false) ||
                                                        (foundcard == false && founddrag == true)) {
                                                        //card is above dragged card
                                                        S.drag.item.offset.y = -this.headerHeight + 50;
                                                    } else {
                                                        //card is below dragged card
                                                        S.drag.item.offset.y = -this.headerHeight;
                                                    }
                                                } else if (changed == true) {
                                                    S.drag.item.offset.y = -this.headerHeight;
                                                }
                                                found = true;
                                                break;
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
                                S.drag.item.offset.y = -this.headerHeight;
                            }
                        },
                        //onStop  /////////////////////////////////////////////////////////////////////////////////
                        function (item) {
                            $('.kanban > .list .items > *').remove();
                            item.elem = $(this.elem);
                            item.elem.removeClass('dragging hide').css({ 'margin-bottom': '' });
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
                                cardlist.each(function (i, card) {
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
                        { hideArea: true, hideAreaOffset: 7, useElemPos:true, offsetY: -($('header').height()), speed:1000 / 30, callee: S.kanban.card.drag }
                    )
                });
            },

            getGeometryForLists: function () {
                //get current rectangular geometry for all lists & subsequent cards
                var geo = { lists: [] };
                var lists = $('.lists .list');
                lists.each(function (i, list) {
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
                boardId: S.kanban.card.boardId || S.board.id,
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
                        S.util.message('.board .message', "error", S.message.error.generic);
                    }
                },
                function () {
                    S.util.message('.board .message', "error", S.message.error.generic);
                }
            );
        },

        restore: function () {
            var data = {
                boardId: S.kanban.card.boardId || S.board.id,
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
                        S.util.message('.board .message', "error", S.message.error.generic);
                    }
                },
                function () {
                    S.util.message('.board .message', "error", S.message.error.generic);
                }
            );
        },

        delete: function () {
            var data = {
                boardId: S.kanban.card.boardId || S.board.id,
                cardId: S.kanban.card.selected.id
            };
            S.ajax.post('Cards/Delete', data,
                function (d) {
                    if (d == 'success') {
                        S.util.message('.board .message', "alert", 'The selected card has been permanently deleted from this board');
                        S.popup.hide();
                    } else {
                        S.util.message('.board .message', "error", S.message.error.generic);
                    }
                },
                function () {
                    S.util.message('.board .message', "error", S.message.error.generic);
                }
            );
        },

        replace: function (html) {
            let card = $('.board .list .item.id-' + S.kanban.card.selected.id).parent();
            card.before(html);
            let newcard = card.prev();
            S.kanban.card.drag.init(newcard.find('.item')[0]);
            card.remove();
        },

        title: {
            cached: null, 
            edit: function () {
                if ($('.popup .card-field-title').hasClass('transparent') == false) { return; }
                let title = $('.popup .textarea-clone').val();
                let input = $('.popup .card-field-title textarea');
                S.kanban.card.title.cached = title;
                input.val(title);
                $('.popup .card-field-title').removeClass('transparent');
                input.on('change, keyup', S.kanban.card.title.change);
                input.on('keydown', S.kanban.card.title.keydown);
                S.kanban.card.title.change();
                $(window).on('click', S.kanban.card.title.update);
            },

            keydown: function (e) {
                if (e.keyCode && e.keyCode == 13) {
                    S.kanban.card.title.update({});
                    e.preventDefault();
                    return false;
                }
            },

            change: function (e) {
                var field = $('.popup .card-field-title textarea');
                //resize field
                var clone = field.parent().find('.textarea-clone > div');
                clone.html(field.val());
                field.css({ height: clone.height() });
                field[0].scrollTo(0, 0);
                //update title
                $('.popup .title h5').html(field.val());
            },

            cancel: function () {
                let input = $('.popup .card-field-title textarea');
                $(window).off('click', S.kanban.card.title.update);
                input.off('change, keyup', S.kanban.card.title.change);
                input.off('keydown', S.kanban.card.title.keydown);
                $('.popup .card-field-title').addClass('transparent');
            },

            update: function (e) {
                let input = $('.popup .card-field-title textarea');
                if (e.target == input[0] || e.target == input.parent()[0]) { return false; }
                S.kanban.card.title.cancel();
                if (input.val().trim() == S.kanban.card.title.cached) { return false; }
                var data = {
                    boardId: S.kanban.card.boardId || S.board.id,
                    cardId: S.kanban.card.selected.id,
                    name: input.val()
                };
                S.ajax.post('Cards/UpdateName', data,
                    function (d) {
                        if (d != '') {
                            //replace existing card with updated card
                            S.kanban.card.replace(d);
                        } else {
                            S.util.message('.board .message', "error", S.message.error.generic);
                        }
                    },
                    function () {
                        S.util.message('.board .message', "error", S.message.error.generic);
                    }
                );
                if (e.preventDefault) { e.preventDefault(); }
                return false;
            }
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
                    boardId: S.kanban.card.boardId || S.board.id,
                    cardId: S.kanban.card.selected.id,
                    description: $('#card_description').val()
                };
                S.ajax.post('Cards/UpdateDescription', data,
                    function (d) {
                        if (d != '') {
                            //replace existing card with updated card
                            S.kanban.card.replace(d);

                            //update description markdown
                            S.kanban.card.description.markdown();
                            S.kanban.card.description.hide();
                        } else {
                            S.util.message('.board .message', "error", S.message.error.generic);
                        }
                    },
                    function () {
                        S.util.message('.board .message', "error", S.message.error.generic);
                    }
                );
                e.preventDefault();
                return false;
            }
        }
    }
};

S.kanban.init();