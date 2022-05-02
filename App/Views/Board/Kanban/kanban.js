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
        $('.board .messages').on('DOMSubtreeModified', S.kanban.list.resize);
        $('.board .messages .btn-close').on('click', () => { $('.board .messages').addClass('hide').hide(); });

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

        var hash = S.util.url.hash.params();
        if (hash.length > 0) {
            var msg = '.toolbar .messages';
            //load card if card is in URL hash
            var param = hash.filter(a => a.key == 'card');
            if (param.length > 0) {
                param = param[0];
                var e = { target: $('.item.id-' + param.value)[0] };
                S.kanban.card.details(e);
            }

            //load message to invited user
            if (hash.filter(a => a.key == 'joined-board').length > 0) {
                S.message.show(msg, 'confirm', 'Thank you for accepting our invitation to this board!');
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
            let w = 0;
            let pos = lists.position();
            let boardWidth = lists.width();
            pos.width = boardWidth;
            //get total width of all lists
            $('.lists .columns > div').each((i, e) => {
                w += $(e).width();
            });
            if (w > boardWidth) {
                //lists wider than window
                scroller.removeClass('hide');
                scrollbar.css({ width: (boardWidth / w) * boardWidth });
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
            let boardWidth = lists.width();
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
                width: boardWidth,
                barWidth: (boardWidth / w) * boardWidth,
                listsW: w,
                cursorX: e.clientX,
                currentX: e.clientX,
                lastX: e.clientX,
                speedSteps: [],
                speed: 0,
                speedStart: 0,
                coastStart: null,
                coasting: false,
                barX: scrollbar.position().left,
                diff: (1 / win.w) * ((boardWidth / w) * win.w)
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
            let perc = (100 / (sel.width - sel.barWidth)) * (sel.barX + sel.currentX - sel.cursorX);
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
            if (S.kanban.scroll.disabled == true) { return; }
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
            if (sel == null || sel.coasting == false || S.kanban.scroll.disabled == true) { return; }
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
                setTimeout(function () { $('.form-new-list').removeClass('cancel').addClass('hide'); }, 500)
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
            current: { listId: null, list: null, side: 'left', pos: 0 },

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
                            $('.lists .list').removeClass('hovering leftside rightside after-hover').css({ 'margin-left': '', 'margin-right': '', left: '', top: '' });
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
                        function (item) { },
                        //options
                        { hideAreaOffset: 7, speed: 1000 / 30, callee: S.kanban.list.drag, offsetX: -19, offsetY: -15 }
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
                    S.util.message('.board .messages', "error", S.message.error.generic);
                }
            );
        }
    },

    card: {
        selected: null,
        boardId: null,
        layouts: [
            { name: 'center' },
            { name: 'rightside' },
            { name: 'leftside' },
            { name: 'fullscreen' }
        ],
        currentLayout: null,

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
                                item.css({ height: '' });
                            }
                        });
                        var nodes = items.children();
                        S.kanban.card.drag.init($(nodes[nodes.length - 1]).children()[0]);
                    },
                    function () {
                        S.util.message('.board .messages', "error", S.message.error.generic);
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
            var popup;
            S.ajax.post('Card/Kanban/Details', data,
                function (d) {
                    var card = d.split('|', 2);
                    console.log(card);
                    function hasbg() {
                        var layout = S.kanban.card.currentLayout;
                        return layout != null ? ['rightside', 'leftside'].filter(a => layout.name).length == 0 : true;
                    }
                    S.popup.hide(popup);
                    popup = S.popup.show(card[0], card[1], {
                        width: '90%', maxWidth: 750, className: 'popup-card-details', bg: hasbg(),
                        onClose: function () {
                            if (callback) { callback(); }
                            $('body').removeClass('card-leftside card-rightside card-fullscreen card-center');
                        },
                        onShow: function () {
                            if (hasbg() == false) {
                                $('.bg.for-popup').addClass('disabled');
                            } else {
                                $('.bg.for-popup').removeClass('disabled');
                            }
                            if (S.kanban.card.currentLayout != null) { S.kanban.card.layout(S.kanban.card.currentLayout.name); }
                            S.kanban.scroll.resize();
                        },
                        onHide: function () {
                            $('body').removeClass('card-leftside card-rightside card-fullscreen card-center');
                        }
                    });
                    S.kanban.card.selected.name = $('.popup.show .card-field-title textarea').val().trim();
                    if (S.kanban.card.currentLayout != null) { S.kanban.card.layout(S.kanban.card.currentLayout.name); }

                    $('.popup.show').prepend('<div class="card-modal-bg" style="display:none;"></div>');

                    //modal bg click
                    $('.popup.show .card-modal-bg, .popup.show .card-modals').on('click', S.kanban.card.modal.hide);

                    //card title event
                    $('.popup.show .card-field-title').on('click', S.kanban.card.title.edit);

                    //assign to button
                    $('.popup.show .button.not-assigned').on('click', S.kanban.card.assignTo.show);

                    //due date button
                    $('.popup.show .button.no-duedate, .popup.show .button.has-duedate').on('click', S.kanban.card.dueDate.show);

                    //initialize checklist
                    S.kanban.card.checklist.init();
                    //initialize attachments
                    S.kanban.card.attachments.init();

                    //add comment button
                    $('.popup.show .comment-link').on('click', S.kanban.card.comments.add.show);

                    //add new comment form (if neccessary)
                    if ($('.popup.show .comment').length > 0) {
                        S.kanban.card.comments.add.show(null, true);
                    }

                    //drop down menu item events
                    $('.popup.show .btn-create-checklist').on('click', S.kanban.card.checklist.add);
                    $('.popup.show .btn-upload-files').on('click', S.kanban.card.attachments.show);
                    $('.popup.show .btn-copy-card').on('click', S.kanban.card.copy.show);
                    $('.popup.show .btn-move-card').on('click', S.kanban.card.move.show);
                    $('.popup.show .btn-archive-card').on('click', S.kanban.card.archive);
                    $('.popup.show .btn-restore-card').on('click', S.kanban.card.restore);
                    $('.popup.show .btn-delete-card').on('click', S.kanban.card.delete);

                    //description events
                    $('.popup.show .description-link').on('click', S.kanban.card.description.edit);
                    $('.popup.show .field-description .btn-cancel').on('click', S.kanban.card.description.cancel);
                    $('.popup.show #card_description').on('input', S.kanban.card.description.resize);
                    $('.popup.show .field-description form').on('submit', S.kanban.card.description.update);
                    S.kanban.card.description.markdown();
                    S.kanban.card.title.edit();
                    S.kanban.card.title.cancel();
                    S.popup.resize();
                    S.accordion.load();
                },
                function () {
                    S.util.message('.board .messages', "error", S.message.error.generic);
                }
            );

        },

        drag: { //also handles card onClick
            dragging: false, timer: null,
            geometry: { lists: null },
            current: { listId: null, list: null, cardId: null, card: null, below: true, special: null },

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
                            if (this.dragging == false) { return; }
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
                        { hideArea: true, hideAreaOffset: 7, useElemPos: true, offsetY: -($('header').height()), speed: 1000 / 30, callee: S.kanban.card.drag }
                    );
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

        checklist: {
            typingTimer: null,
            focusTimer: null,

            init: function () {
                var container = $('.popup.show .card-checklist');
                container.find('.checklist-link').off('click').on('click', S.kanban.card.checklist.addItem);
                container.find('.icon-close').off('click').on('click', S.kanban.card.checklist.deleteItem);
                container.find('.checklist-item input[type="text"]').off('input').on('input', S.kanban.card.checklist.inputItem)
                    .on('focus', S.kanban.card.checklist.focusItemTextbox)
                    .on('blur', S.kanban.card.checklist.blurItemTextbox);
                container.find('.checklist-item input[type="checkbox"]').off('input').on('input', S.kanban.card.checklist.checked);
                S.kanban.card.checklist.drag.init();
            },

            add: function () {
                S.kanban.card.menu.hide();
                S.ajax.post('Cards/AddCheckList', { boardId: S.kanban.card.boardId, cardId: S.kanban.card.selected.id }, (response) => {
                    $('.popup.show .accordion.card-checklist').remove();
                    $('.popup.show .accordion.card-description').after(response);
                    S.kanban.card.checklist.init();
                });
            },

            addItem: function () {
                var container = $('.popup.show .card-checklist');
                container.addClass('expanded');
                S.ajax.post('Cards/NewCheckListItem', { boardId: S.kanban.card.boardId, cardId: S.kanban.card.selected.id }, (response) => {
                    $('.popup.show .accordion.card-checklist .contents').append(response);
                    S.kanban.card.checklist.init();
                });
            },

            deleteItem: function (e) {
                if (!confirm('Do you really want to delete this checklist item? This action cannot be undone.')) { return; }
                var target = $(e.target);
                var item = target.parents('.checklist-item');
                var id = item.attr('data-id');
                var data = {
                    boardId: S.kanban.card.boardId,
                    cardId: S.kanban.card.selected.id,
                    itemId: id
                };
                S.ajax.post('Cards/DeleteCheckListItem', data, (response) => {
                    item.remove();
                });
            },

            checked: function (e) {
                var target = $(e.target);
                var item = target.parents('.checklist-item');
                var id = item.attr('data-id');
                var data = {
                    boardId: S.kanban.card.boardId,
                    cardId: S.kanban.card.selected.id,
                    itemId: id,
                    ischecked: item.find('input[type="checkbox"]')[0].checked === true
                };
                S.ajax.post('Cards/UpdateCheckListItemChecked', data);
            },

            inputItem: function (e) {
                var timer = S.kanban.card.checklist.typingTimer;
                if (timer != null) { clearTimeout(timer); }
                S.kanban.card.checklist.typingTimer = setTimeout(() => {
                    var target = $(e.target);
                    var item = target.parents('.checklist-item');
                    var id = item.attr('data-id');
                    var data = {
                        boardId: S.kanban.card.boardId,
                        cardId: S.kanban.card.selected.id,
                        itemId: id,
                        label: item.find('input[type="text"]').val()
                    };
                    S.ajax.post('Cards/UpdateCheckListItemLabel', data, (response) => {

                    });
                }, 2000);
            },

            blurItemTextbox: function (e) {
                var el = $(e.target);
                el.removeClass('focused');
            },

            drag: { //for checklist items
                dragging: false, timer: null,
                geometry: [],
                current: { itemId: null, item: null, below: true },

                init: function (elems) {
                    let container = '.popup.show .card-checklist .contents';
                    let parent = $(container);
                    parent.addClass('drag');
                    let selItems = container + ' .checklist-item';
                    $(selItems).each(function (i, item) {
                        var cardElem = $(item);
                        S.drag.add(cardElem, cardElem,
                            //onStart  /////////////////////////////////////////////////////////////////////////////////
                            function (item) {
                                this.dragging = true;
                                item.elem.addClass('dragging');
                                $(container).addClass('dragging');

                                //clone item for visual representation
                                let clone = $(item.elem[0].cloneNode(true));
                                clone.addClass('clone');
                                item.elem.addClass('hide');
                                $(container).prepend(clone);
                                this.elem = $(S.drag.item.elem);
                                S.drag.item.elem = clone;
                                clone.find('input[type="text"]').css({ 'cursor': 'move' });

                                //reset classes
                                $(selItems + '.hovering').removeClass('hovering').parent().removeClass('hovering upward downward');

                                //update geometry for items
                                S.kanban.card.checklist.drag.getGeometryForItems();
                            },
                            //onDrag /////////////////////////////////////////////////////////////////////////////////
                            function (item) {
                                //detect where to drop the item
                                if (this.dragging == false) { return; }
                                var current = this.current;
                                var geo = this.geometry;
                                var bounds = { top: item.cursor.y - 5, right: item.cursor.x, bottom: item.cursor.y + 5, left: item.cursor.x };
                                var changed = false;

                                //first, detect which list the cursor is over
                                var found = false;
                                var hovering = $(selItems + '.hovering');
                                hovering.removeClass('hovering');
                                if (geo.items.length > 0) {
                                    for (var y = 0; y < geo.items.length; y++) {
                                        var nextitem = geo.items[y];
                                        var itembounds = { top: nextitem.top - 5, right: nextitem.top + nextitem.width, bottom: nextitem.top + nextitem.height, left: nextitem.left };
                                        if (S.math.intersect(itembounds, bounds)) {
                                            var elem = $(nextitem.elem);
                                            current.itemId = S.util.element.getClassId(nextitem.elem, 'item-');
                                            current.item = elem;
                                            current.list = parent;
                                            elem.addClass('hovering');
                                            parent.addClass('hovering');
                                            changed = true;
                                            if (bounds.top - nextitem.top < nextitem.height / 2) {
                                                //upward drop
                                                if (!parent.hasClass('upward')) {
                                                    parent.addClass('upward').removeClass('downward');
                                                    S.drag.alteredDOM();
                                                }
                                                current.below = false;
                                            } else {
                                                //downward drop
                                                if (!parent.hasClass('downward')) {
                                                    parent.addClass('downward').removeClass('upward');
                                                    S.drag.alteredDOM();
                                                }
                                                current.below = true;
                                            }
                                            found = true;
                                            break;
                                        }
                                    }
                                }

                                if (changed == true) {
                                    S.kanban.card.checklist.drag.getGeometryForItems();
                                }

                                if (found == false && hovering.length > 0) {
                                    hovering.removeClass('hovering');
                                    hovering.parent().removeClass('hovering upward downward');
                                    this.current = { itemId: null, item: null, below: true };
                                    S.drag.alteredDOM();
                                }
                            },
                            //onStop  /////////////////////////////////////////////////////////////////////////////////
                            function (item) {
                                //$('.kanban > .list .items > *').remove();
                                item.elem = $(this.elem);
                                $(container + ' .clone').remove();
                                item.elem.removeClass('dragging hide').css({ 'margin-bottom': '' });
                                parent.removeClass('dragging hovering upward downward');
                                item.elem.css({ top: 0, left: 0 });

                                //move item in DOM to drop area
                                if (this.current.itemId != null) {
                                    if (this.current.below == true) {
                                        //append below item
                                        this.current.item.after(item.elem);
                                    } else {
                                        //append above item
                                        this.current.item.before(item.elem);
                                    }

                                    //send update to server via ajax
                                    var items = [];
                                    var list = $(selItems);
                                    list.each(function (i, item) {
                                        items.push(S.util.element.getClassId(item, 'item-'));
                                    });

                                    S.ajax.post('Card/Kanban/MoveChecklistItem', { boardId: S.board.id, cardId: S.kanban.card.selected.id, itemIds: items });
                                }
                                setTimeout(function () { S.kanban.card.checklist.drag.dragging = false; }, 100);
                            },
                            //onClick  /////////////////////////////////////////////////////////////////////////////////
                            function (item) {
                                item.elem.find('input[type="text"]').addClass('focused')[0].focus();
                            },
                            //options
                            { cancelBubble: false, hideArea: true, hideAreaOffset: 7, useElemPos: false, delay: 500, speed: 1000 / 30, callee: S.kanban.card.checklist.drag }
                        );
                    });
                },

                getGeometryForItems: function () {
                    //get current rectangular geometry for all checklist items
                    var geo = {items:[]};
                    let selItems = '.popup.show .card-checklist .checklist-item:not(.clone):not(.hide)';
                    let parent = $('.popup.show .card-checklist .contents');
                    var cpos = parent.offset();
                    geo.items = $(selItems).map((index, item) => {
                        var pos = $(item).position();
                        var elem = $(item);
                        return {
                            elem: item,
                            top: cpos.top + pos.top,
                            left: cpos.left,
                            right: cpos.left + parent.width(),
                            bottom: cpos.top + elem.height(),
                            width: parent.width(),
                            height: elem.height()
                        }
                    });
                    S.kanban.card.checklist.drag.geometry = geo;
                }
            },
        },

        copy: {
            show: function () {

            },

            cancel: function () {

            }
        },

        move: {
            show: function () {

            },

            cancel: function () {

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
                        $('.popup.show .menu .item-archive').addClass('hide');
                        $('.popup.show .menu .item-restore').removeClass('hide');
                        $('.popup.show .menu .item-delete').removeClass('hide');

                        //remove card from list
                        S.kanban.card.selected.elem.remove();
                        S.kanban.list.resize(S.kanban.card.selected.listId);

                    } else {
                        S.util.message('.board .messages', "error", S.message.error.generic);
                    }
                },
                function () {
                    S.util.message('.board .messages', "error", S.message.error.generic);
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
                        $('.popup.show .menu .item-restore').addClass('hide');
                        $('.popup.show .menu .item-delete').addClass('hide');
                        $('.popup.show .menu .item-archive').removeClass('hide');

                        //add card to list
                        $('.board .list.id-' + S.kanban.card.selected.listId + ' .items').append(d);
                        S.kanban.list.resize(S.kanban.card.selected.listId);

                    } else {
                        S.util.message('.board .messages', "error", S.message.error.generic);
                    }
                },
                function () {
                    S.util.message('.board .messages', "error", S.message.error.generic);
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
                        S.util.message('.board .messages', "alert", 'The selected card has been permanently deleted from this board');
                        S.popup.hide();
                    } else {
                        S.util.message('.board .messages', "error", S.message.error.generic);
                    }
                },
                function () {
                    S.util.message('.board .messages', "error", S.message.error.generic);
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
                            S.util.message('.board .messages', "error", S.message.error.generic);
                        }
                    },
                    function () {
                        S.util.message('.board .messages', "error", S.message.error.generic);
                    }
                );
                if (e.preventDefault) { e.preventDefault(); }
                return false;
            }
        },

        description: {
            cached: null,
            edit: function () {
                var field = $('.popup.show .field-description');
                if (field.hasClass('hide')) {
                    S.kanban.card.description.cached = $('#card_description').val().trim();
                    field.removeClass('hide');
                    $('.popup.show .new-description').addClass('hide');
                    $('.popup.show .description').addClass('hide');
                } else {
                    S.kanban.card.description.update();
                }
                $('.popup.show .card-description').addClass('expanded');
                S.kanban.card.description.resize();
            },

            cancel: function () {
                $('#card_description').val(S.kanban.card.description.cached);
                S.kanban.card.description.markdown();
                S.kanban.card.description.hide();
            },

            hide: function () {
                $('.popup.show .field-description').addClass('hide');
                if ($('.popup.show .description .markdown').html().trim() != '') {
                    $('.popup.show .description').removeClass('hide');
                } else {
                    $('.popup.show .new-description').removeClass('hide');
                }
            },

            markdown: function () {
                var text = $('#card_description').val().trim();
                if (text == '' || text == null) { return; }
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

                $('.popup.show .description .markdown').html(
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
                if (data.description.trim() == S.kanban.card.description.cached.trim()) {
                    //data is identical. cancel ajax and show rendered markdown instead
                    S.kanban.card.description.markdown();
                    S.kanban.card.description.hide();
                    return;
                }
                S.ajax.post('Cards/UpdateDescription', data,
                    function (d) {
                        if (d != '') {
                            //replace existing card with updated card
                            S.kanban.card.replace(d);

                            //update description markdown
                            S.kanban.card.description.markdown();
                            S.kanban.card.description.hide();
                        } else {
                            S.util.message('.board .messages', "error", S.message.error.generic);
                        }
                    },
                    function () {
                        S.util.message('.board .messages', "error", S.message.error.generic);
                    }
                );
                if (e) { e.preventDefault();}
                
                return false;
            },

            resize: function () {
                var textarea = $('.popup.show #card_description');
                var temp = $('.popup.show .card-description .temp');
                temp.html(textarea.val().replace(/\n/g, '<br/>'));
                var pos = temp[0].getBoundingClientRect();
                textarea.css({ 'height': Math.round((pos.height * 1.05) + 20) + 'px' });
            }
        },

        modal: {
            show: function (content) {
                $('.popup.show .card-modals').html(content);
                //$('.popup.show .card-details').css({ 'opacity': 0 });
                $('.popup.show .card-modal-bg').show();
            },

            hide: function (e) {
                if (e != null) {
                    var target = $(e.target);
                    if (target.parents('.btn-cancel').length == 0 && target.parents('.card-modals').length > 0) { return; }
                }

                $('.popup.show .card-modals').html('');
                $('.popup.show .card-details').css({ 'opacity': 1 });
                $('.popup.show .card-modal-bg').hide();
            }
        },

        assignTo: {
            show: function (selectedId) {
                var card = S.kanban.card.selected;
                S.kanban.card.modal.show(temp_assign_to.innerHTML);
                var dropdown = $('.popup.show .assign-to-form #card_assignto');
                dropdown.html('<option>Unassigned</option>');
                //get list of users for dropdown
                S.ajax.post('Cards/GetMembers', { cardId: card.id }, (members) => {
                    for (var x = 0; x < members.length; x++) {
                        dropdown.append('<option value="' + members[x].userId + '"' +
                            (members[x].userId == selectedId ? ' selected' : '') + '> ' + members[x].name + '</option > ');
                    }
                    $('.popup.show #card_assignto').on('input', S.kanban.card.assignTo.submit);
                }, () => { }, true);
                $('.popup.show .assign-to-form .btn-cancel').on('click', () => { S.kanban.card.modal.hide(); });
            },

            submit: function () {
                var card = S.kanban.card.selected;
                var dropdown = $('.popup.show .assign-to-form #card_assignto');
                S.ajax.post('Cards/UpdateAssignedTo', { cardId: card.id, userId: dropdown.val() }, (html) => {
                    S.kanban.card.modal.hide();
                    //update card sub title with new assigned to user
                    if (html.trim().length > 0) {
                        $('.popup.show .assigned-to').html(html).css({ 'display': 'inline-block' });
                        $('.popup.show .not-assigned').hide();
                    } else {
                        $('.popup.show .assigned-to').html('').hide();
                        $('.popup.show .not-assigned').css({ 'display': 'inline-block' });
                    }
                });
            }
        },

        dueDate: {
            show: function () {
                S.kanban.card.modal.show(temp_set_duedate.innerHTML);
                var input = $('.popup.show .duedate-form #card_duedate');
                var duedate = $('.popup.show .has-duedate span').html().replace('Due ', '');
                if (duedate != '') {
                    input[0].valueAsDate = new Date(duedate);
                }
                $('.popup.show #card_duedate').on('change', S.kanban.card.dueDate.submit);
                $('.popup.show .duedate-form .btn-cancel').on('click', () => { S.kanban.card.modal.hide(); });
            },

            submit: function (nodate) {
                var card = S.kanban.card.selected;
                var input = $('.popup.show .duedate-form #card_duedate');
                var duedate = '';
                if (nodate !== true) {
                    var dates = input.val().split('-');
                    if (dates.length > 0) {
                        duedate = dates[1] + '/' + dates[2] + '/' + dates[0];
                    }
                }

                S.ajax.post('Cards/UpdateDueDate', { cardId: card.id, duedate: duedate }, (html) => {
                    S.kanban.card.modal.hide();
                    //update card sub title with new assigned to user
                    if (duedate != '') {
                        $('.popup.show .has-duedate').css({ 'display': 'inline-block' }).find('span').html('Due ' + duedate);
                        $('.popup.show .no-duedate').hide();
                    } else {
                        $('.popup.show .has-duedate').hide().find('span').html('');
                        $('.popup.show .no-duedate').css({ 'display': 'inline-block' });
                    }
                });
            }
        },

        comments: {
            add: {
                show: function (e, bottom) {
                    if (e) { e.cancelBubble = true; }
                    var comments = $('.popup.show .card-comments .contents');
                    comments.find('.add-comment-form').remove();
                    if (bottom === true) {
                        comments.append(temp_add_comment.innerHTML);
                    } else {
                        comments.prepend(temp_add_comment.innerHTML);
                    }
                    $('.popup.show .add-comment-form .cancel').on('click', S.kanban.card.comments.add.hide);
                    $('.popup.show .add-comment-form .apply').on('click', S.kanban.card.comments.add.submit);
                    $('.popup.show #newcomment').on('input', S.kanban.card.comments.add.resize);
                    $('.popup.show .card-comments').addClass('expanded');
                    $('.popup.show .no-comments').hide();
                },

                submit: function () {
                    var card = S.kanban.card.selected;
                    S.ajax.post('Cards/AddComment', { cardId: card.id, comment: $('.popup.show #newcomment').val() }, (html) => {
                        $('.popup.show .comments').append(html);
                        S.kanban.card.comments.add.hide();
                    }, (err) => {
                        S.message.show(null, 'error', 'Could not add comment');
                    });
                },

                hide: function () {
                    $('.popup.show .card-comments .add-comment-form').remove();
                },

                resize: function () {
                    var textarea = $('.popup.show #newcomment');
                    var temp = $('.popup.show .card-comments .temp');
                    temp.html(textarea.val().replace(/\n/g, '<br/>'));
                    var pos = temp[0].getBoundingClientRect();
                    textarea.css({ 'height': Math.round(pos.height + 16) + 'px' });
                }
            },

            edit: {
                show: function (commentId) {
                    var card = S.kanban.card.selected;
                    var comments = $('.popup.show .card-comments .contents');
                    var comment = $('.popup.show .comment-' + commentId);
                    comments.find('.add-comment-form').remove();
                    comment.after(temp_add_comment.innerHTML);
                    comment.hide();
                    $('.popup.show .add-comment-form .field').html('Update Comment');
                    $('.popup.show .add-comment-form .cancel').on('click', () => { S.kanban.card.comments.edit.hide(commentId); });
                    $('.popup.show .add-comment-form .apply').on('click', () => { S.kanban.card.comments.edit.submit(commentId); });
                    $('.popup.show #newcomment').on('input', S.kanban.card.comments.edit.resize);
                    $('.popup.show .card-comments').addClass('expanded');
                    $('.popup.show .no-comments').hide();
                    S.ajax.post('Cards/GetComment', { cardId: card.id, commentId: commentId }, (result) => {
                        $('.popup.show #newcomment').val(result);
                    });
                },

                submit: function (commentId) {
                    var card = S.kanban.card.selected;
                    var comment = $('.popup.show .comment-' + commentId);
                    S.ajax.post('Cards/UpdateComment', { cardId: card.id, commentId: commentId, comment: $('.popup.show #newcomment').val() }, (html) => {
                        comment.before(html);
                        comment.remove();
                        S.kanban.card.comments.add.hide();
                    }, (err) => {
                        S.message.show(null, 'error', 'Could not add comment');
                    });
                },

                hide: function (commentId) {
                    $('.popup.show .card-comments .add-comment-form').remove();
                    var comment = $('.popup.show .comment-' + commentId);
                    comment.show();
                },

                resize: function () {
                    var textarea = $('.popup.show #newcomment');
                    var temp = $('.popup.show .card-comments .temp');
                    temp.html(textarea.val().replace(/\n/g, '<br/>'));
                    var pos = temp[0].getBoundingClientRect();
                    textarea.css({ 'height': Math.round(pos.height + 16) + 'px' });
                }
            },

            delete: function (commentId) {
                if (!confirm('Do you really want to delete the selected comment? This cannot be undone.')) { return; }
                var card = S.kanban.card.selected;
                var comment = $('.popup.show .comment-' + commentId);
                S.ajax.post('Cards/DeleteComment', { cardId: card.id, commentId: commentId }, (html) => {
                    comment.remove();
                    S.kanban.card.comments.add.hide();
                }, (err) => {
                    S.message.show(null, 'error', 'Could not remove comment');
                });
            },

            flag: function (commentId) {
                if (!confirm('Do you want to flag the selected comment for inappropriate behavior?')) { return; }
                var card = S.kanban.card.selected;
                S.ajax.post('Cards/FlagComment', { cardId: card.id, commentId: commentId }, (html) => {
                    $('.popup.show .comment-' + commentId + ' .comment-flag').html('flagged')
                        .removeClass('comment-flag').addClass('comment-flagged');
                    S.kanban.card.comments.add.hide();
                }, (err) => {
                    S.message.show(null, 'error', 'Could not flag comment');
                });
            },


        },

        share: {
            timer: null,

            show: function () {
                S.kanban.card.modal.show(temp_share.innerHTML
                    .replace(/\#url\#/g, window.location.href.split('#')[0] + '#card=' + S.kanban.card.selected.id)
                );
                $('.popup.show #share_name').on('input', S.kanban.card.share.search);
                $('.popup.show .share-form .btn-send').on('click', S.kanban.card.share.submit);
                $('.popup.show .share-form .btn-cancel').on('click', () => { S.kanban.card.modal.hide(); });
            },

            search: function () {
                if (S.kanban.card.share.timer == null) {
                    var container = $('.popup.show .share-form .search-results');
                    container.html('<div class="row no-results">waiting...</div>');
                    container.show();
                }
                if (S.kanban.card.share.timer != null) {
                    clearTimeout(S.kanban.card.share.timer);
                }
                S.kanban.card.share.timer = setTimeout(S.kanban.card.share.endWait, 500);
            },

            endWait: function () {
                //executed after user is done typing in the search field
                var card = S.kanban.card.selected;
                var search = $('.popup.show #share_name').val();
                var container = $('.popup.show .share-form .search-results');
                container.html('<div class="row no-results">searching...</div>');
                if (search.length == 0) {
                    container.hide();
                    return;
                }
                if (search.length < 3) {
                    return;
                }
                S.ajax.post('Cards/FindInvites', { cardId: card.id, search: search }, (results) => {
                    if (results.length > 0) {
                        container.html('');
                        results.forEach(a => {
                            container.append('<div class="row hover result" data-id="' + a.id + '"><span>' + a.name + '</span></div>');
                        });
                        $('.popup.show .result').on('click', S.kanban.card.share.selectResult);
                    } else {
                        container.html('<div class="row no-results">No members were found that match your query</div>');
                    }
                }, () => { }, true);
            },

            selectResult: function (e) {
                S.kanban.card.share.timer = null;
                var target = $(e.target);
                if (!target.hasClass('result')) {
                    target = target.parents('.result').first();
                }
                if (target.length > 0) {
                    var id = target.attr('data-id');
                    var name = target.find('span').html();
                    S.kanban.card.share.createInvite(id, name);
                }
                S.kanban.card.share.hideResults();
            },

            createInvite: function (id, name) {
                $('.popup.show .share-form .invited-users').append(temp_invited_user.innerHTML
                    .replace(/\#id\#/g, id)
                    .replace(/\#name\#/g, name)
                );
            },

            hideResults: function () {
                $('.popup.show #share_name').val('');
                var container = $('.popup.show .share-form .search-results');
                container.hide();
            },

            submit: function () {
                var card = S.kanban.card.selected;
                var invited = $('.popup.show .invited-users .invited');
                S.ajax.post('Cards/BatchInvite', {
                    cardId: card.id,
                    invites: invited.map((i, a) => {
                        var el = $(a);
                        var id = el.attr('data-id');
                        if (id == '0') {
                            return el.attr('title');
                        }
                        return id;
                    }).join(','),
                    canupdate: invite_canupdate.checked ? true : false,
                    canpostcomment: invite_cancomment.checked ? true : false
                }, () => {
                    S.message.show('.popup.show .messages.for-card', '', 'Invited ' +
                        invited.length + (invited.length > 1 ? ' people' : ' person') + ' to view this card');
                    S.kanban.card.modal.hide();
                }, (err) => {
                    S.message.show('.popup.show .share-form .messages', 'error', err.responseText);
                });
            }
        },

        menu: {
            show: function () {
                var menu = $('.popup.show .icon-dots .menu');
                menu.show();
                function bodyclick(e) {
                    var target = $(e.target);
                    if (!target.hasClass('icon-dots') && target.parents('.icon-dots').length == 0) {
                        menu.hide();
                        $('body').off('click', bodyclick);
                    }
                }
                $('body').on('click', bodyclick);
            },
            hide: function () {
                var menu = $('.popup.show .icon-dots .menu');
                menu.hide();
            }
        },

        layout: function (type) {
            S.kanban.card.menu.hide();
            var layout = type;
            var layouts = S.kanban.card.layouts;
            var popup = $('.popup.show');
            if (layout == null || layout == '') { layout = 'center'; }
            var selected = layouts.filter(a => a.name == layout)[0];
            S.kanban.card.currentLayout = selected;
            if ($('body').hasClass('card-' + selected.name)) { return;}
            $('body').removeClass('card-center card-rightside card-leftside card-fullscreen').addClass('card-' + selected.name);
            popup.removeClass('pos-center pos-rightside pos-leftside pos-fullscreen').addClass('pos-' + selected.name);
            switch (type) {
                case 'center':
                    $('.bg.for-popup').removeClass('disabled');
                    break;
                case 'leftside': case 'rightside': case 'fullscreen':
                    $('.bg.for-popup').addClass('disabled');
                    break;
            }
            S.popup.resize();
            S.kanban.scroll.resize();
        },

        attachments: {
            init: function() {
                $('.attachments .attachments-link').on('click', S.kanban.card.attachments.show);
                S.kanban.card.attachments.hide();
            },

            show: function () {
                S.kanban.card.menu.hide();
                //hide menu item (if neccessary)
                var menu = $('.popup.show .icon-dots .menu .btn-upload-files').parents('li').first();
                if (menu.length > 0) {
                    menu.next().remove();
                    menu.remove();
                }
                var div = document.createElement('div');
                div.className = 'upload-bg';
                div.innerHTML = $('#template_upload_modal').html()
                    .replace('#card-name#', S.kanban.card.selected.name);
                $('body').prepend(div);
                //button events
                $('.upload-modal > .icon-close').on('click', S.kanban.card.attachments.hide);
                $('.upload-modal #uploadfiles').on('change', S.kanban.card.attachments.upload);
            },

            hide: function () {
                $('body > .upload-bg').remove();
            },

            upload: function (e) {
                var input = uploadfiles;
                if (input.files && input.files.length > 0) {
                    var files = input.files;
                    var len = files.length;
                    var done = 0;
                    var filenames = [];
                    for (var x = 0; x < files.length; x++) {
                        var xhr = new XMLHttpRequest();
                        var file = files[x];

                        xhr.open('POST', '/upload?cardId=' + S.kanban.card.selected.id, false);

                        xhr.onload = function () {
                            done++;
                            if (xhr.status >= 200 && xhr.status < 400) {
                                //request success
                                console.log(xhr.responseText);
                            }
                            filenames = [...filenames, ...JSON.parse(xhr.responseText)];
                            if (done == len) {
                                //complete upload process
                                var data = {
                                    cardId: S.kanban.card.selected.id,
                                    filenames: filenames.map(a => a.Name)
                                };
                                S.ajax.post('Cards/AddAttachments', data, (response) => {
                                    $('.popup.show .attachments').html(response);
                                    S.kanban.card.attachments.init();
                                });
                            }
                        };

                        console.log('sending file...');
                        var formData = new FormData();
                        formData.append("file", file);
                        xhr.send(formData);
                    }
                }
            },

            reload: function () {
                //reload attachments accordion
                S.ajax.post('/Cards/GetAttachments', { cardId: S.kanban.card.selected.id }, (response) => {
                    $('.popup.show .attachments').html(response);
                    S.kanban.card.attachments.init();
                });
            }
        }
    },

    browser: {
        list: {
            show: function () {
                //allow user to select a list from any board across all 
                //organizations that they have access to
            }
        }
    }
};

S.kanban.init();