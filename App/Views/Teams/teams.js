S.teams = {
    add: {
        orgId: null,

        show: function (orgId) {
            $('.popup > .row > .col > h4').html('Create A New Team');
            $('.board-form, .color-picker').hide();
            $('.team-form').show();
            S.teams.add.orgId = orgId;
        },

        hide: function () {
            $('.popup > .row > .col > h4').html('Create A New Board');
            $('.team-form').hide();
            $('.board-form').show();
        },

        submit: function () {
            var name = $('#teamname').val();
            var description = $('#description').val() || '';
            var msg = $('.popup .message');
            if (name == '' || name == null) {
                S.message.show(msg, 'error', 'Please specify a team name');
                return;
            }
            S.ajax.post('Teams/Create', { orgId: S.teams.add.orgId, name: name, description: description },
                function (data) {
                    if (data.indexOf('success') == 0) {
                        S.boards.updateTeamList(S.teams.add.orgId);
                        S.teams.add.hide();
                    } else {
                        S.message.show(msg, 'error', S.message.error.generic);
                        return;
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