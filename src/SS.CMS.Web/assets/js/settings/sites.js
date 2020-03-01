var $url = '/admin/settings/sites';

var data = utils.initData({
  sites: null,
  rootSiteId: null,
  tableNames: null,
  site: null,

  editPanel: false,
  editForm: null,
  editLoading: false,
  editRules: {
    siteName: [
      { required: true, message: '请输入站点名称', trigger: 'blur' }
    ],
    siteDir: [
      { required: true, message: '请输入文件夹名称', trigger: 'blur' }
    ],
    taxis: [
      { required: true, message: '请输入正确的站点排序', trigger: 'blur' }
    ]
  },
  
  deletePanel: false,
  deleteForm: null,
  deleteLoading: false,
});

var methods = {
  apiGet: function () {
    var $this = this;

    $api.get($url).then(function (response) {
      var res = response.data;

      $this.sites = res.sites;
      $this.rootSiteId = res.rootSiteId;
      $this.tableNames = res.tableNames;
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiDelete: function() {
    var $this = this;

    $api.delete($url, {
      data: this.deleteForm
    }).then(function (response) {
      var res = response.data;

      $this.sites = res.sites;
      $this.deletePanel = false;
      setTimeout(function () {
        window.top.location.href = utils.getIndexUrl();
      }, 1500);
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      $this.deleteLoading = false;
    });
  },

  apiEdit: function() {
    var $this = this;

    $api.put($url, this.editForm).then(function (response) {
      var res = response.data;

      $this.sites = res.sites;
      $this.editPanel = false;
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      $this.editLoading = false;
    });
  },

  btnAddClick: function() {
    location.href = utils.getSettingsUrl('sitesAdd');
  },

  btnSaveClick: function(row) {
    location.href = utils.getSettingsUrl('sitesSave', {siteId: row.id});
  },

  btnChangeClick: function(row) {
    location.href = utils.getSettingsUrl('sitesChangeRoot', {siteId: row.id});
  },

  btnEditClick: function(row) {
    var site = row;

    this.editForm = {
      siteId: site.id,
      root: site.root,
      siteName: site.siteName,
      siteDir: site.siteDir,
      parentId: site.parentId,
      taxis: site.taxis,
      tableRule: 'Choose',
      tableChoose: site.tableName,
      tableHandWrite: '',
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

  btnDeleteClick: function(row) {
    var site = row;

    this.deleteForm = {
      siteId: site.id,
      siteName: site.siteName,
      dir: site.siteDir,
      siteDir: null,
      deleteFiles: false
    };
    this.deletePanel = true;
  },

  btnDeleteCancelClick: function() {
    this.deletePanel = false;
    this.deleteLoading = false;
  },

  btnDeleteSubmitClick: function() {
    var $this = this;
    this.$refs.deleteForm.validate(function(valid) {
      if (valid) {
        $this.deleteLoading = true;
        $this.apiDelete();
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