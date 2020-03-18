var $url = '/admin/settings/sitesUrl';

var data = utils.initData({
  pageType: null,
  sites: null,
  isSeparatedApi: null,
  separatedApiUrl: null,

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
    ]
  },

  apiPanel: false,
  apiForm: null,
  apiLoading: false,
  apiRules: {
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
      $this.isSeparatedApi = res.isSeparatedApi;
      $this.separatedApiUrl = res.separatedApiUrl;
    }).catch(function (error) {
      utils.error($this, error);
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
      separatedAssetsUrl: site.separatedAssetsUrl
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

    $api.put($url + '/actions/web', this.editForm).then(function (response) {
      var res = response.data;

      $this.sites = res.sites;
      $this.editPanel = false;
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      $this.editLoading = false;
    });
  },

  btnApiClick: function() {
    this.apiForm = {
      isSeparatedApi: this.isSeparatedApi,
      separatedApiUrl: this.separatedApiUrl,
    };
    this.apiPanel = true;
  },

  btnApiCancelClick: function() {
    this.apiPanel = false;
    this.apiLoading = false;
  },

  btnApiSubmitClick: function() {
    var $this = this;
    this.$refs.apiForm.validate(function(valid) {
      if (valid) {
        $this.apiLoading = true;
        $this.apiApi();
      }
    });
  },

  apiApi: function() {
    var $this = this;

    $api.put($url + '/actions/api', this.apiForm).then(function (response) {
      var res = response.data;

      $this.isSeparatedApi = $this.apiForm.isSeparatedApi;
      $this.separatedApiUrl = $this.apiForm.separatedApiUrl;

      $this.apiPanel = false;
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      $this.apiLoading = false;
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