var $api = new apiUtils.Api(apiUrl + '/pages/settings/adminRole');

var data = {
  pageLoad: false,
  pageAlert: null,
  pageType: null,
  items: null
};

var methods = {
  getList: function () {
    var $this = this;

    $api.get(null, function (err, res) {
      if (err || !res || !res.value) return;

      $this.items = res.value;
      $this.pageLoad = true;
    });
  },

  btnEditClick: function(row) {
    location.href = 'adminRoleAdd.cshtml?roleId=' + row.id;
  },

  btnDeleteClick: function (item) {
    var $this = this;

    pageUtils.alertDelete({
      title: '删除角色',
      text: '此操作将删除角色 ' + item.roleName + '，确定吗？',
      callback: function () {
        $this.apiDelete(item);
        this.pageType = 'list';
      }
    });
  },

  apiDelete: function (item) {
    var $this = this;

    pageUtils.loading(true);
    $api.delete({
      id: item.id
    }, function (err, res) {
      pageUtils.loading(false);
      if (err || !res || !res.value) return;

      $this.items = res.value;
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