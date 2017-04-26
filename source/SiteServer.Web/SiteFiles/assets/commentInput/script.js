function loadComments(homeUrl, apiGetUrl, apiActionsAddUrl, apiActionsGoodUrl, apiActionsDeleteUrl, apiLogoutUrl, requestCount, isDelete) {
    var regUrl = homeUrl + '/#/reg?returnUrl=' + encodeURIComponent(location.href);
    var findpwdUrl = homeUrl + '/#/findpwd?returnUrl=' + encodeURIComponent(location.href);
    var app = new Vue({
        el: '#stlCommentContainer',
        data: {
            homeUrl: homeUrl,
            regUrl: regUrl,
            findpwdUrl: findpwdUrl,
            isLoading: true,
            isAuthError: false,
            isUnChecked: false,
            user: null,
            isDelete: isDelete,
            comments: null,
            requestCount: requestCount,
            requestOffset: 0,
            totalCount: 0,
            account: '',
            password: '',
            replyId: 0,
            content: ''
        },
        methods: {
            submit: function() {
                if (this.content) {
                    var comment = {
                        account: app.account,
                        password: app.password,
                        replyId: app.replyId,
                        content: app.content
                    };
                    stlClient.post(apiActionsAddUrl, comment, function(err, data) {
                        if (!err) {
                            app.user = data.user;
                            if (data.comment && data.comment.isChecked) {
                                app.comments.splice(0, 0, data.comment);
                                app.count++;
                                app.isUnChecked = false;
                                app.replyId = 0;
                            } else {
                                app.isUnChecked = true;
                            }
                            app.account = '';
                            app.password = '';
                            app.content = '';
                        } else {
                            app.isAuthError = true;
                        }

                        setTimeout(function(){
                            app.isUnChecked = false;
                            app.isAuthError = false;
                        }, 5000);
                    });
                }
            },
            good: function(comment) {
                if (!Cookies.get('stl_comment_good_' + comment.id)) {
                    stlClient.post(apiActionsGoodUrl, {
                        id: comment.id
                    }, function(err, data) {
                        if (!err) {
                            Cookies.set('stl_comment_good_' + comment.id, 'true');
                            comment.goodCount++;
                        }
                    });
                }
            },
            reply: function(commentId) {
                app.replyId = app.replyId === commentId ? 0 : commentId;
                app.content = '';
            },
            remove: function(comment) {
                stlClient.post(apiActionsDeleteUrl, {
                    id: comment.id
                }, function(err, data) {
                    if (!err) {
                        app.comments.splice(app.comments.indexOf(comment), 1);
                    }
                });
            },
            cancel: function() {
                app.replyId = 0;
                app.content = '';
            },
            logout: function() {
                stlClient.post(apiLogoutUrl, null, function(err, data){
                    app.user = null;
                });
            },
            list: function() {
                if (app.totalCount > 0 && app.requestOffset >= app.totalCount) return;
                app.isLoading = true;

                stlClient.get(apiGetUrl + '?requestCount=' + app.requestCount + '&requestOffset=' + app.requestOffset, null, function(err, data) {
                    app.isLoading = false;
                    if (!err) {
                        if (data.isCommentable) {
                            if (!app.comments) app.comments = [];
                            app.user = data.user;
                            app.totalCount = data.totalCount;
                            app.requestOffset += app.requestCount;

                            for (var i = 0; i < data.comments.length; i++) {
                                var comment = data.comments[i];
                                app.comments.push(comment);
                            }
                        }
                    }
                });
            }
        }
    });
    app.list();
}