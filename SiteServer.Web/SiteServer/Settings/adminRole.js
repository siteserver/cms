var $url = '/pages/settings/adminRole';

var data = utils.initData({
  pageType: null,
  items: null
});

var methods = {
  getList: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url).then(function (response) {
      var res = response.data;

      $this.items = res.value;
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnEditClick: function(row) {
    location.href = 'adminRoleAdd.cshtml?roleId=' + row.id;
  },

  btnDeleteClick: function (item) {
    var $this = this;

    utils.alertDelete({
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

    utils.loading(this, true);
    $api.delete($url, {
      data: {
        id: item.id
      }
    }).then(function (response) {
      var res = response.data;

      $this.items = res.value;
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.getList();
  }
});