var $url = '/pages/update';

var $data = {
  pageLoad: false,
  pageAlert: null,
  step: 1
};

var $methods = {
  load: function () {
    var $this = this;

    $api.get($url).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
    });
  },

  btnUpdateClick: function () {
    var $this = this;

    $this.step = 2;
    $api.post($url).then(function (response) {
      var res = response.data;

      $this.step = 3;
    }).catch(function (error) {
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