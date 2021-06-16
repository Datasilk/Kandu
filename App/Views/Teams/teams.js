S.teams = {
    add: {
        orgId: null,
        popup: null,
        callback: null,

        show: function (id, orgId, callback) {
            S.teams.add.orgId = orgId;
            S.teams.add.callback = callback;
            var view = new S.view(
                $('#template_newteam').html()
                    .replace('#submit-label#', !id ? 'Create Team' : 'Update Team')
                    .replace('#submit-click#', !id ? 'S.teams.add.submit()' : 'S.teams.add.submit(\'' + id + '\')')
                , {});
            S.teams.add.popup = S.popup.show(!id ? 'Create A New Team' : 'Edit Team', view.render(), {
                width: 430,
                onClose: function () {
                if (callback) { callback(); }
            }});
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
                    S.teams.add.hide();
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
        orgId: null,
        callback: null,

        show: function (id, orgId, name, callback) {
            S.teams.details.teamId = id;
            S.teams.details.orgId = orgId;
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
                $('.team-details .content-boards').show();
                $('.team-details .tab-teams').on('click', S.orgs.teams.show);

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
            S.boards.events.callbacks.push(callback);
        },

        broadcast: function (action, params) {
            var c = S.boards.events.callbacks;
            for (var x = 0; x < c.length; x++) {
                c[x](action, params);
            }
        }
    },

    members: {
        details: function () {

        }
    }
}