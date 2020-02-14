var $url = '/pages/settings/logConfig';

var data = utils.initData({
  pageType: null,
  config: null,
  isTimeThreshold: null,
  timeThreshold: null,
  isLogSite: null,
  isLogAdmin: null,
  isLogUser: null,
  isLogError: null
});

var methods = {
  getConfig: function () {
    var $this = this;

    $api.get($url).then(function (response) {
      var res = response.data;

      $this.config = _.clone(res.value);

      $this.isTimeThreshold = res.value.isTimeThreshold;
      $this.timeThreshold = res.value.timeThreshold;
      $this.isLogSite = res.value.isLogSite;
      $this.isLogAdmin = res.value.isLogAdmin;
      $this.isLogUser = res.value.isLogUser;
      $this.isLogError = res.value.isLogError;
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      $this.pageType = 'list';
      utils.loading($this, false);
    });
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, {
      isTimeThreshold: this.isTimeThreshold,
      timeThreshold: this.timeThreshold,
      isLogSite: this.isLogSite,
      isLogAdmin: this.isLogAdmin,
      isLogUser: this.isLogUser,
      isLogError: this.isLogError
    }).then(function (response) {
      var res = response.data;

      $this.pageAlert = {
        type: 'success',
        html: '日志设置保存成功！'
      };
      $this.config = _.clone(res.value);
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
      $this.pageType = 'list';
    });
  },

  btnSubmitClick: function () {
    var $this = this;
    this.$validator.validate().then(function (result) {
      if (result) {
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
    this.getConfig();
  }
});