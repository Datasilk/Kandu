S.security = {
    add: {
        orgId: null,
        popup: null,
        callback: null,

        show: function (id, orgId, callback) {
            S.security.add.orgId = orgId;
            S.security.add.callback = callback;
            S.ajax.post('Security/RenderForm', { groupId: id, orgId: orgId }, (html) => {
                S.security.add.popup = S.popup.show(!id ? 'Create A New Security Group' : 'Edit Security Group', html, {
                    width: 430,
                    onClose: function () {
                        if (callback) { callback(); }
                    }
                });
            });
        },

        hide: function () {
            S.popup.hide(S.security.add.popup);
        },

        submit: function () {
            var name = $('#groupname').val();
            var msg = $('.popup.show .message');
            if (name == '' || name == null) {
                S.message.show(msg, 'error', 'Please specify a security group name');
                return;
            }
            S.ajax.post('Security/Create', { orgId: S.security.add.orgId, name: name },
                function (data) {
                    S.security.add.hide();
                    if (S.security.add.callback) {
                        S.security.add.callback();
                        S.security.events.broadcast('security-group-added');
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
        groupId: null,
        orgId: null,
        callback: null,

        show: function (id, orgId, name, callback) {
            S.security.details.groupId = id;
            S.security.details.orgId = orgId;
            S.security.details.callback = callback;
            if (typeof name == 'function') { name = name(id, orgId); }
            S.ajax.post('Security/Details', { groupId: id }, function (result) {
                S.security.details.popup = S.popup.show(name, result, {
                    width: 700,
                    onClose: function () {
                        if (callback) { callback(); }
                    }
                });
                //add events to fields
                $('.security-form input').on('keyup, change', () => {
                    $('.security-form a.apply').removeClass('hide');
                });

                //save button
                $('.security-form a.apply').on('click', S.security.details.save);

                //set up tabs
                $('.security-details .movable > div').hide();
                $('.security-details .content-keys').show();
                $('.security-details .tab-members').on('click', S.orgs.security.show);

                //set up custom scrollbars
                S.scrollbar.add('.team-details .tab-content', { touch: true });
            });
        },

        hide: function () {
            S.popup.hide(S.security.details.popup);
        },

        save: function () {
            var form = {
                groupId: S.security.details.groupId,
                name: $('#teamname').val(),
                description: $('#team_description').val(),
            };
            var msg = $('.popup.show .message');
            S.ajax.post('Security/Update', form, function () {
                $('.popup.show .title h5').html('Team ' + form.name);
                if (S.security.details.callback) {
                    S.security.details.callback('saved');
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
            S.security.events.callbacks.push(callback);
        },

        broadcast: function (action, params) {
            var c = S.security.events.callbacks;
            for (var x = 0; x < c.length; x++) {
                c[x](action, params);
            }
        }
    },
}