var $url = '/admin/cms/contents/contentsLayerView';

var data = utils.initData({
  siteId: utils.getQueryInt('siteId'),
  channelId: utils.getQueryInt('channelId'),
  contentId: utils.getQueryInt('contentId'),
  content: null,
  channelName: null,
  state: null,
  columns: null,
  siteUrl: null,
  groupNames: null,
  tagNames: null,
  editorColumns: null,
  activeName: 'attributes'
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

      $this.content = res.content;
      $this.channelName = res.channelName;
      $this.state = res.state,
      $this.columns = res.columns;
      $this.siteUrl = res.siteUrl;
      $this.groupNames = res.groupNames;
      $this.tagNames = res.tagNames;
      $this.editorColumns = res.editorColumns;
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  isDisplay: function (attributeName) {
    return attributeName !== 'Sequence' && attributeName !== 'Title' && attributeName !== 'ChannelId';
  },

  getUrl: function(virtualUrl) {
    if (!virtualUrl) return '';
    return _.replace(virtualUrl, '@/', this.siteUrl + '/');
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