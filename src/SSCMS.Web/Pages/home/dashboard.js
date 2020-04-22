var $url = '/home/dashboard';

var data = {
  user: null,
  config: null
};

var methods = {
  apiGet: function() {
    var $this = this;

    utils.loading(this, true);
    $api.get($url).then(function (response) {
      var res = response.data;

      if (!res.user) {
        return $this.redirectToLogin();
      }

      $this.user = res.user;
      $this.config = res.config;
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
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