var $url = '/admin/settings/usersView';

var data = utils.initData({
  userId: utils.getQueryInt('userId'),
  userName: utils.getQueryString('userName'),
  returnUrl: utils.getQueryString('returnUrl') || utils.getSettingsUrl('users'),
  user: null,
  groupName: null
});

var methods = {
  apiGet: function () {
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
    location.href = this.returnUrl;
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiGet();
  }
});