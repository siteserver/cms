var $url = '/pages/settings/utilityDbLogDelete';

var data = {
  pageLoad: false,
  pageAlert: null,
  lastExecuteDate: null
};

var methods = {
  getConfig: function () {
    var $this = this;

    $api.get($url).then(function (response) {
      var res = response.data;

      $this.lastExecuteDate = res.value;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
    });
  },

  btnCleanClick: function () {
    var $this = this;
    
    utils.loading(true);
    $api.post($url).then(function (response) {
      var res = response.data;

      $this.lastExecuteDate = res.value;

      $this.pageAlert = {
        type: 'success',
        html: '数据库日志清除成功！'
      };
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
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