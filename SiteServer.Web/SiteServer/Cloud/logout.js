var $url = '/pages/cloud/logout';

var data = {
  pageLoad: false,
  pageAlert: null
};

var methods = {
  logout: function () {

    $api.post($url).then(function (response) {
      var res = response.data;

      location.href = utils.getQueryString('returnUrl') || 'login.cshtml';
    }).catch(function (error) {
      $this.pageLoad = true;
      $this.pageAlert = utils.getPageAlert(error);
    });
  }
};

new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.logout();
  }
});