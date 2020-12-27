var $url = '/cms/contents/contentsLayerReference';

var data = utils.init({
  siteId: utils.getQueryInt('siteId'),
  channelId: utils.getQueryInt('channelId'),
  contentId: utils.getQueryInt('contentId'),
  page: utils.getQueryInt('page'),
  content: null,
  sourceName: null,
  form: {
    siteId: null,
    channelId: null,
    contentId: null,
    title: null
  }
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
      $this.sourceName = res.sourceName;
      $this.form.siteId = $this.siteId;
      $this.form.channelId = $this.channelId;
      $this.form.contentId = $this.contentId;
      $this.form.title = res.content.title;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, this.form).then(function (response) {
      var res = response.data;

      utils.success('引用内容编辑成功!');
      parent.$vue.apiList($this.channelId, $this.page);
      utils.closeLayer();
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnEditOriginalClick: function() {
    utils.addTab('编辑内容', utils.getCmsUrl('editor', {
      siteId: this.siteId,
      channelId: this.content.sourceId,
      contentId: this.content.referenceId,
      page: this.page,
      tabName: parent.utils.getTabName(),
      reloadChannelId: this.channelId
    }));
    utils.closeLayer();
  },

  btnSubmitClick: function () {
    var $this = this;
    this.$refs.form.validate(function(valid) {
      if (valid) {
        $this.apiSubmit();
      }
    });
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
    this.apiGet();
  }
});