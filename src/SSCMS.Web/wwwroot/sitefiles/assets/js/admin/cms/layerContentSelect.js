var $url = '/cms/layerContentSelect';

var data = utils.init({
  siteId: utils.getQueryInt('siteId'),
  channelId: utils.getQueryInt('channelId'),
  contentId: utils.getQueryInt('contentId'),
  pageContents: null,
  total: null,
  pageSize: null,
  page: 1,
});

var methods = {
  apiGet: function (page) {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.siteId,
        channelId: this.channelId,
        contentId: this.contentId,
        page: page
      }
    }).then(function (response) {
      var res = response.data;

      $this.pageContents = res.pageContents;
      $this.total = res.total;
      $this.pageSize = res.pageSize;
      $this.page = page;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  handleCurrentChange: function(val) {
    this.apiGet(val);
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, {
      siteId: this.siteId,
      channelId: this.channelId,
      channelContentIds: this.channelContentIds,
      isRetainFiles: this.form.isRetainFiles,
    }).then(function (response) {
      var res = response.data;

      parent.$vue.apiList($this.channelId, $this.page, '内容删除成功!', true);
      utils.closeLayer();
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  getContentUrl: function (content) {
    return utils.getRootUrl('redirect', {
      siteId: content.siteId,
      channelId: content.channelId,
      contentId: content.id
    });
  },

  btnContentClick: function (content) {
    parent.$vue.runLayerContentSelect(content);
    utils.closeLayer();
  },

  btnCancelClick: function () {
    utils.closeLayer();
  },
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    utils.keyPress(null, this.btnCancelClick);
    this.apiGet();
  }
});
