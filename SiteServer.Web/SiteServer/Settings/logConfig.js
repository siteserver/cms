var $url = '/pages/settings/configAdmin';

var data = {
  pageLoad: false,
  pageAlert: null,
  pageType: null,
  config: null,
  isTimeThreshold: null,
  timeThreshold: null,
};

var methods = {
  getConfig: function () {
    var $this = this;

    $api.get($url).then(function (response) {
      var res = response.data;

      $this.config = _.clone(res.value);

      $this.isTimeThreshold = res.value.isTimeThreshold;
      $this.timeThreshold = res.value.timeThreshold;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageType = 'list';
      $this.pageLoad = true;
    });
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(true);
    $api.post($url, {
      isTimeThreshold: this.isTimeThreshold,
      timeThreshold: this.timeThreshold
    }).then(function (response) {
      var res = response.data;

      $this.pageAlert = {
        type: 'success',
        html: '日志设置保存成功！'
      };
      $this.config = _.clone(res.value);
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
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

new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.getConfig();
  }
});