var $url = '/settings/utilitiesParameters';

var data = utils.init({
  environments: null,
  settings: null
});

var methods = {
  apiGet: function () {
    var $this = this;

    $api.get($url).then(function (response) {
      var res = response.data;

      var server = response.headers['server'];
      if (!server || server === 'Kestrel') {
        server = '命令行';
      }

      $this.environments = res.environments;
      $this.settings = res.settings;

      $this.environments.splice(0, 0, {
        key: '进程管理器',
        value: server
      });
    }).catch(function (error) {
      utils.error(error);
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