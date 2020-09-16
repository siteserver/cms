var $url = "/dashboard";

var data = utils.init({
  adminWelcomeHtml: null
});

var methods = {
  apiGet: function() {
    var $this = this;

    utils.loading(this, true);
    $api.get($url).then(function (response) {
      var res = response.data;

      $this.homeWelcomeHtml = res.homeWelcomeHtml || '欢迎使用用户中心';
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  }
};

var $vue = new Vue({
  el: "#main",
  data: data,
  created: function () {
    this.apiGet();
  },
  methods: methods
});