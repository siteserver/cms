var $url = '/pages/syncDatabase';

var data = {
  pageLoad: false,
  pageAlert: null,
  pageType: 'prepare'
};

var methods = {
  updateDatabase: function () {
    var $this = this;

    $api.post($url).then(function (response) {
      $this.pageType = 'done';
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
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
    this.pageLoad = true;
  }
});
