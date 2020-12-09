var $url = '/settings/homeMenus';

var data = utils.init({
  userMenus: null,
  groups: null,
  expandRowKeys: [],
  userMenu: null,
  defaultOpeneds: [],
  defaultActive: null
});

var methods = {
  btnSideMenuOpen: function(index) {
    var userMenu = _.find(this.userMenus, function(x){
      return x.id == index;
    });
    this.userMenu = _.assign({}, userMenu);
    if (!this.userMenu.groupIds) {
      this.userMenu.groupIds = [];
    }
    this.defaultActive = index;
  },

  btnSideMenuClick: function(val) {
    var sideMenuIds = val + '';
    var ids = sideMenuIds.split('/');
    var menus = this.userMenus;
    var menu = null;
    var defaultOpeneds = [];

    for (var i = 0; i < ids.length; i++) {
      menu = _.find(menus, function(x){
        return x.id == ids[i];
      });
      menus = menu.children;
      defaultOpeneds.push(menu.id);
    }
    this.defaultOpeneds = defaultOpeneds;
    
    if (menu) {
      this.btnMenuClick(menu);
    }
  },

  btnMenuClick: function(menu) {
    this.defaultActive = this.defaultOpeneds.join('/');
    this.userMenu = _.assign({}, menu);
    if (!this.userMenu.groupIds) {
      this.userMenu.groupIds = [];
    }
  },

  getIndex: function (level1, level2, level3) {
    if (level3) return level1.id + '/' + level2.id + '/' + level3.id;
    else if (level2) return level1.id + '/' + level2.id;
    else if (level1) return level1.id;
    return '';
  },

  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url).then(function (response) {
      var res = response.data;

      $this.userMenus = $this.getItems(res.userMenus);
      $this.groups = res.groups;
      // for (var i = 0; i < $this.userMenus.length; i++) {
      //   var menu = $this.userMenus[i];
      //   if (menu.children && menu.children.length > 0) {
      //     $this.expandRowKeys.push(menu.id);
      //   }
      // }
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
      $this.userMenu = null;
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
      $this.userMenu = null;
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
      utils.success(userMenu.id === 0 ? '用户菜单新增成功！' : '用户菜单修改成功！');
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

  getCardTitle: function(){
    if (this.userMenu.id !== 0) return '修改菜单';
    return this.userMenu.parentId > 0 ? '新增下级菜单' : '新增一级菜单';
  },

  btnAddChildClick: function () {
    var parentId = this.userMenu ? this.userMenu.id : 0;

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
    } else if (parentId === 0) {
      _.forEach(this.userMenus, function (value) {
        if (value.taxis > taxis) {
          taxis = value.taxis;
        }
      });
    }

    this.userMenu = {
      id: 0,
      disabled: false,
      parentId: parentId,
      taxis: taxis + 1,
      text: '',
      link: '',
      iconClass: '',
      target: '',
      isGroup: false,
      groupIds: []
    };

    this.defaultActive = null;
  },

  btnAddFirstClick: function () {
    var taxis = 0;
    _.forEach(this.userMenus, function (value) {
      if (value.taxis > taxis) {
        taxis = value.taxis;
      }
    });

    this.userMenu = {
      id: 0,
      disabled: false,
      parentId: 0,
      taxis: taxis + 1,
      text: '',
      link: '',
      iconClass: '',
      target: '',
      isGroup: false,
      groupIds: []
    };

    this.defaultActive = null;
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

  btnDeleteClick: function () {
    var $this = this;

    utils.alertDelete({
      title: '删除用户菜单',
      text: '此操作将删除用户菜单及其下级菜单，确定吗？',
      callback: function () {
        $this.apiDelete($this.userMenu.id);
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

  btnCancelClick: function() {
    this.userMenu = null;
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