S.login = {
    submit: function () {
        var data = {
            email: $('#email').val(),
            password: $('#password').val()
        }
        var msg = $('.login .message');

        S.ajax.post('User/Authenticate', data, function (d) {
            if (d) {
                S.message.show(msg, '', 'Login success! Redirecting...');
                window.location.href = d;
            }
        }, function (err) {
            console.log(err);
        });
    }
};
$('.login form').on('submit', function (e) { S.login.submit(); e.preventDefault(); return false; });