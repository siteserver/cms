var $url = '/pages/syncDatabase';

var data = utils.initData({
  pageType: 'prepare'
});

var methods = {
  updateDatabase: function () {
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
    this.updateDatabase();
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    utils.loading(this, false);
  }
});
