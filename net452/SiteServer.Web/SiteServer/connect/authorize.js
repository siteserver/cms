var $url = '/pages/cloud/authorize';

var $data = {
  pageLoad: false,
  pageAlert: null
};

var $methods = {
  load: function () {
    var $this = this;

    $ssApi.post($ssUrlLogin, {
      account: $this.account,
      password: md5($this.password),
      isAutoLogin: $this.isAutoLogin
    }).then(function (response) {
      var res = response.data;

      top.location.reload();

      // $api.post($url, {
      //   userName: res.value.userName,
      //   accessToken: res.accessToken,
      //   expiresAt: res.expiresAt
      // }).then(function (response) {
      //   var res = response.data;

      //   location.href = utils.getQueryString('returnUrl') || 'dashboard.cshtml';
      // }).catch(function (error) {
      //   utils.loading(false);
      //   $this.pageAlert = utils.getPageAlert(error);
      // });
    }).catch(function (error) {
      utils.loading(false);
      $this.pageAlert = utils.getPageAlert(error);
    });
  }
};

new Vue({
  el: '#main',
  data: $data,
  methods: $methods,
  created: function () {
    this.load();
  }
});