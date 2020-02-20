var $url = "/pages/error";

var data = utils.initData({
  logId: utils.getQueryInt('logId'),
  message: utils.getQueryString('message')
});

var methods = {
  apiGet: function() {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        logId: this.logId
      }
    }).then(function (response) {
      var res = response.data;

      $this.message = res.error.summary + ' ' + res.error.message;
      $this.stacktrace = res.error.stacktrace;
      $this.addDate = res.error.addDate;
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  }
};

var $vue = new Vue({
  el: "#main",
  data: data,
  created: function () {
    if (this.logId > 0) {
      this.apiGet();
    }
  },
  methods: methods
});