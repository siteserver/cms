var $url = '/pages/settings/userView';

var data = utils.initData({
  pageType: utils.getQueryString('pageType'),
  userId: utils.getQueryInt('userId') || 0,
  userName: utils.getQueryString('userName'),
  returnUrl: utils.getQueryString('returnUrl'),
  user: null,
  groupName: null
});

var methods = {
  getConfig: function () {
    var $this = this;

    $api.get($url, {
      params: {
        userId: this.userId,
        userName: this.userName
      }
    }).then(function (response) {
      var res = response.data;

      $this.user = res.user;
      $this.groupName = res.groupName;
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnReturnClick: function () {
    location.href = 'user.cshtml';
  }
};

new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.getConfig();
  }
});