var $url = '/admin/settings/utilitiesCache';

var data = utils.initData({
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

      $this.$message.success('成功清空缓存！');
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
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