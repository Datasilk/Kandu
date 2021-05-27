//NOTE: S.org is available from all dashboard pages

S.orgs = {
    init: function () {
        S.scrollbar.add('.orgs-list-menu .scroll-container', {
            touch: true,
            footer: function () {
                return 20;
            }
        });

        $(window).on('resize', S.orgs.list.resize);
    },

    list: {
        show: function () {
            $('.orgs-list-menu').removeClass('hide');
            S.orgs.list.resize();
            S.scrollbar.update('.orgs-list-menu .scroll-container');
        },

        hide: function () {
            $('.orgs-list-menu').addClass('hide');
        },

        resize: function () {
            var win = S.window;
            $('.orgs-list-menu .scroll-container').css({ 'max-height': (win.h - 70) + 'px' });
        },

        reload: function () {
            S.ajax.post('Organizations/RefreshListMenu', {},
                function (html) {
                    $('.orgs-list-menu').remove();
                    $('.user-menu').after(html);
                    $('.orgs-list-menu').removeClass('hide');
                    //reinit scrollbar for list menu
                    S.scrollbar.add('.orgs-list-menu', {
                        touch: true
                    });
                },
                function (err) {
                    S.message.show(msg, 'error', err.responseText);
                    return;
                }
            );
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

            var form = {
                name: $('#orgname').val(),
                description: $('#org_description').val(),
                website: $('#org_website').val()
            };
            var msg = $('.popup.show .message');
            if (id > 0) { hasid = true; }
            if (form.name == '' || form.name == null) {
                S.message.show(msg, 'error', 'Please specify an organization name');
                return;
            }
            if (hasid) { form.orgId = id; }
            S.ajax.post(hasid ? 'Organizations/Update' : 'Organizations/Create', form,
                function (orgId) {
                    //update the organization list menu
                    S.orgs.list.reload();
                    if (callback) { callback(true, orgId); }
                },
                function (err) {
                    S.message.show(msg, 'error', err.responseText);
                    return;
                }
            );
        }
    },

    details: {
        popup: null,
        orgId: null,

        show: function (id, title) {
            S.ajax.post('Organizations/Details', { orgId: id }, (result) => {
                S.orgs.details.orgId = id;
                S.orgs.details.popup = S.popup.show(title, result, {
                    width: 700, offsetTop: 20, padding: 20,
                    onResize: () => {
                        var win = S.window;
                        var content = $('.org-details .content');
                        var rect = content[0].getBoundingClientRect();
                        content.css({ 'max-height': (win.h - rect.top - 40) + 'px' });
                    }
                });
                //add events to fields
                $('.org-form input').on('keyup, change', () => {
                    $('.org-form a.apply').removeClass('hide');
                });

                //save button
                $('.org-form a.apply').on('click', S.orgs.details.save);

                //set up tabs
                $('.org-details .movable > div').hide();
                $('.org-details .content-boards').show();
                $('.org-details .tab-teams').on('click', S.orgs.teams.show);

                //set up buttons
                $('.org-details .btn-add-board').on('click', S.orgs.boards.add);

                //set up custom scrollbars
                S.scrollbar.add('.org-details .content', { touch: true });
            },
            (err) => {

            });
        },

        save: function () {
            //validate data
            var form = {
                orgId: S.orgs.details.orgId,
                name: $('#orgname').val(),
                description: $('#org_description').val(),
                website: $('#org_website').val()
            };
            var msg = $('.popup.show .message');
            if (form.name == '' || form.name == null) {
                S.message.show(msg, 'error', 'Please specify an organization name');
                return;
            }

            S.ajax.post('Organizations/Update', form, (result) => {
                S.message.show(msg, null, 'Organization details have been updated');
                $('.org-form a.apply').addClass('hide');
                S.orgs.list.reload();
                $('.popup .title > h5').html(form.name);
            },
            (err) => {
                S.message.show(msg, 'error', err.responseText);
            });
        },

        tabs: {
            select: function (id) {
                $('.org-details .tabs > .tab').removeClass('selected');
                $('.org-details .tabs > .tab-' + id).addClass('selected');
                $('.org-details > .content > .movable > div').hide();
                $('.org-details .content-' + id).show();
                S.popup.resize();
            }
        }
    },

    boards: {
        refresh: function () {
            S.ajax.post('Boards/BoardsMenu', { orgId: S.orgs.details.orgId }, (result) => {
                $('.content-boards').html(result);
            },
            (err) => {

            });
        },

        add: function () {
            S.orgs.details.popup.hide();
            S.boards.add.show(null, null, '', S.orgs.details.orgId, () => {
                S.orgs.details.popup.show();
                S.popup.resize();
                S.orgs.boards.refresh();
            });
        }
    },

    teams: {
        show: function () {
            S.orgs.details.tabs.select('teams');
            var content = $('.org-details .content-teams');
            if (content.html().trim() == '') {
                //load teams
                S.orgs.teams.refresh();
            }
        },

        refresh: function () {
            S.ajax.post('Teams/RefreshList', { orgId: S.orgs.details.orgId }, function (result) {
                $('.content-teams').html(result);
            },
            (err) => {

            });
        },

        add: function () {
            S.orgs.details.popup.hide();
            S.teams.add.show(null, null, S.orgs.details.orgId, () => {
                S.orgs.details.popup.show();
                S.popup.resize();
                S.orgs.teams.refresh();
            });
        }
    }
};

setTimeout(() => {
    S.orgs.init();
}, 100);