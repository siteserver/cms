var $url = '/admin/cms/settings/settingsContent';

var data = utils.initData({
  siteId: utils.getQueryInt("siteId"),
  form: {
    isSaveImageInTextEditor: null,
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

      $this.form.isSaveImageInTextEditor = res.isSaveImageInTextEditor;
      $this.form.isAutoPageInTextEditor = res.isAutoPageInTextEditor;
      $this.form.autoPageWordNum = res.autoPageWordNum;
      $this.form.isContentTitleBreakLine = res.isContentTitleBreakLine;
      $this.form.isContentSubTitleBreakLine = res.isContentSubTitleBreakLine;
      $this.form.isAutoCheckKeywords = res.isAutoCheckKeywords;
      $this.form.checkContentLevel = res.checkContentLevel;
      $this.form.checkContentDefaultLevel = res.checkContentDefaultLevel;
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
      siteId: this.siteId,
      isSaveImageInTextEditor: this.form.isSaveImageInTextEditor,
      isAutoPageInTextEditor: this.form.isAutoPageInTextEditor,
      autoPageWordNum: this.form.autoPageWordNum,
      isContentTitleBreakLine: this.form.isContentTitleBreakLine,
      isContentSubTitleBreakLine: this.form.isContentSubTitleBreakLine,
      isAutoCheckKeywords: this.form.isAutoCheckKeywords,
      checkContentLevel: this.form.checkContentLevel,
      checkContentDefaultLevel: this.form.checkContentDefaultLevel
    }).then(function (response) {
      var res = response.data;

      $this.$message.success('内容设置保存成功！');
    }).catch(function (error) {
      utils.error($this, error);
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