var $api = new apiUtils.Api(apiUrl + '/pages/settings/userStyle');

var data = {
  pageLoad: false,
  pageAlert: null,
  pageType: null,
  items: null,
  tableName: null,
  relatedIdentities: null
};

var methods = {
  getList: function () {
    var $this = this;

    $api.get(null, function (err, res) {
      if (err || !res || !res.value) return;

      $this.items = res.value;
      $this.tableName = res.tableName;
      $this.relatedIdentities = res.relatedIdentities;

      $this.pageLoad = true;
    });
  },
  delete: function (attributeName) {
    var $this = this;

    pageUtils.loading(true);
    $api.delete({
      attributeName: attributeName
    }, function (err, res) {
      pageUtils.loading(false);
      if (err || !res || !res.value) return;

      $this.items = res.value;
    });
  },
  btnEditClick: function (attributeName) {
    parent.pageUtils.openLayer({
      title: '编辑字段',
      url: 'Shared/tableStyle.cshtml?tableName=' + this.tableName + '&relatedIdentities=' + this.relatedIdentities + '&attributeName=' + attributeName
    });
  },
  btnValidateClick: function (attributeName) {
    parent.pageUtils.openLayer({
      title: '设置验证规则',
      url: 'Shared/tableValidate.cshtml?tableName=' + this.tableName + '&relatedIdentities=' + this.relatedIdentities + '&attributeName=' + attributeName
    });
  },
  btnAddClick: function () {
    parent.pageUtils.openLayer({
      title: '新增字段',
      url: 'Shared/tableStyle.cshtml?tableName=' + this.tableName + '&relatedIdentities=' + this.relatedIdentities
    });
  },
  btnDeleteClick: function (attributeName) {
    var $this = this;

    pageUtils.alertDelete({
      title: '删除字段',
      text: '此操作将删除字段 ' + attributeName + '，确定吗？',
      callback: function () {
        $this.delete(attributeName);
      }
    });
  }
};

new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.getList();
  }
});