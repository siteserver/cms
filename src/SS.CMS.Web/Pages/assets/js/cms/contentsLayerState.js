var $url = '/admin/cms/contents/contentsLayerState';

var data = utils.initData({
  siteId: utils.getQueryInt('siteId'),
  channelId: utils.getQueryInt('channelId'),
  contentId: utils.getQueryInt('contentId'),
  contentChecks: null,
  content: null,
  state: null
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.siteId,
        channelId: this.channelId,
        contentId: this.contentId
      }
    }).then(function (response) {
      var res = response.data;

      $this.contentChecks = res.contentChecks;
      $this.content = res.content;
      $this.state = res.state;
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  getContentUrl: function (content) {
    if (content.checked) {
      return '../redirect.cshtml?siteId=' + content.siteId + '&channelId=' + content.channelId + '&contentId=' + content.id;
    }
    return $apiUrl + '/preview/' + content.siteId + '/' + content.channelId + '/' + content.id;
  },

  btnAdminClick: function(adminId) {
    utils.openLayer({
      title: "管理员查看",
      url: '../shared/adminLayerView.cshtml?adminId=' + adminId,
      width: 550,
      height: 450
    });
  },
  
  btnSubmitClick: function () {
    window.parent.layer.closeAll()
    window.parent.utils.openLayer({
      title: "审核内容",
      url: "contentsLayerCheck.cshtml?siteId=" +
        this.siteId +
        "&channelId=" +
        this.channelId +
        "&channelContentIds=" +
        this.channelId +
        "_" +
        this.contentId,
      full: true
    });
  },

  btnCancelClick: function () {
    utils.closeLayer();
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