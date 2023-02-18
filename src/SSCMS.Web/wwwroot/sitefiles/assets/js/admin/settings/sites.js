var $url = '/settings/sites';
var $urlUpdate = $url + '/actions/update';
var $urlDelete = $url + '/actions/delete';

var data = utils.init({
  siteTypes: null,
  sites: null,
  rootSiteId: null,
  tableNames: null,
  parentSites: null,
  parentIds: null,
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
  getSiteType: function(siteType) {
    var siteType = this.siteTypes.find(function(x) {
      return x.id === siteType;
    });
    return siteType ? siteType.text : '网站';
  },

  apiGet: function () {
    var $this = this;

    $api.get($url).then(function (response) {
      var res = response.data;

      $this.siteTypes = res.siteTypes;
      $this.sites = res.sites;
      $this.rootSiteId = res.rootSiteId;
      $this.tableNames = res.tableNames;
      $this.parentSites = res.parentSites;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiDelete: function() {
    var $this = this;

    $api.post($urlDelete, this.deleteForm).then(function (response) {
      var res = response.data;

      $this.sites = res.sites;
      $this.deletePanel = false;
      setTimeout(function () {
        window.top.location.href = utils.getIndexUrl();
      }, 1500);
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      $this.deleteLoading = false;
    });
  },

  apiEdit: function() {
    var $this = this;

    this.editForm.parentId = this.parentIds && this.parentIds.length > 0 ? this.parentIds[this.parentIds.length - 1] : 0;
    $api.post($urlUpdate, this.editForm).then(function (response) {
      var res = response.data;

      // $this.sites = res.sites;
      $this.editPanel = false;
      $this.apiGet();
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      $this.editLoading = false;
    });
  },

  btnAddClick: function() {
    utils.addTab('创建新站点', utils.getSettingsUrl('sitesAdd'));
  },

  btnSaveClick: function(site) {
    utils.addTab('保存站点模板', utils.getSettingsUrl('sitesSave', {siteId: site.id}));
  },

  btnChangeClick: function(site) {
    var title = site.root ? '转移到子目录' : '转移到根目录';
    utils.addTab(title, utils.getSettingsUrl('sitesChangeRoot', {siteId: site.id}));
  },

  btnEditClick: function(site) {
    this.parentIds = _.concat([], site.parentIds);
    this.editForm = {
      siteId: site.id,
      root: site.root,
      siteName: site.siteName,
      siteType: site.siteType,
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
      deleteFiles: true
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
    var $this = this;
    utils.keyPress(function () {
      if ($this.editPanel) {
        $this.btnEditSubmitClick();
      } else if ($this.deletePanel) {
        $this.btnDeleteSubmitClick();
      }
    }, function() {
      if ($this.editPanel) {
        $this.btnEditCancelClick();
      } else if ($this.deletePanel) {
        $this.btnDeleteCancelClick();
      } else {
        $this.btnCloseClick();
      }
    });
    this.apiGet();
  }
});
