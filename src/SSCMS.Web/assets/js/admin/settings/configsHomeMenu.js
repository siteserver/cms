var $url = '/settings/configsHomeMenu';

var data = utils.init({
  userMenus: null,
  groups: null,
  expandRowKeys: [],
  userMenu: null,
  panel: false
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url).then(function (response) {
      var res = response.data;

      $this.userMenus = $this.getItems(res.userMenus);
      $this.groups = res.groups;
      for (var i = 0; i < $this.userMenus.length; i++) {
        var menu = $this.userMenus[i];
        if (menu.children && menu.children.length > 0) {
          $this.expandRowKeys.push(menu.id);
        }
      }
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiDelete: function (id) {
    var $this = this;

    utils.loading(this, true);
    $api.delete($url, {
      data: {
        id: id
      }
    }).then(function (response) {
      var res = response.data;

      $this.userMenus = $this.getItems(res.userMenus);
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

      $this.userMenus = $this.getItems(res.userMenus);
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function (userMenu) {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, userMenu).then(function (response) {
      var res = response.data;

      $this.userMenu = null;
      $this.userMenus = $this.getItems(res.userMenus);
      $this.panel = false;
      utils.success(userMenu.id === 0 ? '用户菜单添加成功！' : '用户菜单修改成功！');
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  getItems: function (menus) {
    var userMenus = [];
    for (var i = 0; i < menus.length; i++) {
      var menu = menus[i];
      if (menu.parentId === 0) {
        menu.children = [];
        userMenus.push(menu);
      }
    }
    for (var i = 0; i < menus.length; i++) {
      var menu = menus[i];
      if (menu.parentId > 0) {
        var parent = _.find(userMenus, function (x) {
          return x.id === menu.parentId
        })
        if (parent) {
          parent.children.push(menu);
        }
      }
    }

    return userMenus;
  },

  getUserGroups: function (userMenu) {
    if (userMenu.isGroup) {
      var str = '';
      _.forEach(this.groups, function (group) {
        if (userMenu.groupIds.indexOf(group.id) !== -1) {
          str += ', ' + group.groupName;
        }
      });
      return str ? str.substr(2) : '';
    }
    return '所有用户组';
  },

  btnAddClick: function (parentId) {
    var taxis = 0;
    var parent = null;
    if (parentId > 0) {
      parent = this.userMenus.find(function (x) {
        return x.id === parentId;
      })
    }
    if (parent) {
      _.forEach(parent.children, function (value) {
        if (value.taxis > taxis) {
          taxis = value.taxis;
        }
      });
    } else {
      _.forEach(this.userMenus, function (value) {
        if (value.taxis > taxis) {
          taxis = value.taxis;
        }
      });
    }

    this.userMenu = {
      id: 0,
      systemId: '',
      disabled: false,
      parentId: parentId,
      taxis: taxis + 1,
      text: '',
      href: '',
      iconClass: '',
      target: '',
      isGroup: false,
      groupIds: []
    };
    this.panel = true;
  },

  btnResetClick: function () {
    var $this = this;

    utils.alertDelete({
      title: '重置用户菜单',
      text: '此操作将把用户菜单恢复为系统默认值，确定吗？',
      button: '确认重置',
      callback: function () {
        $this.apiReset();
      }
    });
  },

  btnEditClick: function (userMenu) {
    this.panel = true;
    this.userMenu = userMenu;
  },

  btnDeleteClick: function (userMenu) {
    var $this = this;

    utils.alertDelete({
      title: '删除用户菜单',
      text: '此操作将删除用户菜单 ' + userMenu.text + '，确定吗？',
      callback: function () {
        $this.apiDelete(userMenu.id);
      }
    });
  },

  btnSubmitClick: function () {
    var $this = this;

    this.$refs.userMenu.validate(function(valid) {
      if (valid) {
        $this.apiSubmit($this.userMenu);
      }
    });
  },
  
  btnCancelClick: function () {
    this.panel = false;
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