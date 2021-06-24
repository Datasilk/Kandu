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