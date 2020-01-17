var $url = '/pages/settings/userView';

var data = {
  pageLoad: false,
  pageAlert: null,
  pageType: utils.getQueryString('pageType'),
  userId: parseInt(utils.getQueryString('userId') || '0'),
  userName: utils.getQueryString('userName'),
  returnUrl: utils.getQueryString('returnUrl'),
  user: null,
  groupName: null
};

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
      $this.pageLoad = true;
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