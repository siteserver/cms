var $url = "/error";

var data = utils.init({
  logId: utils.getQueryInt('logId'),
  uuid: utils.getQueryString('uuid'),
  message: utils.getQueryString('message'),
  stackTrace: null,
  createdDate: null
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

      $this.message = res.summary + ' ' + res.message;
      $this.stackTrace = res.stackTrace;
      $this.createdDate = res.createdDate;
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
      this.createdDate = error.createdDate;
      utils.loading(this, false);
    }
  },
});
