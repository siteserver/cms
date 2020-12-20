var $url = '/settings/administratorsRole';

var data = utils.init({
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
      utils.error(error);
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
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnAdminViewClick: function(userName) {
    utils.openLayer({
      title: '查看资料',
      url: utils.getCommonUrl('adminLayerView', { userName: userName }),
      full: true
    });
  },

  btnAddClick: function() {
    utils.addTab('添加角色', utils.getSettingsUrl('administratorsRoleAdd', {
      tabName: utils.getTabName()
    }));
  },

  btnEditClick: function(row) {
    utils.addTab('编辑角色', utils.getSettingsUrl('administratorsRoleAdd', {
      roleId: row.id,
      tabName: utils.getTabName()
    }));
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