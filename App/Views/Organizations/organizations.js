//NOTE: S.org is available from all dashboard pages

S.orgs = {
    list: {
        show: function () {
            $('.orgs-list-menu').removeClass('hide');
        },

        hide: function () {
            $('.orgs-list-menu').addClass('hide');
        }
    },
    add: {
        show: function (e, id) {
            var hasid = false;
            if (id > 0) { hasid = true; }
            var view = new S.view(
                $('#template_neworg').html()
                    .replace('#submit-label#', !hasid ? 'Create Organization' : 'Update Organization')
                    .replace('#submit-click#', !hasid ? 'S.orgs.add.submit()' : 'S.orgs.add.submit(\'' + id + '\')')
                , {});
            S.popup.show(!hasid ? 'Create A New Organization' : 'Edit Organization Settings', view.render(), { width: 430 });

            //load organization details if id is supplied
            if (hasid) {
                S.ajax.post('Organization/Details', { orgId: id }, function (data) {
                    $('#orgname').val(data.org.name);
                    $('#org_description').val(data.org.description);
                    $('#org_website').val(data.org.website);
                }, null, true);
            }
            if (e) { e.cancelBubble = true; }
            return false;
        },

        submit: function (id) {
            var hasid = false;
            var name = $('#boardname').val();
            var color = S.util.color.rgbToHex($('.popup .color-input').css('background-color')).replace('#', '');
            var orgId = $('#orgId').val();
            var msg = $('.popup .message');
            if (id > 0) { hasid = true; }
            if (name == '' || name == null) {
                S.message.show(msg, 'error', 'Please specify a board name');
                return;
            }
            var form = { name: name, color: color, orgId: orgId };
            if (hasid) { form.boardId = id; }
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
    }
};