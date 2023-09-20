var $url = '/settings/utilitiesCache';
var $urlClearCache = '/settings/utilitiesCache/actions/clearCache';
var $urlRestart = '/settings/utilitiesCache/actions/restart';

var data = utils.init({
  configuration: null,
  parameters: null
});

var methods = {
  apiGet: function () {
    var $this = this;

    $api.get($url).then(function (response) {
      var res = response.data;

      $this.configuration = res.configuration;
      $this.parameters = [];
      _.forOwn(res.configuration, function(value, key) {
        $this.parameters.push({
          key: key,
          value: value
        });
      });
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnCleanClick: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlClearCache).then(function (response) {
      var res = response.data;

      utils.success('缓存清空成功！');
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiRestart: function () {
    utils.loading(this, true, '系统重启中，请稍后...');
    $api.post($urlRestart).then(function (response) {
      var res = response.data;

      setTimeout(function () {
        utils.alertSuccess({
          title: '系统重启成功',
          callback: function() {
            window.top.location.reload(true);
          }
        });
      }, 30000);
    }).catch(function (error) {
      utils.error(error);
    });
  },

  btnRestartClick: function() {
    utils.alertDelete({
      title: '重启系统',
      text: '此操作将重启系统，确定吗？',
      button: '确 定',
      callback: this.apiRestart
    });
  },

  btnCloseClick: function() {
    utils.removeTab();
  },
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    utils.keyPress(this.btnCleanClick, this.btnCloseClick);
    this.apiGet();
  }
});
