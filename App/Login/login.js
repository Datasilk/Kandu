S.login = {
    submit: function () {
        var data = {
            email: $('#email').val(),
            password: $('#password').val()
        }

        S.ajax.post('User/Authenticate', data, function (d) {
            if (d) {
                var msg = $('.login .message');
                console.log(d);
                if (d == 'err') {
                    S.message.show(msg, 'error', 'Your credentials are incorrect');
                    return false;
                } else if (d.indexOf('success') == 0) {
                    S.message.show(msg, '', 'Login success! Redirecting...');
                    window.location.href = d.substr(7);
                }
            }
        });
    }
}
$('.login form').on('submit', function (e) { S.login.submit(); e.preventDefault(); return false; });