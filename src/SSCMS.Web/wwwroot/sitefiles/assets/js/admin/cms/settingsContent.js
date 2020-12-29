var $url = '/cms/settings/settingsContent';

var data = utils.init({
  siteId: utils.getQueryInt("siteId"),
  isCensorTextEnabled: null,
  form: {
    isSaveImageInTextEditor: null,
    pageSize: null,
    isAutoPageInTextEditor: null,
    autoPageWordNum: null,
    isContentTitleBreakLine: null,
    isContentSubTitleBreakLine: null,
    isAutoCheckKeywords: null,
    checkContentLevel: null,
    checkContentDefaultLevel: null,
  }
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

      $this.isCensorTextEnabled = res.isCensorTextEnabled;
      $this.form.isSaveImageInTextEditor = res.site.isSaveImageInTextEditor;
      $this.form.pageSize = res.site.pageSize;
      $this.form.isAutoPageInTextEditor = res.site.isAutoPageInTextEditor;
      $this.form.autoPageWordNum = res.site.autoPageWordNum;
      $this.form.isContentTitleBreakLine = res.site.isContentTitleBreakLine;
      $this.form.isContentSubTitleBreakLine = res.site.isContentSubTitleBreakLine;
      $this.form.isAutoCheckKeywords = res.site.isAutoCheckKeywords;
      $this.form.checkContentLevel = res.site.checkContentLevel;
      $this.form.checkContentDefaultLevel = res.site.checkContentDefaultLevel;
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
      isSaveImageInTextEditor: this.form.isSaveImageInTextEditor,
      pageSize: this.form.pageSize,
      isAutoPageInTextEditor: this.form.isAutoPageInTextEditor,
      autoPageWordNum: this.form.autoPageWordNum,
      isContentTitleBreakLine: this.form.isContentTitleBreakLine,
      isContentSubTitleBreakLine: this.form.isContentSubTitleBreakLine,
      isAutoCheckKeywords: this.form.isAutoCheckKeywords,
      checkContentLevel: this.form.checkContentLevel,
      checkContentDefaultLevel: this.form.checkContentDefaultLevel
    }).then(function (response) {
      var res = response.data;

      utils.success('内容设置保存成功！');
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnSubmitClick: function () {
    var $this = this;
    this.$refs.form.validate(function(valid) {
      if (valid) {
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