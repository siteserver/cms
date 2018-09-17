var $apiUrl = $apiConfig.apiUrl;
var $api = new apiUtils.Api($apiUrl + '/pages/cms/createStatus');
var $siteId = parseInt(pageUtils.getQueryStringByName('siteId'));

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
};

var methods = {
  load: function () {
    var $this = this;

    $api.get({
        siteId: $this.siteId
      },
      function (err, res) {
        setTimeout(function () {
          $this.load();
        }, 3000);
        if (err || !res || !res.value) return;

        $this.tasks = res.value.tasks;
        $this.channelsCount = res.value.channelsCount;
        $this.contentsCount = res.value.contentsCount;
        $this.filesCount = res.value.filesCount;
        $this.specialsCount = res.value.specialsCount;
        $this.pageLoad = true;
      });
  },
  getRedirectUrl: function (task) {
    var url = '../pageRedirect.aspx?siteId=' + task.siteId;
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