var $url = '/v1/administrators/actions/logout';

var $data = {
  pageLoad: false,
  pageAlert: null
};

var $methods = {
  logout: function () {
    var $this = this;

    $api.post($url).then(function (response) {
      var res = response.data;

      window.top.location.href = 'pageLogin.cshtml';
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
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