var $url = '/admin/settings/logsConfig';

var data = utils.initData({
  config: null,
  form: {
    isTimeThreshold: null,
    timeThreshold: null,
    isLogSite: null,
    isLogAdmin: null,
    isLogUser: null,
    isLogError: null
  }
});

var methods = {
  apiGet: function () {
    var $this = this;

    $api.get($url).then(function (response) {
      var res = response.data;

      $this.form.isTimeThreshold = res.config.isTimeThreshold;
      $this.form.timeThreshold = res.config.timeThreshold;
      $this.form.isLogSite = res.config.isLogSite;
      $this.form.isLogAdmin = res.config.isLogAdmin;
      $this.form.isLogUser = res.config.isLogUser;
      $this.form.isLogError = res.config.isLogError;
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, {
      isTimeThreshold: this.form.isTimeThreshold,
      timeThreshold: this.form.timeThreshold,
      isLogSite: this.form.isLogSite,
      isLogAdmin: this.form.isLogAdmin,
      isLogUser: this.form.isLogUser,
      isLogError: this.form.isLogError
    }).then(function (response) {
      var res = response.data;

      $this.$message.success('日志设置保存成功！');
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnSubmitClick: function () {
    var $this = this;

    this.$refs.form.validate(function(valid) {
      if (valid) {
        $this.apiSubmit();
      }
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