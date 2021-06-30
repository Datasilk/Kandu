S.members = {
    search: {
        orgId: null,
        cache: {
            page: 1,
            length: 10,
            search: '',
        },

        init: function (orgId, container) {
            S.members.search.orgId = orgId;
            S.members.search.query(orgId, 1, 0, container, '');
        },

        query: function (orgId, page, length, container, search) {
            S.members.search.cache = { page: page, length: length, search: search };
            S.ajax.post('Members/RefreshList', { orgId: orgId, page: page, length: length, search: search }, (html) => {
                if (length == 0) {
                    $(container).html(html);
                    $(container + ' .search-members form').on('submit', (e) => {
                        e.preventDefault();
                        S.members.search.query(orgId, 1, 20, container, $(container + ' #search_members').val());
                        return false;
                    });
                } else {
                    $(container + ' .content-members .members-list').html(html);
                }
            });
        },
    },

    events: {
        callbacks: [],

        listen: function (callback) {
            S.members.events.callbacks.push(callback);
        },

        broadcast: function (action, params) {
            var c = S.members.events.callbacks;
            for (var x = 0; x < c.length; x++) {
                c[x](action, params);
            }
        }
    },
}