var $url = '/syncDatabase';
var $urlVerify = '/syncDatabase/actions/verify';

var data = utils.init({
  pageType: 'prepare',
  databaseVersion: null,
  version: null,
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url).then(function (response) {
      var res = response.data;

      $this.databaseVersion = res.databaseVersion;
      $this.version = res.version;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function (securityKey) {
    var $this = this;

    this.pageType = 'update';
    $api.post($url, {
      securityKey: securityKey
    }).then(function (response) {
      $this.pageType = 'done';
    }).catch(function (error) {
      utils.error(error);
    });
  },

  getDocsUrl: function() {
    return cloud.getDocsUrl('');
  },

  btnStartClick: function (e) {
    var $this = this;
    e.preventDefault();

    if (this.databaseVersion === this.version) {
      this.$prompt('请进入系统根目录，打开 sscms.json 获取 SecurityKey的值', 'SecurityKey验证', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
      }).then(function(val) {
        $this.apiSubmit(val.value);
      });
    } else {
      this.apiSubmit('');
    }
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    var $this = this;
    utils.keyPress(function () {
      if ($this.pageType === 'prepare') {
        $this.btnStartClick();
      }
    });
    this.apiGet();
  }
});
