var $api = new apiUtils.Api(apiUrl + '/pages/cms/configSite?siteId=' + pageUtils.getQueryStringByName("siteId"));

var data = {
  pageLoad: false,
  pageAlert: null,
  pageType: null,
  siteInfo: null,
  config: null,
  files: [],
  uploadUrl: null,
  siteName: null,
  charset: null,
  pageSize: null,
  isCreateDoubleClick: null
};

var methods = {
  getConfig: function () {
    var $this = this;

    $api.get(null, function (err, res) {
      if (err || !res || !res.value) return;

      $this.siteInfo = _.clone(res.value);
      $this.config = _.clone(res.config);

      $this.siteName = res.value.siteName;
      $this.charset = res.config.charset;
      $this.pageSize = res.config.pageSize;
      $this.isCreateDoubleClick = res.config.isCreateDoubleClick;
      $this.uploadUrl = apiUrl + '/pages/cms/configSite/upload?adminToken=' + res.adminToken;

      $this.pageType = 'list';
      $this.pageLoad = true;
    });
  },

  inputLogo(newFile, oldFile) {
    if (Boolean(newFile) !== Boolean(oldFile) || oldFile.error !== newFile.error) {
      if (!this.$refs.logo.active) {
        this.$refs.logo.active = true
      }
    }

    if (newFile && oldFile && newFile.xhr && newFile.success !== oldFile.success) {
      this.adminLogoUrl = newFile.response.value;
    }
  },

  submit: function () {
    var $this = this;

    pageUtils.loading(true);
    $api.post({
      siteName: $this.siteName,
      charset: $this.charset,
      pageSize: $this.pageSize,
      isCreateDoubleClick: $this.isCreateDoubleClick
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
        html: '站点设置保存成功！'
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

  getCharsetName: function (val) {
    switch (val) {
      case 'utf-8':
        return 'Unicode (UTF-8)';
      case 'gb2312':
        return '简体中文 (GB2312)';
      case 'big5':
        return '繁体中文 (Big5)';
      case 'iso-8859-1':
        return '西欧 (iso-8859-1)';
      case 'euc-kr':
        return '韩文 (euc-kr)';
      case 'euc-jp':
        return '日文 (euc-jp)';
      case 'iso-8859-6':
        return '阿拉伯文 (iso-8859-6)';
      case 'windows-874':
        return '泰文 (windows-874)';
      case 'iso-8859-9':
        return '土耳其文 (iso-8859-9)';
      case 'iso-8859-5':
        return '西里尔文 (iso-8859-5)';
      case 'iso-8859-8':
        return '希伯来文 (iso-8859-8)';
      case 'iso-8859-7':
        return '希腊文 (iso-8859-7)';
      case 'windows-1258':
        return '越南文 (windows-1258)';
      case 'iso-8859-2':
        return '中欧 (iso-8859-2)';
      default:
        return 'Unicode (UTF-8)';
    }
  }
};

new Vue({
  el: '#main',
  data: data,
  components: {
    FileUpload: VueUploadComponent
  },
  methods: methods,
  created: function () {
    this.getConfig();
  }
});