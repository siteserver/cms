var $siteId = parseInt(utils.getQueryString('siteId'));

var $url = '/pages/cms/createStatus';
var $apiCancel = '/pages/cms/createStatus/actions/cancel';

var data = {
  pageLoad: false,
  pageAlert: null,
  pageType: null,
  siteId: $siteId,
  tasks: null,
  channelsCount: null,
  contentsCount: null,
  filesCount: null,
  specialsCount: null,
  timeoutId: null
};

var methods = {
  load: function () {
    var $this = this;
    $this.pageAlert = null;

    if (this.pageLoad) utils.loading(true);
    $api.get($url, {
        params: {
          siteId: $this.siteId
        }
      }).then(function(response) {
        var res = response.data;

        $this.tasks = res.value.tasks;
        $this.channelsCount = res.value.channelsCount;
        $this.contentsCount = res.value.contentsCount;
        $this.filesCount = res.value.filesCount;
        $this.specialsCount = res.value.specialsCount;
      })
      .catch(function(error) {
        $this.pageAlert = utils.getPageAlert(error);
      })
      .then(function() {
        $this.timeoutId = setTimeout(function () {
          $this.load();
        }, 3000);
        utils.loading(false);
        $this.pageLoad = true;
      });
  },

  getRedirectUrl: function (task) {
    var url = '../redirect.cshtml?siteId=' + task.siteId;
    if (task.channelId) {
      url += '&channelId=' + task.channelId;
    }
    if (task.contentId) {
      url += '&contentId=' + task.contentId;
    }
    if (task.fileTemplateId) {
      url += '&fileTemplateId=' + task.fileTemplateId;
    }
    if (task.specialId) {
      url += '&specialId=' + task.specialId;
    }
    return url;
  },

  btnCancelClick: function () {
    clearTimeout(this.timeoutId);
    var $this = this;

    utils.loading(true);
    $api
      .post($url + '/actions/cancel', {
        siteId: $this.siteId
      })
      .then(function(response) {
        var res = response.data;

        $this.load();
      })
      .catch(function(error) {
        $this.pageAlert = utils.getPageAlert(error);
      });
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.load();
  }
});