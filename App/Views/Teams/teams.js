S.teams = {
    add: {
        orgId: null,
        popup: null,
        callback: null,

        show: function (id, orgId, callback) {
            S.teams.add.orgId = orgId;
            S.teams.add.callback = callback;
            S.ajax.post('Teams/RenderForm', { teamId: id, orgId: orgId }, (html) => {
                S.teams.add.popup = S.popup.show(!id ? 'Create A New Team' : 'Edit Team', html, {
                    width: 430,
                    onClose: function () {
                        if (callback) { callback(); }
                    }
                });
            });
        },

        hide: function () {
            S.popup.hide(S.teams.add.popup);
        },

        submit: function () {
            var name = $('#teamname').val();
            var description = $('#description').val() || '';
            var msg = $('.popup.show .message');
            if (name == '' || name == null) {
                S.message.show(msg, 'error', 'Please specify a team name');
                return;
            }
            S.ajax.post('Teams/Create', { orgId: S.teams.add.orgId, name: name, description: description },
                function (data) {
                    S.popup.hide(S.teams.add.popup);
                    if (S.teams.add.callback) {
                        S.teams.add.callback();
                        S.teams.events.broadcast('team-added');
                    }
                },
                function () {
                    S.message.show(msg, 'error', S.message.error.generic);
                    return;
                }
            );
        }
    },

    details: {
        popup: null,
        teamId: null,
        name:'',
        orgId: null,
        callback: null,

        show: function (id, orgId, name, callback) {
            S.teams.details.teamId = id;
            S.teams.details.orgId = orgId;
            S.teams.details.name = name;
            S.teams.details.callback = callback;
            if (typeof name == 'function') { name = name(id, orgId); }
            S.ajax.post('Teams/Details', {teamId: id}, function (result) {
                S.teams.details.popup = S.popup.show('Team ' + name, result, {
                    width: 700,
                    onClose: function () {
                        if (callback) { callback(); }
                    }
                });
                //add events to fields
                $('.team-form input').on('keyup, change', () => {
                    $('.team-form a.apply').removeClass('hide');
                });

                //save button
                $('.team-form a.apply').on('click', S.teams.details.save);

                //set up tabs
                $('.team-details .movable > div').hide();
                $('.team-details .content-members').show();
                $('.team-details .tab-members').on('click', S.teams.members.show);

                //set up custom scrollbars
                S.scrollbar.add('.team-details .tab-content', { touch: true });

                //update members tab
                S.teams.members.updateEvents();
            });
        },

        editInfo: function () {
            $('.team-details .team-form').removeClass('hide');
            $('.team-details .team-info').hide();
            $('.team-edit-link').hide();
        },

        cancelEdit: function () {
            $('.team-details .team-form').addClass('hide');
            $('.team-details .team-info').show();
            $('.team-edit-link').show();
        },

        hide: function () {
            S.popup.hide(S.teams.details.popup);
        },

        save: function () {
            var form = {
                teamId: S.teams.details.teamId,
                name: $('#teamname').val(),
                description: $('#team_description').val(),
            };
            var msg = $('.popup.show .message');
            S.ajax.post('Teams/Update', form, function () {
                $('.popup.show .title h5').html('Team ' + form.name);
                if (S.teams.details.callback) {
                    S.teams.details.callback('saved');
                }
                $('.team-details .team-description').html(form.description);
                S.teams.details.cancelEdit();
                S.message.show(msg, null, 'Team updated successfully');
            }, (err) => {
                S.message.show(msg, 'error', err.responseText);
            });
        },

        tabs: {
            select: function (id) {
                $('.team-details .tabs > .tab').removeClass('selected');
                $('.team-details .tabs > .tab-' + id).addClass('selected');
                $('.team-details > .tab-content > .movable > div').hide();
                $('.team-details .content-' + id).show();
                S.popup.resize();
            }
        }
    },

    events: {
        callbacks: [],

        listen: function (callback) {
            S.teams.events.callbacks.push(callback);
        },

        broadcast: function (action, params) {
            var c = S.teams.events.callbacks;
            for (var x = 0; x < c.length; x++) {
                c[x](action, params);
            }
        }
    },

    members: {
        show: function () {
            S.teams.details.tabs.select('members');
            var content = $('.team-details .content-members');
            if (content.html().trim() == '') {
                //load security groups
                S.teams.members.refresh();
            }
        },

        refresh: function () {
            S.ajax.post('Teams/RefreshMembers', { orgId: S.orgs.details.orgId }, function (result) {
                $('.team-details .content-members').html(result);
                S.teams.members.updateEvents();
            },
            (err) => {

            });
        },

        updateEvents: function () {
            $('.team-details .btn-add-member').on('click', () => {
                S.teams.details.popup.hide();
                S.teams.invite.show(S.teams.details.orgId, S.teams.details.teamId, S.teams.details.name, (total, failed) => {
                    S.teams.details.popup.show();
                    S.message.show('.popup.show .message', '', total + (total != 1 ? ' people' : ' person') + ' invited' +
                        (failed != null && failed != '' ? ', ' + failed + ' could not be invited' : ''));
                });
            });
            S.popup.resize();
        }
    },

    invite: {
        orgId: null,
        teamId: null,
        popup: null,
        callback: null,
        cache: null,

        show: function (orgId, teamId, teamName, callback) {
            S.teams.invite.orgId = orgId;
            S.teams.invite.teamId = teamId;
            S.teams.invite.callback = callback;
            S.ajax.post('Teams/RenderInviteForm', { teamId: teamId, orgId: orgId }, (html) => {
                S.teams.invite.popup = S.popup.show('Invite People to Team ' + teamName, html, {
                    width: 700,
                    onClose: function () {
                        if (callback) { callback(); }
                    }
                });
                //initialize the member search form
                S.teams.invite.query(orgId, teamId, 1, 0, '');

                //hide submit button until user selects at least 1 member
                $('.invite-form a.apply').hide();
            });
        },

        hide: function () {
            S.popup.hide(S.teams.invite.popup);
        },

        query: function (orgId, teamId, page, length, search) {
            S.teams.invite.cache = { page: page, length: length, search: search };
            S.ajax.post('Teams/RefreshInviteList', { orgId: orgId, teamId: teamId, page: page, length: length, search: search }, (html) => {
                var inviteform = $('.invite-form .search-users');
                if (length == 0) {
                    inviteform.html(html);

                    //add invited section to form
                    $('.invite-form .members-list').after(
                        '<h5 class="invited-title hide">Selected Invites</h5>' +
                        '<div class="invited-list grid-items hide"></div>');
                    var win = { h: window.innerHeight };
                    $('.invite-form .invited-list, .invite-form .members-list').css({ 'max-height': ((win.h) / 3) + 'px' });

                    //when user searches
                    $('.invite-form form').on('submit', (e) => {
                        e.preventDefault();
                        S.teams.invite.query(orgId, teamId, 1, 20, $('.invite-form #search_members').val());
                        return false;
                    });
                    $('.invite-form .browse-all').attr('onclick', '').on('click', () => {
                        S.teams.invite.query(orgId, teamId, 1, 20, '');
                    });
                } else {
                    $('.invite-form .members-list').html(html);
                    //move results
                    var results = $('.invite-form .members-list .results');
                    var info = $('.invite-form .search-members form > .info');
                    info.html(results[0].outerHTML);
                    results.remove();
                    S.teams.invite.hideSelectedItems();
                }

                //change all item onclick attributes
                inviteform.find('.item > a').each((i, a) => {
                    a.setAttribute('onclick', a.getAttribute('onclick').replace('S.members.details.show(', 'S.teams.invite.select(event, '));
                });
            });
        },

        select: function (e, id) {
            S.ajax.post('Teams/RenderMemberSelectedItem', { orgId: S.teams.invite.orgId, userId: id }, (html) => {
                $('.invited-list').removeClass('hide').prepend(html);
                $('.invited-title').removeClass('hide');
                S.teams.invite.hideSelectedItems();
                S.popup.resize(S.teams.invite.popup);
            });
        },

        selectEmail: function (e, email) {
            //add email to the list of members
            S.ajax.post('Teams/RenderEmailSelectedItem', { orgId: S.teams.invite.orgId, email:email }, (html) => {
                $('.invited-list').removeClass('hide').append(html);
                $('.invited-title').removeClass('hide');
                S.teams.invite.hideSelectedItems();
                S.popup.resize(S.teams.invite.popup);
            });
            $('.invite-form #search_members')[0].value = '';
        },

        hideSelectedItems: function () {
            $('.invite-form .members-list .item').show();
            $('.invited-list .item').each((i, a) => {
                $('.invite-form .members-list .item[data-id="' + a.getAttribute('data-id') + '"]').hide();
            });
            if ($('.invited-list .item').length == 0) {
                $('.invite-form > .buttons a.apply').hide();
            } else {
                $('.invite-form > .buttons a.apply').css({ 'display': 'inline-block' });
            }
        },

        removeSelected: function (e, id) {
            $('.invited-list .item.item-' + id).remove();
            S.teams.invite.hideSelectedItems();
            S.popup.resize(S.teams.invite.popup);
        },

        removeSelectedEmail: function (e) {
            var target = $(e.target);
            if (!target.hasClass('item')) {
                target = target.parents('.item').first();
            }
            target.remove();
            S.teams.invite.hideSelectedItems();
            S.popup.resize(S.teams.invite.popup);
        },

        submit: function () {
            var data = {
                orgId: S.teams.invite.orgId,
                teamId: S.teams.invite.teamId,
                people: $('.invited-list .item').map((i, a) => a.getAttribute('data-id'))
            };
            S.ajax.post('Teams/InvitePeople', data,
                function (response) {
                    S.popup.hide(S.teams.invite.popup);
                    if (S.teams.invite.callback) {
                        S.teams.invite.callback(data.people.length);
                        S.teams.events.broadcast('people-invited');
                    }
                },
                function () {
                    S.message.show(msg, 'error', S.message.error.generic);
                    return;
                }
            );
        }
    }
}