var $api = new apiUtils.Api(apiUrl + '/pages/cms/configContent?siteId=' + pageUtils.getQueryStringByName("siteId"));

var data = {
  pageLoad: false,
  pageAlert: null,
  pageType: null,
  siteInfo: null,
  config: null,
  isSaveImageInTextEditor: null,
  isAutoPageInTextEditor: null,
  autoPageWordNum: null,
  isContentTitleBreakLine: null,
  isContentSubTitleBreakLine: null,
  isAutoCheckKeywords: null,
  isCheckContentLevel: null,
  checkContentLevel: null,
  checkContentDefaultLevel: null
};

var methods = {
  getConfig: function () {
    var $this = this;

    $api.get(null, function (err, res) {
      if (err || !res || !res.value) return;

      $this.siteInfo = _.clone(res.value);
      $this.config = _.clone(res.config);

      $this.isSaveImageInTextEditor = res.config.isSaveImageInTextEditor;
      $this.isAutoPageInTextEditor = res.config.isAutoPageInTextEditor;
      $this.autoPageWordNum = res.config.autoPageWordNum;
      $this.isContentTitleBreakLine = res.config.isContentTitleBreakLine;
      $this.isContentSubTitleBreakLine = res.config.isContentSubTitleBreakLine;
      $this.isAutoCheckKeywords = res.config.isAutoCheckKeywords;
      $this.isCheckContentLevel = res.config.isCheckContentLevel;

      $this.checkContentLevel = $this.isCheckContentLevel ? res.config.checkContentLevel : 1;
      $this.checkContentDefaultLevel = res.config.checkContentDefaultLevel;

      $this.pageType = 'list';
      $this.pageLoad = true;
    });
  },

  submit: function () {
    var $this = this;

    $this.isCheckContentLevel = $this.checkContentLevel > 1;

    pageUtils.loading(true);
    $api.post({
      isSaveImageInTextEditor: $this.isSaveImageInTextEditor,
      isAutoPageInTextEditor: $this.isAutoPageInTextEditor,
      autoPageWordNum: $this.autoPageWordNum,
      isContentTitleBreakLine: $this.isContentTitleBreakLine,
      isContentSubTitleBreakLine: $this.isContentSubTitleBreakLine,
      isAutoCheckKeywords: $this.isAutoCheckKeywords,
      isCheckContentLevel: $this.isCheckContentLevel,
      checkContentLevel: $this.checkContentLevel,
      checkContentDefaultLevel: $this.checkContentDefaultLevel
    }, function (err, res) {
      pageUtils.loading(false);
      if (err) {
        $this.pageAlert = {
          type: 'danger',
          html: err.message
        };
        return;
      }

      $this.pageAlert = {
        type: 'success',
        html: '内容设置保存成功！'
      };
      
      $this.siteInfo = _.clone(res.value);
      $this.config = _.clone(res.config);

      $this.pageType = 'list';
    });
  },

  btnSubmitClick: function () {
    var $this = this;
    this.$validator.validate().then(function (result) {
      if (result) {
        $this.submit();
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

new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.getConfig();
  }
});