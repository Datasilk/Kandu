S.kandu = {
    details : {
        popup: null,

        show: function () {
            $('.user-menu').addClass('hide');
            $('.bg-for-user-menu').addClass('hide');
            S.ajax.post('KanduApp/Details', {}, function (result) {
                S.kandu.details.popup = S.popup.show('Kandu Settings', result, {
                    width: 700
                });

                //set up tabs
                $('.app-details .movable > div').hide();
                $('.app-details .content-settings').show();
                $('.app-details .tab-settings').on('click', S.kandu.settings.show);
                $('.app-details .tab-email').on('click', S.kandu.email.show);
                $('.app-details .tab-plugins').on('click', S.kandu.plugins.show);
            });
        },

        tabs: {
            select: function (id) {
                $('.app-details .tabs > .tab').removeClass('selected');
                $('.app-details .tabs > .tab-' + id).addClass('selected');
                $('.app-details > .tab-content > .movable > div').hide();
                $('.app-details .content-' + id).show();
                S.popup.resize();
            }
        }
    },

    settings: {
        show: function () {
            var content = $('.app-details .content-settings');
            if (content.html().trim() == '') {
                //load app settings
                S.kandu.settings.refresh();
            }
        },

        refresh: function () {
            S.ajax.post('KanduApp/RefreshSettings', {}, function (result) {
                $('.app-details .content-settings').html(result);
            },
                (err) => {

                });
        }
    },

    email: {
        show: function () {
            var content = $('.app-details .content-email');
            if (content.html().trim() == '') {
                //load email client
                S.kandu.email.refresh();
            }
        },

        refresh: function () {
            S.ajax.post('KanduApp/RefreshEmail', { }, function (result) {
                $('.app-details .content-email').html(result);
            },
            (err) => {

            });
        },

        add: {
            popup: null,
            client: null,
            id: null,

            show: function (id) {
                S.kandu.email.add.id = id ? id : '';
                S.ajax.post('KanduApp/RenderEmailClientForm', {id:id}, function (result) {
                    S.kandu.details.popup.hide();
                    S.kandu.email.add.popup = S.popup.show((id ? 'Update' : 'Add') + ' Email Client', result, {
                        width: 700,
                        onClose: () => {
                            S.kandu.details.popup.show();
                        }
                    });
                    S.kandu.email.add.setup();
                },
                (err) => {

                });
            },

            update: function (key) {
                S.ajax.post('KanduApp/RenderEmailClientForm', { key: key}, function (result) {
                    $('.popup.show .client-form').remove();
                    $('.popup.show').append(result);
                    S.kandu.email.add.setup();
                },
                (err) => {

                });
            },

            setup: function () {
                var email_client = $('.popup.show .client-form #email_key');
                S.kandu.email.add.client = email_client.val();
                if (email_client.attr('type') != 'hidden') {
                    email_client.on('change', () => {
                        var val = email_client.val();
                        if (val != S.kandu.email.add.client) {
                            S.kandu.email.add.client = val;
                            S.kandu.email.add.update(val);
                        }
                    });
                }
            },

            cancel: function () {
                S.kandu.details.popup.show();
                S.popup.hide(S.kandu.email.add.popup);
            },

            submit: function () {
                var data = {
                    clientId: S.kandu.email.add.id,
                    key: $('#email_key').val(),
                    label: $('#email_label').val(),
                    parameters: {}
                };
                var inputs = $('.popup.show .client-form').find('input, select');
                for (var x = 0; x < inputs.length; x++) {
                    var input = $(inputs[x]);
                    if (['email_key', 'email_label'].indexOf(input.attr('id')) < 0) {
                        var val = input.val();
                        if (input.attr('type') == 'checkbox') {
                            val = input[0].checked == true ? 'True' : 'False';
                        }
                        data.parameters[input.attr('id').replace(data.key + '_', '')] = val;
                    }
                }

                S.ajax.post((data.clientId == '' ? 'KanduApp/CreateEmailClient' : 'KanduApp/UpdateEmailClient'),
                    data, function (result) {
                        S.kandu.email.refresh();
                        S.kandu.email.add.cancel();
                },
                (err) => {

                });
            }
        },

        clients: {
            remove: function (id) {
                if (confirm('Do you really want to remove this email client? This cannot be undone, ' +
                    'and any email actions that rely on this email client will no longer work and will need to be updated')) {
                    S.ajax.post('KanduApp/RemoveEmailClient', { clientId: id }, function (result) {
                        S.kandu.email.refresh();
                        S.kandu.email.add.cancel();
                    },
                    (err) => {

                    });
                }
            }
        },

        action: {
            popup: null,
            client: null,
            key: null,

            details: function (key) {
                S.kandu.email.action.key = key;
                S.ajax.post('KanduApp/RenderEmailActionForm', { key: key }, function (result) {
                    S.kandu.details.popup.hide();
                    S.kandu.email.action.popup = S.popup.show('Update Email Action', result, {
                        width: 700,
                        onClose: () => {
                            S.kandu.details.popup.show();
                        }
                    });
                },
                (err) => {

                });
            },

            save: function () {
                var data = {
                    key: S.kandu.email.action.key,
                    clientId: parseInt($('#action_clientId').val()),
                    subject: $('#action_subject').val(),
                    fromName: $('#action_fromname').val(),
                    fromAddress: $('#action_fromaddress').val(),
                    bodyText: $('#action_body_text').val(),
                    bodyHtml: $('#action_body_html').val()
                };
                S.ajax.post('KanduApp/UpdateEmailAction', data, function (result) {
                    S.kandu.email.refresh();
                    S.kandu.email.action.cancel();
                },
                (err) => {

                });
            },

            cancel: function () {
                S.kandu.details.popup.show();
                S.popup.hide(S.kandu.email.action.popup);
            }
        }
    },

    plugins: {
        show: function () {
            var content = $('.app-details .content-plugins');
            if (content.html().trim() == '') {
                //load plugins list
                S.kandu.plugins.refresh();
            }
        },

        refresh: function () {
            S.ajax.post('KanduApp/RefreshPlugins', {}, function (result) {
                $('.app-details .content-plugins').html(result);
            },
            (err) => {

            });
        }
    }
};
