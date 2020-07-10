var $url = "/error";

var data = utils.init({
  logId: utils.getQueryInt('logId'),
  uuid: utils.getQueryString('uuid'),
  message: utils.getQueryString('message'),
  stackTrace: null,
  addDate: null
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
      $this.stackTrace = res.error.stackTrace;
      $this.addDate = res.error.addDate;
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
  methods: methods,
  created: function () {
    if (this.logId > 0) {
      this.apiGet();
    } else if (this.uuid) {
      var error = JSON.parse(sessionStorage.getItem(this.uuid));
      this.message = error.message;
      this.stackTrace = error.stackTrace;
      this.addDate = error.addDate;
      utils.loading(this, false);
    }
  },
});