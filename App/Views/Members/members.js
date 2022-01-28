S.members = {
    search: {
        orgId: null,
        container: null,
        cache: {
            page: 1,
            length: 10,
            search: '',
        },

        init: function (orgId, container, onclick) {
            S.members.search.orgId = orgId;
            S.members.search.container = container;
            //render search form with instructions
            S.members.search.query(orgId, 1, 0, container, '', onclick);
        },

        query: function (orgId, page, length, container, search, onclick) {
            if (container == null) {
                container = S.members.search.container;
            }
            S.members.search.cache = { page: page, length: length, search: search, onclick };
            S.ajax.post('Members/RefreshList', { orgId: orgId, page: page, length: length, search: search, onclick:onclick }, (html) => {
                if (length == 0) {
                    $(container).html(html);
                    S.popup.resize();
                    $(container + ' .search-members form').on('submit', (e) => {
                        e.preventDefault();
                        S.members.search.query(orgId, 1, 20, container, $(container + ' #search_members').val(), onclick);
                        return false;
                    });
                } else {
                    $(container + ' .members-list').html(html);
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