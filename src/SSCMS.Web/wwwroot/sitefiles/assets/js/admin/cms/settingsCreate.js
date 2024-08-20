var $url = '/cms/settings/settingsCreate';

var data = utils.init({
  siteId: utils.getQueryInt("siteId"),
  pageType: null,
  form: null,
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

      $this.form = {
        siteId: $this.siteId,
        isCreateShowPageInfo: res.value.isCreateShowPageInfo,
        isCreateIe8Compatible: res.value.isCreateIe8Compatible,
        isCreateBrowserNoCache: res.value.isCreateBrowserNoCache,
        isCreateJsIgnoreError: res.value.isCreateJsIgnoreError,
        isCreateWithJQuery: res.value.isCreateWithJQuery,
        isCreateDisableFileDownloadApi: res.value.isCreateDisableFileDownloadApi,
        isCreateFilterGray: res.value.isCreateFilterGray,
        isCreateDoubleClick: res.value.isCreateDoubleClick,
        createStaticMaxPage: res.value.createStaticMaxPage,
        isCreateUseDefaultFileName: res.value.isCreateUseDefaultFileName,
        createDefaultFileName: res.value.createDefaultFileName,
        isCreateStaticContentByAddDate: res.value.isCreateStaticContentByAddDate,
        createStaticContentAddDate: res.value.createStaticContentAddDate,
      };
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

      utils.success('页面生成设置保存成功！');
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
