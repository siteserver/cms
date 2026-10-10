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

      // 页面以文本插值渲染，换行由 white-space: pre-wrap 处理，不再拼接 <br>
      $this.message = [res.summary, res.message].filter(Boolean).join(' ');
      $this.stackTrace = res.stackTrace || '';
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
      const value = sessionStorage.getItem(this.uuid);
      if (value && typeof value === "string") {
        try {
          var error = JSON.parse(value);
          this.message = error.message;
          this.stackTrace = error.stackTrace;
          this.createdDate = error.createdDate;
        } catch (e) {
          this.message = value;
        }
      }
      utils.loading(this, false);
    }
  },
});
