var $url = '/cms/contents/contentsLayerTaxis';

var data = utils.init({
  page: utils.getQueryInt('page'),
  siteId: utils.getQueryInt('siteId'),
  channelId: utils.getQueryInt('channelId'),
  fileName: utils.getQueryString('fileName'),
  contents: [],
  totalCount: 0,
  form: {
    type: 'to',
    value: 1
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
        fileName: this.fileName
      }
    }).then(function (response) {
      var res = response.data;

      $this.contents = res.contents;
      $this.totalCount = res.totalCount;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, {
      siteId: this.siteId,
      channelId: this.channelId,
      fileName: this.fileName,
      type: this.form.type,
      value: this.form.value,
    }).then(function (response) {
      var res = response.data;

      parent.$vue.apiList($this.channelId, $this.page, '内容排序成功!');
      utils.closeLayer();
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  getTypeName: function () {
    if (this.form.type === 'to') {
      return '调整至';
    } else if (this.form.type === 'up') {
      return '上升数目';
    } else if (this.form.type === 'down') {
      return '下降数目';
    }
    return '';
  },

  getContentUrl: function (content) {
    if (content.checked) {
      return utils.getRootUrl('redirect', {
        siteId: content.siteId,
        channelId: content.channelId,
        contentId: content.id
      });
    }
    return $apiUrl + '/preview/' + content.siteId + '/' + content.channelId + '/' + content.id;
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
    utils.keyPress(this.btnSubmitClick, this.btnCancelClick);
    this.apiGet();
  }
});
