var $url = '/admin/settings/administratorsRole';

var data = utils.initData({
  roles: null
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url).then(function (response) {
      var res = response.data;

      $this.roles = res.roles;
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
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

      $this.roles = res.roles;
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnAddClick: function() {
    location.href = utils.getSettingsUrl('administratorsRoleAdd');
  },

  btnEditClick: function(row) {
    location.href = utils.getSettingsUrl('administratorsRoleAdd', {roleId: row.id});
  },

  btnDeleteClick: function (item) {
    var $this = this;

    utils.alertDelete({
      title: '删除角色',
      text: '此操作将删除角色 ' + item.roleName + '，确定吗？',
      callback: function () {
        $this.apiDelete(item);
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