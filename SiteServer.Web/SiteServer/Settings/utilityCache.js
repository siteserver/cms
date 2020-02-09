var $url = '/pages/settings/utilityCache';

var data = utils.initData({
  parameters: null,
  count: null
});

var methods = {
  getConfig: function () {
    var $this = this;

    $api.get($url).then(function (response) {
      var res = response.data;

      $this.parameters = res.value;
      $this.count = res.count;
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

      $this.parameters = res.value;
      $this.count = res.count;
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