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
            var msg = $('.popup.show .messages');
            if (name == '' || name == null) {
                S.util.message(msg, 'error', 'Please specify a security group name');
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
                    S.util.message(msg, 'error', S.message.error.generic);
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
        title: null,

        show: function (id, orgId, title, callback) {
            S.security.details.groupId = id;
            S.security.details.orgId = orgId;
            S.security.details.title = title;
            S.security.details.callback = callback;
            if (typeof title == 'function') { title = title(id, orgId); }
            S.security.details.load(id, title, callback);
        },

        load: function (id, title, callback) {
            S.ajax.post('SecurityGroups/Details', { groupId: id }, function (result) {
                if (!S.security.details.popup) {
                    S.security.details.popup = S.popup.show(title, result, {
                        width: 700,
                        backButton: true,
                        onClose: function () {
                            S.security.details.popup = null;
                            if (callback) { callback(); }
                        }
                    });
                } else {
                    //popup already exists
                    S.security.details.popup.find('.popup-body').html(result);
                }

                //set up security key events
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
            S.security.details.popup = null;
        },

        addKey: {
            popup: null,
            groupId: null,
            show: function (id, callback) {
                S.security.details.addKey.groupId = id;
                S.ajax.post('SecurityGroups/ShowAddKey', { groupId: id }, function (result) {
                    S.security.details.addKey.popup = S.popup.show('Add Security Key', result, {
                        width: 700,
                        backButton: true,
                        onClose: function () {
                            if (callback) { callback(); }
                        }
                    });

                    //set up events
                    var elem = $('#security_keys');
                    var description = $('.key-description');
                    var option = $(elem[0].options[elem[0].selectedIndex]);
                    description.html(option.attr('data-title'));

                    elem.on('input', () => {
                        //user changed security key
                        option = $(elem[0].options[elem[0].selectedIndex]);
                        description.html(option.attr('data-title'));
                        var str = option.attr('data-scopes');
                        if (str != null && str != '') {
                            var scopes = str.split(',');
                            var elemscope = $('#security_scope');
                            elemscope.find('option').removeAttr('selected').hide();
                            for (var x = 0; x < scopes.length; x++) {
                                var scopetype = scopes[x];
                                var option = elemscope.find('option[value="' + scopetype + '"]');
                                if (x == 0) {
                                    option.val(scopetype);
                                    option.attr('selected', 'selected');
                                    S.security.details.getScopeItems(id);
                                }
                                option.show();
                            }
                            $('.add-security-key .scope').show();
                        } else {
                            $('.add-security-key .scope').hide();
                            $('.add-security-key .scope-id').hide();
                        }
                    });

                    $('#security_scope').on('input', () => { S.security.details.getScopeItems(id); });

                    $('.add-security-key a.button').on('click', S.security.details.addKey.submit);
                });
            },

            submit: function () {
                var groupId = S.security.details.addKey.groupId;
                var form = {
                    groupId: groupId,
                    key: $('#security_keys').val(),
                    scope: $('#security_scope').val(),
                    scopeId: $('#security_scopeId').val()
                }
                S.ajax.post('SecurityGroups/AddKey', form, function (result) {
                    S.popup.hide(S.security.details.addKey.popup);
                    S.security.details.load(groupId, S.security.details.title);
                });
            }
        },

        saveKey: function (e) {
            var checkbox = $(e.target);
            var form = {
                groupId: checkbox.attr('data-id'),
                key: checkbox.attr('name').replace('key-', ''),
                ischecked: checkbox[0].checked,
                scope: checkbox.attr('data-scope'),
                scopeid: checkbox.attr('data-scopeid')
            }
            S.ajax.post('SecurityGroups/SaveKey', form, function (result) {});
        },

        removeKey: function (e, groupId, key, scope, scopeId) {
            if (!confirm('Do you really want to remove this key from your security group?')) { return; }
            var form = {
                groupId: groupId,
                key: key,
                scope: scope,
                scopeId: scopeId
            };
            var msg = $('.popup.show .messages');
            S.ajax.post('SecurityGroups/RemoveKey', form, function () {
                var target = $(e.target);
                if (!target.hasClass('key')) { target = target.parents('.key').first(); }
                target.remove();
            }, (err) => {
                S.util.message(msg, 'error', err.responseText);
            });
        },

        save: function () {
            var form = {
                groupId: S.security.details.groupId,
                name: $('#groupname').val()
            };
            var msg = $('.popup.show .messages');
            S.ajax.post('SecurityGroups/Update', form, function () {
                $('.popup.show .title h5').html(form.name);
                S.orgs.security.refresh();
                if (S.security.details.callback) {
                    S.security.details.callback('saved');
                }
                S.util.message(msg, null, 'Security Group updated successfully');
            }, (err) => {
                S.util.message(msg, 'error', err.responseText);
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
        },

        getScopeItems: function (groupId) {
            var form = {
                groupId: groupId,
                key: $('#security_keys').val(),
                scope: $('#security_scope').val()
            };

            if (form.scope == 0) {
                $('.popup .scope-id').hide();
                $('#security_scopeId').html('');
                return;
            }
            S.ajax.post('SecurityGroups/GetScopeItems', form, function (data) {
                $('#security_scopeId').html(data);
            }, (err) => {
                S.util.message(msg, 'error', err.responseText);
            });
            $('.popup .scope-id').show();
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