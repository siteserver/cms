var $url = '/cms/settings/settingsContent';

var data = utils.init({
  siteId: utils.getQueryInt("siteId"),
  taxisTypes: [],
  form: {
    isSaveImageInTextEditor: null,
    pageSize: null,
    taxisType: null,
    isAutoPageInTextEditor: null,
    autoPageWordNum: null,
    isContentTitleBreakLine: null,
    isContentSubTitleBreakLine: null,
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

      $this.form.isSaveImageInTextEditor = res.site.isSaveImageInTextEditor;
      $this.form.pageSize = res.site.pageSize;
      $this.form.taxisType = res.site.taxisType;
      $this.form.isAutoPageInTextEditor = res.site.isAutoPageInTextEditor;
      $this.form.autoPageWordNum = res.site.autoPageWordNum;
      $this.form.isContentTitleBreakLine = res.site.isContentTitleBreakLine;
      $this.form.isContentSubTitleBreakLine = res.site.isContentSubTitleBreakLine;
      $this.form.checkContentLevel = res.site.checkContentLevel;
      $this.form.checkContentDefaultLevel = res.site.checkContentDefaultLevel;
      $this.taxisTypes = res.taxisTypes;
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
      taxisType: this.form.taxisType,
      isAutoPageInTextEditor: this.form.isAutoPageInTextEditor,
      autoPageWordNum: this.form.autoPageWordNum,
      isContentTitleBreakLine: this.form.isContentTitleBreakLine,
      isContentSubTitleBreakLine: this.form.isContentSubTitleBreakLine,
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
  },

  btnCloseClick: function() {
    utils.removeTab();
  },
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    utils.keyPress(this.btnSubmitClick, this.btnCloseClick);
    this.apiGet();
  }
});
