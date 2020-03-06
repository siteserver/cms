var $url = '/admin/syncDatabase';

var data = utils.initData({
  pageType: 'prepare',
  databaseVersion: null,
  productVersion: null
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url).then(function (response) {
      var res = response.data;

      $this.databaseVersion = res.databaseVersion;
      $this.productVersion = res.productVersion;
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiUpdate: function () {
    var $this = this;

    $api.post($url).then(function (response) {
      $this.pageType = 'done';
    }).catch(function (error) {
      utils.error($this, error);
    });
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
