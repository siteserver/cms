var $url = '/admin/settings/administratorsRoleAdd';

var data = utils.initData({
  roleId: utils.getQueryInt('roleId'),
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
  checkedSiteIds: null,
  site: null,
  permissionInfo: null,

  treeData: [],
  defaultExpandedKeys: []
});

var methods = {
  getConfig: function () {
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
      $this.checkedSiteIds = res.checkedSiteIds || [];

      for (var i = 0; i < res.sitePermissionsList.length; i++){
        var permissionInfo = $this.getPermissionInfo(res.sitePermissionsList[i]);
        for (var j = 0; j < res.sites.length; j++){
          var site = res.sites[j];
          if (site.id === permissionInfo.siteId) {
            site.permissionInfo = permissionInfo;
          }
        }
      }

      $this.pageType = 'role';
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
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

  getPermissionInfo: function(res) {
    var sitePermissions = res.sitePermissions;
    var checkedSitePermissions = [];
    var allSitePermissions = [];
    for (var i = 0; i < res.sitePermissions.length; i++){
      allSitePermissions.push(res.sitePermissions[i].name)
      if (res.sitePermissions[i].selected){
        checkedSitePermissions.push(res.sitePermissions[i].name)
      }
    }

    var pluginPermissions = res.pluginPermissions;
    var checkedPluginPermissions = [];
    var allPluginPermissions = [];
    for (var i = 0; i < res.pluginPermissions.length; i++){
      allPluginPermissions.push(res.pluginPermissions[i].name)
      if (res.pluginPermissions[i].selected){
        checkedPluginPermissions.push(res.pluginPermissions[i].name)
      }
    }

    var channelPermissions = res.channelPermissions;
    var checkedChannelPermissions = [];
    var allChannelPermissions = [];
    for (var i = 0; i < res.channelPermissions.length; i++){
      allChannelPermissions.push(res.channelPermissions[i].name)
      if (res.channelPermissions[i].selected){
        checkedChannelPermissions.push(res.channelPermissions[i].name)
      }
    }

    var channel = res.channel;
    var checkedChannelIds = res.checkedChannelIds || [];

    return {
      siteId: res.value,
      sitePermissions: sitePermissions,
      siteCheckAll: false,
      isSiteIndeterminate: true,
      allSitePermissions: allSitePermissions,
      checkedSitePermissions: checkedSitePermissions,
  
      pluginPermissions: pluginPermissions,
      pluginCheckAll: false,
      isPluginIndeterminate: true,
      allPluginPermissions: allPluginPermissions,
      checkedPluginPermissions: checkedPluginPermissions,
  
      channelPermissions: channelPermissions,
      channelCheckAll: false,
      isChannelIndeterminate: true,
      allChannelPermissions: allChannelPermissions,
      checkedChannelPermissions: checkedChannelPermissions,
  
      channel: channel,
      checkedChannelIds: checkedChannelIds
    };
  },

  btnPermissionClick: function(site) {
    this.site = site;
    if (this.site.permissionInfo) {
      this.permissionInfo = this.site.permissionInfo;
      this.pageType = 'permissions';
      return;
    }

    var $this = this;

    utils.loading(this, true);
    $api.get($url + '/' + this.site.id, {
      params: {
        roleId: this.roleId
      }
    }).then(function (response) {
      var res = response.data;

      $this.site.permissionInfo = $this.getPermissionInfo(res);
      $this.permissionInfo = $this.site.permissionInfo;
      $this.treeData = [$this.permissionInfo.channel];
      $this.defaultExpandedKeys = [$this.permissionInfo.channel.id];
      $this.pageType = 'permissions';
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },
  
  handleSiteCheckAllChange: function(val) {
    this.permissionInfo.checkedSitePermissions = [];
    if (val) {
      for (var i = 0; i < this.permissionInfo.sitePermissions.length; i++){
        this.permissionInfo.checkedSitePermissions.push(this.permissionInfo.sitePermissions[i].name)
      }
    }
    this.permissionInfo.isSiteIndeterminate = false;
  },

  handleCheckedSitePermissionsChange: function(value) {
    var checkedCount = value.length;
    this.permissionInfo.siteCheckAll = checkedCount === this.permissionInfo.sitePermissions.length;
    this.permissionInfo.isSiteIndeterminate = checkedCount > 0 && checkedCount < this.permissionInfo.sitePermissions.length;
  },

  getSitePermissionText: function(name) {
    for (var i = 0; i < this.permissionInfo.sitePermissions.length; i++){
      if (this.permissionInfo.sitePermissions[i].name === name) {
        return this.permissionInfo.sitePermissions[i].text;
      }
    }
    return '';
  },

  handlePluginCheckAllChange: function(val) {
    this.permissionInfo.checkedPluginPermissions = [];
    if (val) {
      for (var i = 0; i < this.permissionInfo.pluginPermissions.length; i++){
        this.permissionInfo.checkedPluginPermissions.push(this.permissionInfo.pluginPermissions[i].name)
      }
    }
    this.permissionInfo.isPluginIndeterminate = false;
  },

  handleCheckedPluginPermissionsChange: function(value) {
    var checkedCount = value.length;
    this.permissionInfo.pluginCheckAll = checkedCount === this.permissionInfo.pluginPermissions.length;
    this.permissionInfo.isPluginIndeterminate = checkedCount > 0 && checkedCount < this.permissionInfo.pluginPermissions.length;
  },

  getPluginPermissionText: function(name) {
    for (var i = 0; i < this.permissionInfo.pluginPermissions.length; i++){
      if (this.permissionInfo.pluginPermissions[i].name === name) {
        return this.permissionInfo.pluginPermissions[i].text;
      }
    }
    return '';
  },

  handleChannelCheckAllChange: function(val) {
    this.permissionInfo.checkedChannelPermissions = [];
    if (val) {
      for (var i = 0; i < this.permissionInfo.channelPermissions.length; i++){
        this.permissionInfo.checkedChannelPermissions.push(this.permissionInfo.channelPermissions[i].name)
      }
    }
    this.permissionInfo.isChannelIndeterminate = false;
  },

  handleTreeChanged: function() {
    this.permissionInfo.checkedChannelIds = this.$refs.tree.getCheckedKeys();
  },

  handleCheckedChannelPermissionsChange: function(value) {
    var checkedCount = value.length;
    this.permissionInfo.channelCheckAll = checkedCount === this.permissionInfo.channelPermissions.length;
    this.permissionInfo.isChannelIndeterminate = checkedCount > 0 && checkedCount < this.permissionInfo.channelPermissions.length;
  },

  getChannelPermissionText: function(name) {
    for (var i = 0; i < this.permissionInfo.channelPermissions.length; i++){
      if (this.permissionInfo.channelPermissions[i].name === name) {
        return this.permissionInfo.channelPermissions[i].text;
      }
    }
    return '';
  },

  apiSubmit: function () {
    var $this = this;
    utils.loading(this, true);

    var sitePermissions = [];
    for (var i = 0; i < this.sites.length; i++){
      var site = this.sites[i];
      if (site.permissionInfo) {
        sitePermissions.push({
          siteId: site.id,
          channelIds: site.permissionInfo.checkedChannelIds,
          channelPermissions: site.permissionInfo.checkedChannelPermissions,
          websitePermissions: _.union(site.permissionInfo.checkedSitePermissions, site.permissionInfo.checkedPluginPermissions),
        });
      }
    }

    if (this.roleId > 0) {
      utils.loading(this, true);
      $api.put($url + '/' + this.roleId, {
        roleName: this.form.roleName,
        description: this.form.description,
        generalPermissions: this.form.checkedPermissions,
        sitePermissions: sitePermissions
      }).then(function (response) {
        var res = response.data;
  
        setTimeout(function() {
          location.href = utils.getSettingsUrl('administratorsRole');
        }, 1000);
        $this.$message.success('角色保存成功！');
      }).catch(function (error) {
        utils.error($this, error);
      }).then(function () {
        utils.loading($this, false);
      });
    } else {
      utils.loading(this, true);
      $api.post($url, {
        roleName: this.form.roleName,
        description: this.form.description,
        generalPermissions: this.form.checkedPermissions,
        sitePermissions: sitePermissions
      }).then(function (response) {
        var res = response.data;
  
        setTimeout(function() {
          location.href = utils.getSettingsUrl('administratorsRole');
        }, 1000);
        $this.$message.success('角色保存成功！');
      }).catch(function (error) {
        utils.error($this, error);
      }).then(function () {
        utils.loading($this, false);
      });
    }
  },

  btnSaveClick: function () {
    var $this = this;

    this.$refs.form.validate(function(valid) {
      if (valid) {
        $this.apiSubmit();
      }
    });
  },

  btnReturnClick: function () {
    location.href = utils.getSettingsUrl('administratorsRole');
  },

  btnSubmitClick: function () {
    if (this.checkedSiteIds.indexOf(this.site.id) === -1) {
      this.checkedSiteIds.push(this.site.id);
    }
    this.pageType = 'role';
  },

  btnCancelClick: function () {
    this.pageType = 'role';
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.getConfig();
  }
});