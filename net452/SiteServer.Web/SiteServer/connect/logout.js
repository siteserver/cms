var $url = '/pages/cloud/logout';

var $data = {
  pageLoad: false,
  pageAlert: null
};

var $methods = {
  logout: function () {

    $ssApi.post($ssUrlLogout).then(function (response) {
      var res = response.data;

      top.location.reload();
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    });

    // $api.post($url).then(function (response) {
    //   var res = response.data;

    //   location.href = utils.getQueryString('returnUrl') || 'login.cshtml';
    // }).catch(function (error) {
    //   $this.pageLoad = true;
    //   $this.pageAlert = utils.getPageAlert(error);
    // });
  }
};

new Vue({
  el: '#main',
  data: $data,
  methods: $methods,
  created: function () {
    this.logout();
  }
});