var $url = '/pages/settings/utilityCache';

var data = {
  pageLoad: false,
  pageAlert: null,
  parameters: null,
  count: null
};

var methods = {
  getConfig: function () {
    var $this = this;

    $api.get($url).then(function (response) {
      var res = response.data;

      $this.parameters = res.value;
      $this.count = res.count;
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

      $this.parameters = res.value;
      $this.count = res.count;
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