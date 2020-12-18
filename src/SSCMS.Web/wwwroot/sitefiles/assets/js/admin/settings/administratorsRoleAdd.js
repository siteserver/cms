var $url = '/settings/administratorsRoleAdd';
var $urlSitePermission = '/settings/administratorsRoleAdd/actions/sitePermission';

var data = utils.init({
  roleId: utils.getQueryInt('roleId'),
  tabName: utils.getQueryString('tabName'),
  pageType: null,
  form: {
    roleName: null,
    description: null,
    checkedPermissions: null,
  },
  permissions: null,
  allPermissions: null,
  systemCheckAll: false,
  isSystemIndeterminate: true,
  sites: null,
  sitePermissions: [],
  site: null,
  permissionInfo: null,

  treeData: [],
  defaultExpandedKeys: []
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        roleId: this.roleId
      }
    }).then(function (response) {
      var res = response.data;

      if (res.role) {
        $this.form.roleName = res.role.roleName;
        $this.form.description = res.role.description;
      }
      $this.permissions = res.permissions;
      $this.form.checkedPermissions = [];
      $this.allPermissions = [];
      for (var i = 0; i < res.permissions.length; i++){
        $this.allPermissions.push(res.permissions[i].name)
        if (res.permissions[i].selected){
          $this.form.checkedPermissions.push(res.permissions[i].name)
        }
      }
      $this.sites = res.sites;
      $this.sitePermissions = res.sitePermissions;

      $this.pageType = 'role';
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiAdd: function () {
    var $this = this;
    
    utils.loading(this, true);
    $api.post($url, {
      roleId: 0,
      roleName: this.form.roleName,
      description: this.form.description,
      appPermissions: this.form.checkedPermissions,
      sitePermissions: this.sitePermissions
    }).then(function (response) {
      var res = response.data;

      $this.closeAndRedirect(false);
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiEdit: function () {
    var $this = this;
    
    utils.loading(this, true);
    $api.put($url, {
      roleId: this.roleId,
      roleName: this.form.roleName,
      description: this.form.description,
      appPermissions: this.form.checkedPermissions,
      sitePermissions: this.sitePermissions
    }).then(function (response) {
      var res = response.data;

      $this.closeAndRedirect(true);
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSitePermission: function (siteId) {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlSitePermission, {
      roleId: this.roleId,
      siteId: siteId
    }).then(function (response) {
      var res = response.data;

      utils.openLayer({
        title: '权限设置',
        url: utils.getSettingsUrl('administratorsRoleAddLayerSite', {
          roleId: this.roleId,
          siteId: siteId
        }),
        full: true,
        success: function(layero, index) {
          var iframeWin = window[layero.find('iframe')[0]['name']];

          var sitePermission = $this.getSitePermission(siteId);
          iframeWin.$vue.setValues(res, sitePermission);
        }
      });

    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  closeAndRedirect: function(isEdit) {
    var tabVue = utils.getTabVue(this.tabName);
    if (tabVue) {
      if (isEdit) {
        utils.success('角色编辑成功！');
      } else {
        utils.success('角色添加成功！');
      }
      tabVue.apiGet();
    }
    utils.removeTab();
    utils.openTab(this.tabName);
  },

  getSitePermission: function(siteId) {
    return _.find(this.sitePermissions, function(o) { return o.siteId === siteId; });
  },

  handleSystemCheckAllChange: function(val) {
    this.form.checkedPermissions = [];
    if (val) {
      for (var i = 0; i < this.permissions.length; i++){
        this.form.checkedPermissions.push(this.permissions[i].name)
      }
    }
    this.isSystemIndeterminate = false;
  },

  handleCheckedPermissionsChange: function(value) {
    var checkedCount = value.length;
    this.systemCheckAll = checkedCount === this.permissions.length;
    this.isSystemIndeterminate = checkedCount > 0 && checkedCount < this.permissions.length;
  },

  getPermissionText: function(name) {
    for (var i = 0; i < this.permissions.length; i++){
      if (this.permissions[i].name === name) {
        return this.permissions[i].text;
      }
    }
    return '';
  },

  btnPermissionClick: function(site) {
    this.apiSitePermission(site.id);
  },

  btnSubmitClick: function () {
    var $this = this;

    this.$refs.form.validate(function(valid) {
      if (valid) {
        if ($this.roleId > 0) {
          $this.apiEdit();
        } else {
          $this.apiAdd();
        }
      }
    });
  },

  btnCancelClick: function () {
    utils.removeTab();
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