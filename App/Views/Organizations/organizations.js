﻿//NOTE: S.org is available from all dashboard pages

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
            S.head.user.submenus.hide();
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
                    var msg = $('.popup.show .messages');
                    S.util.message(msg, 'error', err.responseText);
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
            var msg = $('.popup.show .messages');
            if (id > 0) { hasid = true; }
            if (form.name == '' || form.name == null) {
                S.util.message(msg, 'error', 'Please specify an organization name');
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
                    S.util.message(msg, 'error', err.responseText);
                    return;
                }
            );
        }
    },

    details: {
        popup: null,
        orgId: null,

        show: function (id, title, callback) {
            S.ajax.post('Organizations/Details', { orgId: id }, (result) => {
                S.orgs.details.orgId = id;
                S.orgs.details.popup = S.popup.show(title, result, {
                    width: 700, offsetTop: 20, padding: 20,
                    onResize: () => {
                        var win = S.window;
                        var content = $('.org-details .tab-content');
                        var rect = content[0].getBoundingClientRect();
                        content.css({ 'max-height': (win.h - rect.top - 40) + 'px' });
                    },
                    onClose: callback
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
                $('.org-details .tab-members').on('click', S.orgs.members.show);
                $('.org-details .tab-security').on('click', S.orgs.security.show);
                $('.org-details .tab-settings').on('click', S.orgs.settings.show);
                $('.org-details .tab-theme').on('click', S.orgs.theme.show);

                //set up buttons
                $('.org-details .btn-add-board').on('click', S.orgs.boards.add);

                //set up custom scrollbars
                S.scrollbar.add('.org-details .tab-content', { touch: true });
            },
            (err) => {

            });
        },

        editInfo: function () {
            $('.org-details .org-form').removeClass('hide');
            $('.org-details .org-info').hide();
            $('.org-details .org-edit-link').hide();
        },

        cancelEdit: function () {
            $('.org-details .org-form').addClass('hide');
            $('.org-details .org-info').show();
            $('.org-details .org-edit-link').show();
        },

        save: function () {
            //validate data
            var form = {
                orgId: S.orgs.details.orgId,
                name: $('#orgname').val(),
                description: $('#org_description').val(),
                website: $('#org_website').val()
            };
            var msg = $('.popup.show .messages');
            if (form.name == '' || form.name == null) {
                S.util.message(msg, 'error', 'Please specify an organization name');
                return;
            }
            if (form.website != '') {
                form.website = 'https://' + form.website.replace('http://', '').replace('https://', '');
            }

            S.ajax.post('Organizations/Update', form, (result) => {
                S.util.message(msg, null, 'Organization details have been updated');
                $('.org-form a.apply').addClass('hide');
                S.orgs.list.reload();
                $('.popup .title > h5').html(form.name);
                S.orgs.details.cancelEdit();
                $('.org-details .org-description').html(form.description);
                $('.org-details .website-link').html(form.website != '' ? '<a href="' + form.website + '" target="_blank">' + form.website + '</a>' : '');
            },
            (err) => {
                S.util.message(msg, 'error', err.responseText);
            });
        },

        tabs: {
            select: function (id) {
                $('.org-details .tabs > .tab').removeClass('selected');
                $('.org-details .tabs > .tab-' + id).addClass('selected');
                $('.org-details > .tab-content > .movable > div').hide();
                $('.org-details .content-' + id).show();
                S.popup.resize();
            }
        }
    },

    boards: {
        refresh: function () {
            S.popup.resize();
            S.ajax.post('Boards/BoardsMenu', { orgId: S.orgs.details.orgId, listOnly:true }, (result) => {
                $('.org-details .content-boards').html(result);
                $('.org-details .btn-add-board').on('click', S.orgs.boards.add);
                S.popup.resize();
            },
            (err) => {

            });
        },

        add: function () {
            S.orgs.details.popup.hide();
            S.boards.add.show(null, null, '', S.orgs.details.orgId, () => {
                S.orgs.details.popup.show();
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
                $('.org-details .content-teams').html(result);
                $('.org-details .btn-add-team').on('click', S.orgs.teams.add);
                S.popup.resize();
            },
            (err) => {

            });
        },

        details: function (id, name) {
            S.orgs.details.popup.hide();
            S.teams.details.show(id, S.orgs.details.orgId, name, (type) => {
                if (type != 'saved') {
                    S.orgs.details.popup.show();
                    S.popup.resize();
                    S.orgs.teams.refresh();
                }
            });
        },

        add: function () {
            S.orgs.details.popup.hide();
            S.teams.add.show(null, S.orgs.details.orgId, () => {
                S.orgs.details.popup.show();
                S.popup.resize();
                S.orgs.teams.refresh();
            });
        }
    },

    members: {
        show: function () {
            S.orgs.details.tabs.select('members');
            var content = $('.org-details .content-members');
            if (content.html().trim() == '') {
                //load members search
                S.members.search.init(S.orgs.details.orgId, '.org-details .content-members', 'S.orgs.members.details.show');
            }
        },
         
        refresh: function () {
            S.members.search.query(S.orgs.details.orgId, 1, 20, '.org-details .content-members', $('#search_members').val(), 'S.orgs.members.details.show');
        },

        details: {
            show: function (userId, name) {
                S.orgs.details.popup.hide();
                S.user.details.show(userId, S.orgs.details.orgId, name, () => {
                    S.orgs.details.popup.show();
                    S.popup.resize();
                });
            }
        }
    },

    security: {
        show: function () {
            S.orgs.details.tabs.select('security');
            var content = $('.org-details .content-security');
            if (content.html().trim() == '') {
                //load security groups
                S.orgs.security.refresh();
            }
        },

        refresh: function () {
            S.ajax.post('SecurityGroups/RefreshList', { orgId: S.orgs.details.orgId }, function (result) {
                $('.org-details .content-security').html(result);
                $('.org-details .btn-add-security-group').on('click', S.orgs.security.add);
                S.popup.resize();
            },
                (err) => {

            });
        },

        add: function () {
            S.orgs.details.popup.hide();
            S.security.add.show('', S.orgs.details.orgId, () => {
                S.orgs.details.popup.show();
                S.orgs.boards.refresh();
            });
        },

        details: function (id, orgId, title, callback) {
            S.security.details.show(id, orgId, title, callback);
        }
    },

    settings: {
        show: function () {
            S.orgs.details.tabs.select('settings');
            var content = $('.org-details .content-settings');
            if (content.html().trim() == '') {
                //load organization settings
                S.orgs.settings.refresh();
            }
        },

        refresh: function () {
            S.ajax.post('Organizations/RefreshSettings', { orgId: S.orgs.details.orgId }, function (result) {
                var content = $('.org-details .content-settings');
                content.html(result);
                var savebtn = $('.org-details .btn-save-settings');
                content.find('select, input').on('keyup, change', () => {
                    savebtn.removeClass('hide');
                });
                S.popup.resize();
            },
                (err) => {

            });
        },

        save: function () {
            var savebtn = $('.org-details .btn-save-settings');
            savebtn.addClass('hide');
            var obj = new Object();
            $('.org-details .content-settings').find('select, input, textarea').map((i, a) => {
                a = $(a);
                obj[a.attr('id')] = a.val();
            });
            var data = {
                orgId: S.orgs.details.orgId,
                parameters: JSON.stringify(obj)
            };
            console.log(data);
            S.ajax.post('Organizations/SaveSettings', data, function (result) {
                S.util.message('.org-details .content-settings .message', null, 'Organization settings have been saved');
            },
                (err) => {
                    S.util.message('.org-details .content-settings .message', 'error', err.responseText);
            });
        }
    },

    theme: {
        show: function () {
            S.orgs.details.tabs.select('theme');
            var content = $('.org-details .content-theme');
            if (content.html().trim() == '') {
                //load organization theme
                S.orgs.theme.refresh();
            }
        },

        refresh: function () {
            S.ajax.post('Organizations/RefreshTheme', { orgId: S.orgs.details.orgId }, function (result) {
                var content = $('.org-details .content-theme');
                content.html(result);
                $('#themefileCSS').on('change', S.orgs.theme.canSave);
                $('#themefileJS').on('change', S.orgs.theme.canSave);
                $('#themefileResources').on('change', S.orgs.theme.canSave);
                $('.org-details .btn-save-theme').on('click', S.orgs.theme.save);
                $('.org-details .resource .icon-close').on('click', S.orgs.theme.deleteResource);
                S.popup.resize();
                S.accordion.load();
            },
            (err) => {

            });
        },

        canSave: function () {
            $('.org-details .btn-save-theme').removeClass('hide');
        },

        upload: function (file, type, callback) {
            console.log('upload theme file');
            var xhr = new XMLHttpRequest();
            xhr.onload = function () {
                if (xhr.status >= 200 && xhr.status < 400) {
                    //request success
                    if (callback) { callback();}
                }
            };
            var formData = new FormData();
            formData.append("file", file);
            xhr.open('POST', '/uploadtheme?orgId=' + S.orgs.details.orgId + '&type=' + type, false);
            xhr.send(formData);
        },

        queueUploads: function (files, type, callback) {
            if (files == null || files.length == 0) {
                if (callback) { callback(); }
                return;
            }
            console.log(files);
            var file = files.splice(0, 1)[0];
            console.log(['file', file]);
            S.orgs.theme.upload(file, type, () => {
                console.log(files);
                S.orgs.theme.queueUploads(files, type, callback);
            });
        },

        save: function () {
            var savebtn = $('.org-details .btn-save-theme');
            savebtn.addClass('hide');
            //upload CSS file first /////////////////////////////////////////////////////////////
            var input = themefileCSS;
            console.log('upload css...')
            if (input.files && input.files.length > 0) {
                S.orgs.theme.upload(input.files[0], 'css', uploadJs);
            } else {
                uploadJs();
            }

            function uploadJs() {
                //upload CSS file
                input = themefileJS;
                console.log('upload js...')
                if (input.files && input.files.length > 0) {
                    S.orgs.theme.upload(input.files[0], 'js', uploadResources);
                } else {
                    uploadResources();
                }
            }

            function uploadResources() {
                //upload all resource files
                input = themefileResources;
                console.log('upload resources...')
                if (input.files && input.files.length > 0) {
                    S.orgs.theme.queueUploads([...input.files], 'resource', () => {
                        //finally, refresh themes panel
                        S.orgs.theme.refresh();
                        savebtn.removeClass('hide');
                        console.log('finished uploading...');
                    });
                } else {
                    console.log('finished uploading...');
                }
            }
        },

        deleteResource: function (e) {
            if (!confirm('Do you really want to delete this theme file resource? This cannot be undone.')) { return;}
            var target = $(e.target).parents('.resource').first();
            var filename = target.attr('data-id');
            S.ajax.post('Organizations/DeleteThemeResource', { orgId: S.orgs.details.orgId, filename: filename }, () => {
                S.orgs.theme.refresh();
            });
        }
    }
};

setTimeout(() => {
    S.orgs.init();
}, 100);