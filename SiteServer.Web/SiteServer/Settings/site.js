var $url = '/pages/settings/site';

var data = {
  pageLoad: false,
  pageAlert: null,
  sites: null,
  rootSiteId: null,
  tableNames: null,
  site: null,

  editPanel: false,
  editAlert: null,
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
  deleteAlert: null,
  deleteForm: null,
  deleteLoading: false,
};

var methods = {
  apiGetConfig: function () {
    var $this = this;

    $api.get($url).then(function (response) {
      var res = response.data;

      $this.sites = res.value;
      $this.rootSiteId = res.rootSiteId;
      $this.tableNames = res.tableNames;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
    });
  },

  btnSaveClick: function(row) {
    location.href = 'siteSave.cshtml?siteId=' + row.id;
  },

  btnChangeClick: function(row) {
    location.href = 'siteChangeRoot.cshtml?siteId=' + row.id;
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
    this.editAlert = null;
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

      $this.sites = res.value;
      $this.editPanel = false;
    }).catch(function (error) {
      $this.editAlert = utils.getPanelAlert(error);
    }).then(function () {
      $this.editLoading = false;
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
    this.deleteAlert = {
      type: 'warning',
      title: '此操作将会删除站点 ' + row.siteName + '，且数据无法恢复，请谨慎操作！',
    };
    this.deletePanel = true;
  },

  btnDeleteCancelClick: function() {
    this.deletePanel = false;
    this.deleteLoading = false;
    this.deleteAlert = null;
  },

  btnDeleteSubmitClick: function() {
    var $this = this;
    this.$refs.deleteForm.validate(function(valid) {
      if (valid) {
        $this.deleteLoading = true;
        $this.apiDelete();
      }
    });
  },

  apiDelete: function() {
    var $this = this;

    $api.delete($url, {
      data: this.deleteForm
    }).then(function (response) {
      var res = response.data;

      $this.sites = res.value;
      $this.deletePanel = false;
      setTimeout(function () {
        window.top.location.href='../main.cshtml';
      }, 1500);
    }).catch(function (error) {
      $this.deleteAlert = utils.getPanelAlert(error);
    }).then(function () {
      $this.deleteLoading = false;
    });
  }
};

new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiGetConfig();
  }
});