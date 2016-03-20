$('#select-tag').selectize({
    valueField: 'name',
    labelField: 'name',
    searchField: 'name',
    options: [],
    maxItems: 1,
    render: {
        option: function (item, escape) {
            return '<div>' +
                '<span class="title">' +
                    '<span class="name">' + escape(item.name) + '</span>' +

                '</span>' +
                '<span class="description">' + escape(item.info) + '</span>' +
                '<ul class="meta">' +
                    '<li class="watchers"><span>' + escape(item.followers) + '</span> followers</li>' +
                    '<li class="forks"><span>' + escape(item.questions) + '</span> times used</li>' +
                '</ul>' +
            '</div>';
        }
    },
    load: function (query, callback) {
        if (!query.length) return callback();
        $.ajax({
            url: '/api/Tag/SearchTags?s=' + encodeURIComponent(query),
            type: 'POST',
            error: function () {
                callback();
            },
            success: function (res) {
                callback(res.slice(0, 10));
            }
        });
    }
});