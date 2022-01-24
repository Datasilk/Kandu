S.security = {
    add: {
        orgId: null,
        popup: null,
        callback: null,

        show: function (id, orgId, callback) {
            S.security.add.orgId = orgId;
            S.security.add.callback = callback;
            S.ajax.post('SecurityGroups/RenderGroupForm', { groupId: id, orgId: orgId }, (html) => {
                S.security.add.popup = S.popup.show(!id ? 'Create A New Security Group' : 'Edit Security Group', html, {
                    width: 430,
                    backButton:true,
                    onClose: function () {
                        if (callback) { callback(false); }
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
            S.ajax.post('SecurityGroups/Create', { orgId: S.security.add.orgId, name: name },
                function (data) {
                    S.security.add.hide();
                    var callback = S.security.add.callback;
                    if (callback) {
                        S.orgs.security.refresh();
                        callback(true);
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

        show: function (id, orgId, title, callback) {
            S.security.details.groupId = id;
            S.security.details.orgId = orgId;
            S.security.details.callback = callback;
            if (typeof title == 'function') { title = title(id, orgId); }
            S.ajax.post('SecurityGroups/Details', { groupId: id }, function (result) {
                S.security.details.popup = S.popup.show(title, result, {
                    width: 700,
                    backButton:true,
                    onClose: function () {
                        if (callback) { callback(); }
                    }
                });
                //set up key check event
                $('.security-details .key').on('input', S.security.details.saveKey);

                //set up tabs
                $('.security-details .movable > div').hide();
                $('.security-details .content-keys').show();
                $('.security-details .tab-members').on('click', S.orgs.security.show);

                //set up custom scrollbars
                S.scrollbar.add('.security-details .tab-content', { touch: true });
            });
        },

        hide: function () {
            S.popup.hide(S.security.details.popup);
        },

        saveKey: function (e) {
            var checkbox = $(e.target);
            S.ajax.post('SecurityGroups/SaveKey', { groupId: id, key:checkbox.attr('name').replace('key-',''), checked: checkbox[0].checked ? 1 : 0 }, function (result) {

            });
        },

        save: function () {
            var form = {
                groupId: S.security.details.groupId,
                name: $('#groupname').val()
            };
            var msg = $('.popup.show .message');
            S.ajax.post('SecurityGroups/Update', form, function () {
                $('.popup.show .title h5').html(form.name);
                S.orgs.security.refresh();
                if (S.security.details.callback) {
                    S.security.details.callback('saved');
                }
                S.message.show(msg, null, 'Security Group updated successfully');
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