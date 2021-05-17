//NOTE: S.boards is available from all dashboard pages

S.boards = {
    add: {
        show: function (e, id) {
            //get team list
            var hasid = false;
            if (id > 0) { hasid = true;}
            S.ajax.post('Organizations/List', { security: 'board-create' }, function (data) {
                console.log(data);
                var view = new S.view(
                    $('#template_newboard').html()
                        .replace('#org-options#',
                        data.orgs.map(a => {
                            return '<option value="' + a.orgId + '">' + a.name + '</option>';
                        }).join(''))
                        .replace('#color#', '#0094ff')
                        .replace('#submit-label#', !hasid ? 'Create Board' : 'Update Board')
                        .replace('#submit-click#', !hasid ? 'S.boards.add.submit()' : 'S.boards.add.submit(\'' + id + '\')')
                    , {});
                S.popup.show(!hasid ? 'Create A New Board' : 'Edit Board Settings', view.render(), { width: 430 });

                //load board details if id is supplied
                if (hasid) {
                    S.ajax.post('Boards/Details', { boardId: id }, function (data) {
                        $('#boardname').val(data.board.name);
                        $('.popup .color-input').css({ 'background-color': data.board.color });
                        $('#boardteam').val(data.board.teamId);
                    }, null, true);
                }
            }, null, true);
            if (e) { e.cancelBubble = true;}
            return false;
        },

        submit: function (id) {
            var hasid = false;
            var name = $('#boardname').val();
            var color = S.util.color.rgbToHex($('.popup .color-input').css('background-color')).replace('#','');
            var orgId = $('#orgId').val();
            var msg = $('.popup .message');
            if (id > 0) { hasid = true;}
            if (name == '' || name == null) {
                S.message.show(msg, 'error', 'Please specify a board name');
                return;
            }
            var form = { name: name, color: color, orgId: orgId };
            if (hasid) { form.boardId = id;}
            S.ajax.post(hasid ? 'Boards/Update' : 'Boards/Create', form,
                function (data) {
                    if (data == 'success') {
                        window.location.reload();
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
    },

    updateTeamList: function () {
        S.ajax.post('Teams/List', {}, function (data) {
            $('#boardteam').html(
                data.teams.map(a => {
                    return '<option value="' + a.teamId + '">' + a.name + '</option>';
                }).join('')
            );
        }, null, true);
    },

    colorPicker: {
        callback:null,
        show: function (callback) {
            $('.board-form').hide();
            $('.color-picker').show();
            $('.color-picker .color').on('click', S.boards.colorPicker.select);
            S.boards.colorPicker.callback = callback;
        },

        hide: function () {
            $('.color-picker .color').off('click', S.boards.colorPicker.select);
            $('.color-picker').hide();
            $('.board-form').show();
        },

        select: function (e) {
            console.log(e);
            var elem = e.target;
            console.log('---------------------------------------------------');
            console.log($(elem).css('background-color'));
            var color = S.util.color.rgbToHex($(elem).css('background-color'));
            $('.color-input').css({ 'background-color': color });
            S.boards.colorPicker.hide();
            if (typeof S.boards.colorPicker.callback == 'function') {
                S.boards.colorPicker.callback(color);
            }
        }
    }
};

S.teams = {
    add: {
        show: function () {
            $('.popup > .row > .col > h4').html('Create A New Team');
            $('.board-form, .color-picker').hide();
            $('.team-form').show();
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
            S.ajax.post('Teams/Create', { name: name, description: description },
                function (data) {
                    if (data.indexOf('success') == 0) {
                        S.boards.updateTeamList();
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
$('.boards .board.create-new').on('click', S.boards.add.show);