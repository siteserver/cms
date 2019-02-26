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

      window.top.location.href = 'login.cshtml';
    }).catch(function (error) {
      $this.pageLoad = true;
      $this.pageAlert = utils.getPageAlert(error);
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