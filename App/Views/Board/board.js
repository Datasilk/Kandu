S.board = {
    id: 0,

    init: function () {
        S.accordion.load({
            container: '.boards-menu .menu-section',
            target: '.boards-menu .section-bar',
            opened: true
        });
    }
};

S.board.init();