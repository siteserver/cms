var $url = "/pages/error";

var $data = {
  pageLoad: false,
  pageAlert: null,
  logId: utils.getQueryString('logId'),
  message: utils.getQueryString('message'),
  stacktrace: null,
  addDate: null
};

var $methods = {
  loadError: function () {
    var $this = this;
    if (!$this.logId) {
      $this.pageLoad = true;
      return;
    }

    $api.get($url, {
      params: {
        logId: $this.logId
      }
    }).then(function (response) {
      var res = response.data;

      $this.message = res.logInfo.summary + ' ' + res.logInfo.message;
      $this.stacktrace = res.logInfo.stacktrace;
      $this.addDate = res.logInfo.addDate;
      $this.reportError(res.logInfo, res.version);
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
    });
  },

  reportError: function (logInfo, version) {
    if (!logInfo) return;
    Rollbar.error(logInfo.summary + ' ' + logInfo.message, {
      version: version,
      stacktrace: logInfo.stacktrace
    });
  }
};

var $vue = new Vue({
  el: '#main',
  data: $data,
  methods: $methods,
  created: function () {
    this.loadError();
  }
});