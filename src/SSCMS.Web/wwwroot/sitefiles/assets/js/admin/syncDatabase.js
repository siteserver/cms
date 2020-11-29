var $url = '/syncDatabase';

var data = utils.init({
  pageType: 'prepare',
  databaseVersion: null,
  version: null
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

  apiUpdate: function () {
    var $this = this;

    $api.post($url).then(function (response) {
      $this.pageType = 'done';
    }).catch(function (error) {
      utils.error(error);
    });
  },

  getDocsUrl: function() {
    return cloud.getDocsUrl('');
  },

  btnStartClick: function (e) {
    e.preventDefault();
    this.pageType = 'update';
    this.apiUpdate();
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
