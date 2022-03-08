S.invite = {
    accept: function () {
        $('.accept-form .step-1').hide();
        $('.accept-form .step-2').show();
        const params = new URLSearchParams(window.location.search);
        const key = params.get('pk');
        S.ajax.post('Invitations/Accept', { email: $('#invite_email').val(), publickey: key }, (response) => {
            $('.accept-form .step-2').hide();
            $('.accept-form .step-3').show();
            window.location.href = response;

        }, (err) => {
            S.message.show('.accept-form .messages', 'error', err.responseText);
            $('.accept-form .step-2').hide();
            $('.accept-form .step-1').show();
        });
    }
};

$('.accept-form button.accept').on('click', (e) => {
    S.invite.accept();
    e.stopPropagation(); e.preventDefault(); return false; //prevent form submit
});