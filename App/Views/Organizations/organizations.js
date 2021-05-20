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
        show: function (id, callback) {
            var hasid = false;
            if (id > 0) { hasid = true; }
            var view = new S.view(
                $('#template_neworg').html()
                    .replace('#submit-label#', !hasid ? 'Create Organization' : 'Update Organization')
                    .replace('#submit-click#', !hasid ? 'S.orgs.add.submit()' : 'S.orgs.add.submit(\'' + id + '\')')
                , {});
            var popup = S.popup.show(!hasid ? 'Create A New Organization' : 'Edit Organization', view.render(), { width: 430 });

            $('.org-form .button.apply').on('click', () => {
                S.orgs.add.submit(id, () => {
                    popup.hide();
                    if (callback) { callback(true); }
                });
            });
            $('.org-form .button.cancel').on('click', () => {
                popup.hide();
                if (callback) { callback(false); }
            });

            //load organization details if id is supplied
            if (hasid) {
                S.ajax.post('Organization/Details', { orgId: id }, function (data) {
                    $('#orgname').val(data.org.name);
                    $('#org_description').val(data.org.description);
                    $('#org_website').val(data.org.website);
                }, null, true);
            }
            return false;
        },

        submit: function (id, callback) {
            var hasid = false;
            var name = $('#orgname').val();
            var description = $('#org_description').val();
            var website = $('#org_website').val();
            var msg = $('.popup.show .message');
            if (id > 0) { hasid = true; }
            if (name == '' || name == null) {
                S.message.show(msg, 'error', 'Please specify an organization name');
                return;
            }
            var form = { name: name, description: description, website: website };
            if (hasid) { form.orgId = id; }
            S.ajax.post(hasid ? 'Organizations/Update' : 'Organizations/Create', form,
                function (orgId) {
                    if (callback) { callback(true, orgId); }
                },
                function (err) {
                    S.message.show(msg, 'error', err.responseText);
                    return;
                }
            );
        }
    }
};