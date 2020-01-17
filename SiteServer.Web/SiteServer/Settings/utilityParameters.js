var $url = '/pages/settings/utilityParameters';

var data = {
  pageLoad: false,
  pageAlert: null,
  parameters: null
};

var methods = {
  getConfig: function () {
    var $this = this;

    $api.get($url).then(function (response) {
      var res = response.data;

      $this.parameters = res.value;
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      $this.pageLoad = true;
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