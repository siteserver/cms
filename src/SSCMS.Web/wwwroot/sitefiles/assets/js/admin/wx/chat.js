var $url = '/wx/chat';
var $urlStar = '/wx/chat/actions/star';

var data = utils.init({
  count: null,
  chats: null,
  form: {
    siteId: utils.getQueryInt('siteId'),
    star: false,
    keyword: null,
    page: 1,
    perPage: 20
  }
});

var methods = {
  apiGet: function (page) {
    this.form.page = page;
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: this.form
    }).then(function (response) {
      var res = response.data;

      if (!res.isWxEnabled) {
        location.href = utils.getWxUrl('account', {
          siteId: $this.form.siteId,
        });
        return;
      }

      $this.count = res.count;
      $this.chats = res.chats;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiStar: function (chat) {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlStar, {
      siteId: this.form.siteId,
      chatId: chat.id,
      star: !chat.isStar
    }).then(function (response) {
      var res = response.data;

      utils.success(chat.isStar ? '取消收藏成功！' : '收藏成功！');
      chat.isStar = !chat.isStar;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnSearchClick() {
    this.apiGet(1);
  },

  btnPageClick: function(val) {
    this.apiGet(val);
  },

  btnStarClick: function (chat) {
    this.apiStar(chat);
  },

  btnReplyClick: function (chat) {
    utils.addTab('回复消息', utils.getWxUrl('chatSend', {
      siteId: this.form.siteId,
      openId: chat.openId
    }));
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiGet(1);
  }
});
