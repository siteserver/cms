var $url = '/admin/logout';

var data = utils.initData({});

var methods = {
  logout: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url).then(function (response) {
      var res = response.data;

      $this.redirect();
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  redirect: function () {
    window.top.location.href = 'login.cshtml';
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.logout();
  }
});