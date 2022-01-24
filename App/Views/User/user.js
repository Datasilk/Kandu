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
                $('.user-details .tab-teams').on('click', S.user.teams.show);
                $('.user-details .tab-account').on('click', S.user.account.show);
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
            var content = $('.user-details .content-boards');
            if (content.html().trim() == '') {
                //load user account
                S.user.boards.refresh();
            }
        },

        refresh: function () {
            S.ajax.post('User/RefreshBoards', { userId: S.user.details.userId }, function (result) {
                var content = $('.user-details .content-boards');
                content.html(result);
                S.popup.resize();
            },
                (err) => {

                });
        },
    },

    teams: {
        show: function () {
            var content = $('.user-details .content-teams');
            if (content.html().trim() == '') {
                //load user account
                S.user.teams.refresh();
            }
        },

        refresh: function () {
            S.ajax.post('User/RefreshTeams', { userId: S.user.details.userId }, function (result) {
                var content = $('.user-details .content-teams');
                content.html(result);
                S.popup.resize();
            },
            (err) => {

            });
        },

        details: function (orgId, teamId, title) {
            S.user.details.popup.hide();
            S.teams.details.show(teamId, orgId, title, () => {
                S.user.details.popup.show();
            });
        }
    },

    account: {
        show: function () {
            var content = $('.user-details .content-account');
            if (content.html().trim() == '') {
                //load user account
                S.user.account.refresh();
            }
        },

        refresh: function () {
            S.ajax.post('User/RefreshAccount', { orgId: S.orgs.details.orgId, userId: S.user.details.userId }, function (result) {
                var content = $('.user-details .content-account');
                content.html(result);
                var savebtn = $('.user-details .btn-save-account');
                content.find('select, input').on('keyup, change', () => {
                    savebtn.removeClass('hide');
                });
                S.popup.resize();
            },
            (err) => {

            });
        },

        changeEmail: function () {
            $('.change-email').hide();
            $('.update-email').removeClass('hide');
            $('.current-pass').removeClass('hide');

        },

        changePass: function () {
            $('.change-pass').hide();
            $('.new-pass').removeClass('hide');
        },

        cancelEmail: function () {
            $('.change-email').show();
            $('.update-email').addClass('hide');
            if ($('.new-pass').hasClass('hide')) {
                $('.current-pass').addClass('hide');
            }
        },

        cancelPass: function () {
            $('.change-pass').show();
            $('.new-pass').addClass('hide');
            if (!$('.update-email').hasClass('hide')) {
                $('.current-pass').removeClass('hide');
            }
        },

        save: function () {
            var msg = $('.popup.show .message');
            var data = {
                userId: S.user.details.userId,
                name: $('#user_name').val(),
                email: $('#user_email').val(),
                oldpass: $('#oldpass').val(),
                newpass1: $('#newpass1').val(),
                newpass2: $('#newpass2').val()
            }
            S.ajax.post('User/UpdateInfo', data, 
                function (data) {
                    S.message.show(msg, null, 'Account settings were updated successfully');
                },
                function (e) {
                    S.message.show(msg, 'error', e.responseText);
                    return;
                }
            );
        }
    },

    security: {
        show: function () {
            var content = $('.user-details .content-security');
            console.log(content);
            if (content.html().trim() == '') {
                //load security groups
                S.user.security.refresh();
            }
        },

        refresh: function () {
            S.ajax.post('SecurityGroups/RefreshListForUser', { userId: S.user.details.userId }, function (result) {
                $('.user-details .content-security').html(result);
                S.popup.resize();
            },
            (err) => {

            });
        },

        details: function (groupId, orgId, name) {
            S.user.details.popup.hide();
            S.security.details.show(groupId, orgId, name, () => {
                S.user.details.popup.show();
            });
        }
    },

    email: {
        show: function () {
            var content = $('.user-details .content-email-settings');
            console.log(content);
            if (content.html().trim() == '') {
                //load email settings
                S.user.email.refresh();
            }
        },

        refresh: function () {
            S.ajax.post('User/RefreshEmailSettings', { userId: S.user.details.userId }, function (result) {
                $('.user-details .content-email-settings').html(result);
                S.popup.resize();
            },
                (err) => {

                });
        },
    },
};