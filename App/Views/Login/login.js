S.login = {
    submit: function () {
        var data = {
            email: $('#email').val(),
            password: $('#password').val()
        }

        S.ajax.post('User/Authenticate', data, function (d) {
            if (d) {
                var msg = $('.login .message');
                if (d == 'err') {
                    S.message.show(msg, 'error', 'Your credentials are incorrect');
                    return false;
                } else if (d == 'success') {
                    S.message.show(msg, '', 'Login success! Redirecting to dashboard...');
                    window.location.href = '/dashboard';
                }
            }
        });
    }
}
$('.login form').on('submit', function (e) { S.login.submit(); e.preventDefault(); return false; });