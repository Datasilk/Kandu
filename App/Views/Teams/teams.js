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
            S.teams.add.popup = S.popup.show(!id ? 'Create A New Team' : 'Edit Team', view.render(), { width: 430 });
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
}