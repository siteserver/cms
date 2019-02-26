var $url = '/pages/cms/createStatus';
var $urlCancel = '/pages/cms/createStatus/actions/cancel';

var $data = {
  pageLoad: false,
  pageAlert: null,
  pageType: null,
  siteId: parseInt(utils.getQueryString('siteId')),
  tasks: null,
  channelsCount: null,
  contentsCount: null,
  filesCount: null,
  specialsCount: null,
  timeoutId: null
};

var $methods = {
  load: function () {
    var $this = this;

    $api.get($url, {
      params: {
        siteId: $this.siteId
      }
    }).then(function (response) {
      var res = response.data;

      $this.tasks = res.value.tasks;
      $this.channelsCount = res.value.channelsCount;
      $this.contentsCount = res.value.contentsCount;
      $this.filesCount = res.value.filesCount;
      $this.specialsCount = res.value.specialsCount;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
      $this.timeoutId = setTimeout(function () {
        $this.load();
      }, 3000);
    });
  },

  btnCancelClick: function () {
    var $this = this;
    clearTimeout(this.timeoutId);

    utils.loading(true);
    $api.post($urlCancel, {
      siteId: $this.siteId
    }).then(function (response) {
      var res = response.data;

      $this.load();
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  }
};

var $vue = new Vue({
  el: '#main',
  data: $data,
  methods: $methods,
  created: function () {
    this.load();
  }
});