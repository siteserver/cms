var $url = '/pages/settings/userStyle';

var $data = {
  pageLoad: false,
  pageAlert: null,
  pageType: null,
  items: null,
  tableName: null,
  relatedIdentities: null
};

var $methods = {
  getList: function () {
    var $this = this;

    $api.get($url).then(function (response) {
      var res = response.data;

      $this.items = res.value;
      $this.tableName = res.tableName;
      $this.relatedIdentities = res.relatedIdentities;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
    });
  },

  delete: function (attributeName) {
    var $this = this;

    utils.loading(true);
    $api.delete($url, {
      params: {
        attributeName: attributeName
      }
    }).then(function (response) {
      var res = response.data;

      $this.items = res.value;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  btnEditClick: function (attributeName) {
    utils.openLayer({
      title: '编辑字段',
      url: '../Shared/tableStyle.cshtml?tableName=' + this.tableName + '&relatedIdentities=' + this.relatedIdentities + '&attributeName=' + attributeName
    });
  },

  btnValidateClick: function (attributeName) {
    utils.openLayer({
      title: '设置验证规则',
      url: '../Shared/tableValidate.cshtml?tableName=' + this.tableName + '&relatedIdentities=' + this.relatedIdentities + '&attributeName=' + attributeName
    });
  },

  btnAddClick: function () {
    utils.openLayer({
      title: '新增字段',
      url: '../Shared/tableStyle.cshtml?tableName=' + this.tableName + '&relatedIdentities=' + this.relatedIdentities
    });
  },

  btnDeleteClick: function (attributeName) {
    var $this = this;

    swal2({
        title: '删除字段',
        text: '此操作将删除字段 ' + attributeName + '，确定吗？',
        type: 'question',
        confirmButtonText: '删 除',
        confirmButtonClass: 'btn btn-danger',
        showCancelButton: true,
        cancelButtonText: '取 消'
      })
      .then(function (result) {
        if (result.value) {
          $this.delete(attributeName);
        }
      });
  }
};

new Vue({
  el: '#main',
  data: $data,
  methods: $methods,
  created: function () {
    this.getList();
  }
});