var $url = '/cms/settings/settingsStyleSite';
var $urlImport = '/cms/settings/settingsStyleSite/actions/import';

var data = utils.init({
  siteId: utils.getQueryInt('siteId'),
  returnUrl: utils.getQueryString('returnUrl'),
  urlUpload: null,
  styles: null,
  tableName: null,
  relatedIdentities: null,

  uploadPanel: false,
  uploadLoading: false,
  uploadList: []
});

var methods = {
  apiList: function (message) {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.siteId
      }
    }).then(function (response) {
      var res = response.data;

      $this.styles = res.styles;
      $this.tableName = res.tableName;
      $this.relatedIdentities = res.relatedIdentities;

      $this.urlUpload = $apiUrl + $urlImport + '?siteId=' + $this.siteId;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
      if (message) {
        utils.success(message);
      }
    });
  },

  apiDelete: function (attributeName) {
    var $this = this;

    utils.loading(this, true);
    $api.delete($url, {
      data: {
        siteId: this.siteId,
        attributeName: attributeName
      }
    }).then(function (response) {
      var res = response.data;

      $this.styles = res.value;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  getRules: function(rules) {
    if (!rules || rules.length === 0) return '无验证';
    return _.map(rules, function (rule) {
      return rule.message;
    }).join(',');
  },

  btnEditClick: function (attributeName) {
    utils.openLayer({
      title: '编辑字段',
      url: utils.getSharedUrl('tableStyleLayerEditor', {
        tableName: this.tableName,
        relatedIdentities: this.relatedIdentities,
        attributeName: attributeName
      })
    });
  },

  btnValidateClick: function (attributeName) {
    utils.openLayer({
      title: '设置验证规则',
      url: utils.getSharedUrl('tableStyleLayerValidate', {
        tableName: this.tableName,
        relatedIdentities: this.relatedIdentities,
        attributeName: attributeName
      })
    });
  },

  btnDeleteClick: function (attributeName) {
    var $this = this;

    utils.alertDelete({
      title: '删除字段',
      text: '此操作将删除字段 ' + attributeName + '，确定吗？',
      callback: function () {
        $this.apiDelete(attributeName);
      }
    });
  },

  btnCommandClick: function(command){
    if (command === 'Add') {
      this.btnAddClick();
    } else if (command === 'AddMultiple') {
      this.btnAddMultipleClick();
    } else if (command === 'Import') {
      this.btnImportClick();
    } else if (command === 'Export') {
      this.btnExportClick();
    }
  },

  btnAddClick: function () {
    utils.openLayer({
      title: '新增字段',
      url: utils.getSharedUrl('tableStyleLayerEditor', {
        tableName: this.tableName,
        relatedIdentities: this.relatedIdentities
      })
    });
  },

  btnAddMultipleClick: function () {
    utils.openLayer({
      title: '批量新增字段',
      url: utils.getSharedUrl('tableStyleLayerAddMultiple', {
        tableName: this.tableName,
        relatedIdentities: this.relatedIdentities
      })
    });
  },

  btnImportClick: function() {
    this.uploadPanel = true;
  },

  btnReturnClick: function () {
    location.href = this.returnUrl;
  },

  uploadBefore(file) {
    var isZip = file.name.indexOf('.zip', file.name.length - '.zip'.length) !== -1;
    if (!isZip) {
      utils.error('样式导入文件只能是 Zip 格式!');
    }
    return isZip;
  },

  uploadProgress: function() {
    utils.loading(this, true);
  },

  uploadSuccess: function(res, file) {
    this.uploadList = [];
    this.uploadPanel = false;
    this.apiList('字段导入成功！');
  },

  uploadError: function(err) {
    this.uploadList = [];
    utils.loading(this, false);
    var error = JSON.parse(err.message);
    utils.error(error.message);
  },

  btnExportClick: function() {
    var $this = this;
    
    utils.loading(this, true);
    $api.post($url + '/actions/export', {
      siteId: this.siteId
    }).then(function (response) {
      var res = response.data;

      window.open(res.value);
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiList();
  }
});