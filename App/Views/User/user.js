S.user = {
    details: {
        popup: null,
        userId: null,
        name: '',
        orgId: null,
        callback: null,

        show: function (id, orgId, name, callback) {
            S.user.details.userId = id;
            S.user.details.orgId = orgId;
            S.user.details.name = name;
            S.user.details.callback = callback;
            if (typeof name == 'function') { name = name(id, orgId); }
            S.ajax.post('User/Details', { userId: id, orgId: orgId }, function (result) {
                S.user.details.popup = S.popup.show(name, result, {
                    width: 700,
                    onClose: function () {
                        if (callback) { callback(); }
                    }
                });


                //add events to fields
                $('.user-form input').on('keyup, change', () => {
                    $('.user-form a.apply').removeClass('hide');
                });

                //save button
                $('.user-form a.apply').on('click', S.user.details.save);

                //set up tabs
                $('.user-details .movable > div').hide();
                $('.user-details .content-cards').show();
                $('.user-details .tab-boards').on('click', S.user.boards.show);
                $('.user-details .tab-orgs').on('click', S.user.orgs.show);
                $('.user-details .tab-security').on('click', S.user.security.show);
                $('.user-details .tab-email-settings').on('click', S.user.email.show);

                //set up custom scrollbars
                S.scrollbar.add('.user-details .tab-content', { touch: true });

                //load kanban css & js for card styling & card details
                S.boards.loadKanbanCss();
                S.boards.loadKanbanJs(() => {
                    S.user.cards.init();
                });
            });
        },

        editInfo: function () {
            $('.user-details .user-form').removeClass('hide');
            $('.user-details .user-info').hide();
            $('.user-edit-link').hide();
        },

        cancelEdit: function () {
            $('.user-details .user-form').addClass('hide');
            $('.user-details .user-info').show();
            $('.user-edit-link').show();
        },

        hide: function () {
            S.popup.hide(S.user.details.popup);
        },

        save: function () {
            var form = {
                userId: S.user.details.userId,
                name: $('#displayname').val(),
            };
            var msg = $('.popup.show .message');
            S.ajax.post('User/Update', form, function () {
                $('.popup.show .title h5').html(form.name);
                if (S.user.details.callback) {
                    S.user.details.callback('saved');
                }
                S.user.details.cancelEdit();
            }, (err) => {
                S.message.show(msg, 'error', err.responseText);
            });
        },

        tabs: {
            select: function (id) {
                $('.user-details .tabs > .tab').removeClass('selected');
                $('.user-details .tabs > .tab-' + id).addClass('selected');
                $('.user-details > .tab-content > .movable > div').hide();
                $('.user-details .content-' + id).show();
                S.popup.resize();
            }
        }
    },

    cards: {
        init: function () {
            //add click event to cards
            $('.user-details .content-cards .item').on('click', (e) => {
                S.user.details.popup.hide();
                S.kanban.card.details(e, () => {
                    S.user.details.popup.show();
                    S.popup.resize();
                });
            });
        }
    },

    boards: {
        show: function () {

        }
    },

    orgs: {
        show: function () {

        }
    },

    security: {
        show: function () {

        }
    },

    email: {
        show: function () {

        }
    },
};