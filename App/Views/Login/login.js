S.login = {
    submit: function () {
        var data = {
            email: $('#email').val(),
            password: $('#password').val()
        }
        var msg = '.login .messages';

        S.ajax.post('User/Authenticate', data, function (d) {
            if (d) {
                S.message.show(msg, '', 'Login success! Redirecting...');
                if (location.href.toLowerCase().indexOf('login')) {
                    window.location.href = d;
                } else {
                    window.location.reload();
                }
            }
        }, function (err) {
            S.message.clear(msg);
            S.message.show(msg, 'error', 'Incorrect email or password');
        });
    }
};
$('.login form').on('submit', function (e) { S.login.submit(); e.preventDefault(); return false; });