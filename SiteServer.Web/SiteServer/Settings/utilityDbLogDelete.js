var $url = '/pages/settings/utilityDbLogDelete';

var data = utils.initData({
  lastExecuteDate: null
});

var methods = {
  getConfig: function () {
    var $this = this;

    $api.get($url).then(function (response) {
      var res = response.data;

      $this.lastExecuteDate = res.value;
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnCleanClick: function () {
    var $this = this;
    
    utils.loading(this, true);
    $api.post($url).then(function (response) {
      var res = response.data;

      $this.lastExecuteDate = res.value;

      $this.pageAlert = {
        type: 'success',
        html: '数据库日志清除成功！'
      };
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
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