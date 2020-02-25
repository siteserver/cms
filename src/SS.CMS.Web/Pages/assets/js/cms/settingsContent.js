var $url = '/admin/cms/settings/settingsContent';

var data = utils.initData({
  siteId: utils.getQueryInt("siteId"),
  pageType: null,
  site: null,
  isSaveImageInTextEditor: null,
  isAutoPageInTextEditor: null,
  autoPageWordNum: null,
  isContentTitleBreakLine: null,
  isContentSubTitleBreakLine: null,
  isAutoCheckKeywords: null,
  checkContentLevel: null,
  checkContentDefaultLevel: null
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.siteId
      }
    }).then(function (response) {
      var res = response.data;

      $this.site = _.assign({}, res.value);

      $this.isSaveImageInTextEditor = res.value.isSaveImageInTextEditor;
      $this.isAutoPageInTextEditor = res.value.isAutoPageInTextEditor;
      $this.autoPageWordNum = res.value.autoPageWordNum;
      $this.isContentTitleBreakLine = res.value.isContentTitleBreakLine;
      $this.isContentSubTitleBreakLine = res.value.isContentSubTitleBreakLine;
      $this.isAutoCheckKeywords = res.value.isAutoCheckKeywords;

      $this.checkContentLevel = res.value.checkContentLevel;
      $this.checkContentDefaultLevel = res.value.checkContentDefaultLevel;

      $this.pageType = 'list';
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, {
      isSaveImageInTextEditor: this.isSaveImageInTextEditor,
      isAutoPageInTextEditor: this.isAutoPageInTextEditor,
      autoPageWordNum: this.autoPageWordNum,
      isContentTitleBreakLine: this.isContentTitleBreakLine,
      isContentSubTitleBreakLine: this.isContentSubTitleBreakLine,
      isAutoCheckKeywords: this.isAutoCheckKeywords,
      checkContentLevel: this.checkContentLevel,
      checkContentDefaultLevel: this.checkContentDefaultLevel
    }).then(function (response) {
      var res = response.data;

      $this.site = _.assign({}, res.value);

      $this.$message.success('内容设置保存成功！');
      $this.pageType = 'list';
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnSubmitClick: function () {
    var $this = this;
    this.$validator.validate().then(function (result) {
      if (result) {
        $this.apiSubmit();
      }
    });
  },

  getCheckContentLevel: function (val) {
    switch (val) {
      case 1:
        return '一级';
      case 2:
        return '二级';
      case 3:
        return '三级';
      case 4:
        return '四级';
      case 5:
        return '五级';
      default:
        return '一级';
    }
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