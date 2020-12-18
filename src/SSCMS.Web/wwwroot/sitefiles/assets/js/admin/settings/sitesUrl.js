var $url = '/settings/sitesUrl';

var data = utils.init({
  pageType: null,
  sites: null,

  editPanel: false,
  editForm: null,
  editLoading: false,
  editRules: {
    separatedWebUrl: [
      { required: true, message: '独立部署站点访问地址', trigger: 'blur' }
    ],
    assetsDir: [
      { required: true, message: '上传文件存储文件夹', trigger: 'blur' }
    ],
    separatedAssetsUrl: [
      { required: true, message: '独立部署上传文件访问地址', trigger: 'blur' }
    ],
    separatedApiUrl: [
      { required: true, message: '独立部署API访问地址', trigger: 'blur' }
    ]
  }
});

var methods = {
  apiGet: function () {
    var $this = this;

    $api.get($url).then(function (response) {
      var res = response.data;

      $this.sites = res.sites;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnEditClick: function(row) {
    var site = row;

    this.editForm = {
      siteId: site.id,
      siteName: site.siteName,
      isSeparatedWeb: site.isSeparatedWeb,
      separatedWebUrl: site.separatedWebUrl,
      isSeparatedAssets: site.isSeparatedAssets,
      assetsDir: site.assetsDir,
      separatedAssetsUrl: site.separatedAssetsUrl,
      isSeparatedApi: site.isSeparatedApi,
      separatedApiUrl: site.separatedApiUrl,
    };
    this.editPanel = true;
  },

  btnEditCancelClick: function() {
    this.editPanel = false;
    this.editLoading = false;
  },

  btnEditSubmitClick: function() {
    var $this = this;
    this.$refs.editForm.validate(function(valid) {
      if (valid) {
        $this.editLoading = true;
        $this.apiEdit();
      }
    });
  },

  apiEdit: function() {
    var $this = this;

    $api.put($url, this.editForm).then(function (response) {
      var res = response.data;

      $this.sites = res.sites;
      $this.editPanel = false;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      $this.editLoading = false;
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