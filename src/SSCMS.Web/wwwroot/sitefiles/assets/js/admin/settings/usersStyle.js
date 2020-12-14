var $url = '/settings/usersStyle';

var data = utils.init({
  returnUrl: decodeURIComponent(utils.getQueryString('returnUrl')),
  urlUpload: null,
  styles: null,
  tableName: null,
  relatedIdentities: null,

  uploadPanel: false,
  uploadLoading: false,
  uploadList: []
});

var methods = {
  runTableStyleLayerAddMultiple: function() {
    this.apiGet();
  },

  runTableStyleLayerEditor: function() {
    this.apiGet();
  },

  runTableStyleLayerValidate: function() {
    this.apiGet();
  },

  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url).then(function (response) {
      var res = response.data;

      $this.styles = res.styles;
      $this.tableName = res.tableName;
      $this.relatedIdentities = res.relatedIdentities;

      $this.urlUpload = $apiUrl + $url + '/actions/import';
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiDelete: function (attributeName) {
    var $this = this;

    utils.loading(this, true);
    $api.delete($url, {
      data: {
        attributeName: attributeName
      }
    }).then(function (response) {
      var res = response.data;

      $this.styles = res.styles;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiReset: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url + '/actions/reset').then(function (response) {
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
      url: utils.getCommonUrl('tableStyleLayerEditor', {
        tableName: this.tableName,
        relatedIdentities: this.relatedIdentities,
        attributeName: attributeName,
        excludes: 'TextEditor,SelectCascading,Customize'
      })
    });
  },

  btnValidateClick: function (attributeName) {
    utils.openLayer({
      title: '设置验证规则',
      url: utils.getCommonUrl('tableStyleLayerValidate', {
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

  btnAddClick: function () {
    utils.openLayer({
      title: '新增字段',
      url: utils.getCommonUrl('tableStyleLayerEditor', {
        tableName: this.tableName,
        relatedIdentities: this.relatedIdentities,
        excludes: 'TextEditor,SelectCascading,Customize'
      })
    });
  },

  btnAddMultipleClick: function () {
    utils.openLayer({
      title: '批量新增字段',
      url: utils.getCommonUrl('tableStyleLayerAddMultiple', {
        tableName: this.tableName,
        relatedIdentities: this.relatedIdentities,
        excludes: 'TextEditor,SelectCascading,Customize'
      })
    });
  },

  btnImportClick: function() {
    this.uploadPanel = true;
  },

  btnResetClick: function() {
    var $this = this;

    utils.alertDelete({
      title: '重置字段',
      text: '此操作将清空自定义字段并将用户字段恢复为系统默认值，确定吗？',
      callback: function () {
        $this.apiReset();
      }
    });
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
    utils.success('字段导入成功！');
    this.apiGet();
  },

  uploadError: function(err) {
    this.uploadList = [];
    utils.loading(this, false);
    var error = JSON.parse(err.message);
    utils.error(error.message);
  },

  btnExportClick: function() {
    window.open($apiUrl + $url + '/actions/export?access_token=' + $token);
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