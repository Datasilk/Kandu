S.login = {
    watchPass: function (e) {
        
    },

    savePass: function () {
        //save new password for user
        var pass = $('#password').val();
        var pass2 = $('#password2').val();
        var msg = $('.login .message');
        var msglbl = $('.login .message > span');
        //validate password
        if (pass == '' || pass2 == '') {
            S.message.show(msg, 'error', 'You must type in your password twice');
            return;
        }
        if (pass != pass2) {
            S.message.show(msg, 'error', 'Your passwords do not match'); 
            return;
        }
        if (pass.length < 8) {
            S.message.show(msg, 'error', 'Your password must be at least 8 characters long');
            return;
        }

        //disable button
        $('#btnsavepass').prop("disabled", "disabled");

        //send new password to server
        S.ajax.post('User/SaveAdminPassword', { password: pass }, function (data) {
            //callback, replace form with message
            if (data == 'success') { 
                //show success message
                window.location.reload();
            } else {
                //show error message
                S.message.show(msg, 'error', 'An error occurred while trying to update your password');
            }
        });
    },

    validatePass: function (pass, pass2) {
        
    }
}

//add event listeners
$('#password, #password2').on('input', S.login.watchPass);
$('.login form').on('submit', function (e) {
    e.preventDefault();
    e.cancelBubble = true;
    S.login.savePass();
    return false;
});