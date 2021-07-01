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

                //btn add member
                $('.team-details .btn-add-member').on('click', () => {
                    S.teams.details.popup.hide();
                    S.teams.members.add.show(orgId, id, name, () => {
                        S.teams.details.popup.show();
                    });
                });

                //save button
                $('.team-form a.apply').on('click', S.teams.details.save);

                //set up tabs
                $('.team-details .movable > div').hide();
                $('.team-details .content-members').show();
                $('.team-details .tab-members').on('click', S.teams.members.show);

                //set up custom scrollbars
                S.scrollbar.add('.team-details .tab-content', { touch: true });
            });
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
                $('.team-details .btn-add-member').on('click', () => {
                    S.teams.details.popup.hide();
                    S.teams.members.add.show(S.orgs.details.orgId, S.orgs.details.teamId, S.orgs.details.name, () => {
                        S.teams.details.popup.show();
                    });
                });
                S.popup.resize();
            },
            (err) => {

            });
        },

        add: {
            orgId: null,
            popup: null,
            callback: null,
            cache: null,

            show: function (orgId, teamId, teamName, callback) {
                S.teams.members.add.orgId = orgId;
                S.teams.members.add.callback = callback;
                S.ajax.post('Teams/RenderInviteForm', { teamId: teamId, orgId: orgId }, (html) => {
                    S.teams.members.add.popup = S.popup.show('Invite People to Team ' + teamName, html, {
                        width: 700,
                        onClose: function () {
                            if (callback) { callback(); }
                        }
                    });
                    //initialize the member search form
                    S.teams.members.add.query(orgId, teamId, 1, 0, '');

                    //hide submit button until user selects at least 1 member
                    $('.team-members-form a.apply').hide();
                });
            },

            hide: function () {
                S.popup.hide(S.teams.members.add.popup);
            },

            query: function (orgId, teamId, page, length, search) {
                S.teams.members.add.cache = { page: page, length: length, search: search };
                S.ajax.post('Teams/RefreshInviteList', { orgId: orgId, page: page, length: length, search: search, excludeTeamId: teamId }, (html) => {
                    if (length == 0) {
                        var memberform = $('.team-members-form .search-users');
                        memberform.html(html);

                        //change all item onclick attributes
                        memberform.find('.item > a').foreach((i, a) => {
                            a.setAttribute('onclick', a.getAttribute('onclick').replace('S.members.details.show(', 'S.teams.members.select(event, '));
                        });

                        //when user searches
                        $('.team-members-form .search-members form').on('submit', (e) => {
                            e.preventDefault();
                            S.teams.members.add.query(orgId, teamId, 1, 20, $('.team-members-form #search_members').val());
                            return false;
                        });
                        $('.team-members-form .browse-all').attr('onclick', '').on('click', () => {
                            S.teams.members.add.query(orgId, teamId, 1, 20, '');
                        });
                    } else {
                        $('.team-members-form .search-users .members-list').html(html);
                    }
                });
            },

            selectEmail: function (e, email) {
                //add email to the list of members

            },

            submit: function () {
                var name = $('#teamname').val();
                var description = $('#description').val() || '';
                var msg = $('.popup.show .message');
                if (name == '' || name == null) {
                    S.message.show(msg, 'error', 'Please specify a team name');
                    return;
                }
                S.ajax.post('Teams/Create', { orgId: S.teams.members.add.orgId, name: name, description: description },
                    function (data) {
                        S.popup.hide(S.teams.members.add.popup);
                        if (S.teams.members.add.callback) {
                            S.teams.members.add.callback();
                            S.teams.events.broadcast('team-added');
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
}