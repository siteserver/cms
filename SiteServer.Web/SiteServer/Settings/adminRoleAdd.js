var $api = new apiUtils.Api(apiUrl + '/pages/settings/adminRoleAdd');
var $roleId = pageUtils.getQueryInt('roleId');

var data = {
  pageLoad: false,
  pageAlert: null,
  pageType: null,
  roleName: null,
  description: null,
  permissions: null,
  allPermissions: null,
  checkedPermissions: null,
  systemCheckAll: false,
  isSystemIndeterminate: true,
  siteInfoList: null,
  checkedSiteIdList: null,
  siteInfo: null,
  
  permissionInfo: null
};

var methods = {
  getConfig: function () {
    var $this = this;

    $api.get({
      roleId: $roleId
    }, function (err, res) {
      if (err || !res || !res.value) return;

      if (res.roleInfo) {
        $this.roleName = res.roleInfo.roleName;
        $this.description = res.roleInfo.description;
      }
      $this.permissions = res.permissions;
      $this.checkedPermissions = [];
      $this.allPermissions = [];
      for (var i = 0; i < res.permissions.length; i++){
        $this.allPermissions.push(res.permissions[i].name)
        if (res.permissions[i].selected){
          $this.checkedPermissions.push(res.permissions[i].name)
        }
      }
      $this.siteInfoList = res.siteInfoList;
      $this.checkedSiteIdList = res.checkedSiteIdList || [];

      for (var i = 0; i < res.sitePermissionsList.length; i++){
        var permissionInfo = $this.getPermissionInfo(res.sitePermissionsList[i]);
        for (var j = 0; j < res.siteInfoList.length; j++){
          var siteInfo = res.siteInfoList[j];
          if (siteInfo.id === permissionInfo.siteId) {
            siteInfo.permissionInfo = permissionInfo;
          }
        }
      }

      $this.pageType = 'role';
      $this.pageLoad = true;
    });
  },

  handleSystemCheckAllChange: function(val) {
    this.checkedPermissions = [];
    if (val) {
      for (var i = 0; i < this.permissions.length; i++){
        this.checkedPermissions.push(this.permissions[i].name)
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

    var channelInfo = res.channelInfo;
    var checkedChannelIdList = res.checkedChannelIdList || [];

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
  
      channelInfo: channelInfo,
      checkedChannelIdList: checkedChannelIdList
    };
  },

  btnPermissionClick: function(siteInfo) {
    this.siteInfo = siteInfo;
    if (this.siteInfo.permissionInfo) {
      this.permissionInfo = this.siteInfo.permissionInfo;
      this.pageType = 'permissions';
      return;
    }

    pageUtils.loading(true);
    var $this = this;

    $api.getAt(this.siteInfo.id, {
      roleId: $roleId
    }, function (err, res) {
      pageUtils.loading(false);
      if (err || !res || !res.value) return;

      $this.siteInfo.permissionInfo = $this.getPermissionInfo(res);
      $this.permissionInfo = $this.siteInfo.permissionInfo;
      $this.pageType = 'permissions';
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
    this.permissionInfo.checkedChannelIdList = this.$refs.tree.getCheckedKeys();
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
    pageUtils.loading(true);

    var sitePermissions = [];
    for (var i = 0; i < this.siteInfoList.length; i++){
      var siteInfo = this.siteInfoList[i];
      if (siteInfo.permissionInfo) {
        sitePermissions.push({
          siteId: siteInfo.id,
          channelIdCollection: _.join(siteInfo.permissionInfo.checkedChannelIdList, ','),
          channelPermissions: _.join(siteInfo.permissionInfo.checkedChannelPermissions, ','),
          websitePermissions: _.join(_.union(siteInfo.permissionInfo.checkedSitePermissions, siteInfo.permissionInfo.checkedPluginPermissions), ','),
        });
      }
    }

    if ($roleId > 0) {
      $api.putAt($roleId, {
        roleName: this.roleName,
        description: this.description,
        generalPermissions: this.checkedPermissions,
        sitePermissions: sitePermissions
      }, function (err, res) {
        pageUtils.loading(false);
        if (err) {
          $this.pageAlert = {
            type: 'danger',
            html: err.message
          };
          return;
        }
  
        $this.pageAlert = {
          type: 'success',
          html: '角色保存成功！'
        };
        setTimeout(function() {
          location.href = 'adminRole.cshtml';
        }, 1000);
      });
    } else {
      $api.post({
        roleName: this.roleName,
        description: this.description,
        generalPermissions: this.checkedPermissions,
        sitePermissions: sitePermissions
      }, function (err, res) {
        pageUtils.loading(false);
        if (err) {
          $this.pageAlert = {
            type: 'danger',
            html: err.message
          };
          return;
        }
  
        $this.pageAlert = {
          type: 'success',
          html: '角色保存成功！'
        };
        setTimeout(function() {
          location.href = 'adminRole.cshtml';
        }, 1000);
      });
    }
  },

  btnSaveClick: function () {
    var $this = this;
    this.$validator.validate().then(function (result) {
      if (result) {
        $this.apiSubmit();
      }
    });
  },

  btnReturnClick: function () {
    location.href = 'adminRole.cshtml';
  },

  btnSubmitClick: function () {
    if (this.checkedSiteIdList.indexOf(this.siteInfo.id) === -1) {
      this.checkedSiteIdList.push(this.siteInfo.id);
    }
    this.pageType = 'role';
  },

  btnCancelClick: function () {
    this.pageType = 'role';
  }
};

new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.getConfig();
  }
});