var $url = '/admin/cms/settings/settingsCreate';

var data = utils.initData({
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
        isCreateContentIfContentChanged: res.value.isCreateContentIfContentChanged,
        isCreateChannelIfChannelChanged: res.value.isCreateChannelIfChannelChanged,
        isCreateShowPageInfo: res.value.isCreateShowPageInfo,
        isCreateIe8Compatible: res.value.isCreateIe8Compatible,
        isCreateBrowserNoCache: res.value.isCreateBrowserNoCache,
        isCreateJsIgnoreError: res.value.isCreateJsIgnoreError,
        isCreateWithJQuery: res.value.isCreateWithJQuery,
        isCreateDoubleClick: res.value.isCreateDoubleClick,
        createStaticMaxPage: res.value.createStaticMaxPage,
        isCreateUseDefaultFileName: res.value.isCreateUseDefaultFileName,
        createDefaultFileName: res.value.createDefaultFileName,
        isCreateStaticContentByAddDate: res.value.isCreateStaticContentByAddDate,
        createStaticContentAddDate: res.value.createStaticContentAddDate,
      };
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, this.form).then(function (response) {
      var res = response.data;

      $this.$message.success('页面生成设置保存成功！');
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