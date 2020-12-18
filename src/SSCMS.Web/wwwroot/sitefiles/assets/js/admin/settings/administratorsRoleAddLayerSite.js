var data = utils.init({
  roleId: utils.getQueryInt('roleId'),
  siteId: utils.getQueryInt('siteId'),
  isSite: false,
  site: null,
  permissionInfo: null,
  treeData: [],
  defaultExpandedKeys: []
});

var methods = {
  getPermissionInfo: function(res) {
    var sitePermissions = res.sitePermissions || [];
    var checkedSitePermissions = [];
    var allSitePermissions = [];
    for (var i = 0; i < sitePermissions.length; i++){
      allSitePermissions.push(sitePermissions[i].name)
      if (sitePermissions[i].selected){
        checkedSitePermissions.push(sitePermissions[i].name)
      }
    }

    var channelPermissions = res.channelPermissions || [];
    var checkedChannelPermissions = [];
    var allChannelPermissions = [];
    for (var i = 0; i < channelPermissions.length; i++){
      allChannelPermissions.push(channelPermissions[i].name)
      if (channelPermissions[i].selected){
        checkedChannelPermissions.push(channelPermissions[i].name)
      }
    }

    var contentPermissions = res.contentPermissions || [];
    var checkedContentPermissions = [];
    var allContentPermissions = [];
    for (var i = 0; i < contentPermissions.length; i++){
      allContentPermissions.push(contentPermissions[i].name)
      if (contentPermissions[i].selected){
        checkedContentPermissions.push(contentPermissions[i].name)
      }
    }

    var channel = res.channel;
    var checkedChannelIds = res.checkedChannelIds || [];

    return {
      siteId: this.siteId,
      sitePermissions: sitePermissions,
      siteCheckAll: false,
      isSiteIndeterminate: true,
      allSitePermissions: allSitePermissions,
      checkedSitePermissions: checkedSitePermissions,
  
      channelPermissions: channelPermissions,
      channelCheckAll: false,
      isChannelIndeterminate: true,
      allChannelPermissions: allChannelPermissions,
      checkedChannelPermissions: checkedChannelPermissions,

      contentPermissions: contentPermissions,
      contentCheckAll: false,
      isContentIndeterminate: true,
      allContentPermissions: allContentPermissions,
      checkedContentPermissions: checkedContentPermissions,
  
      channel: channel,
      checkedChannelIds: checkedChannelIds
    };
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

  handleChannelCheckAllChange: function(val) {
    this.permissionInfo.checkedChannelPermissions = [];
    if (val) {
      for (var i = 0; i < this.permissionInfo.channelPermissions.length; i++){
        this.permissionInfo.checkedChannelPermissions.push(this.permissionInfo.channelPermissions[i].name)
      }
    }
    this.permissionInfo.isChannelIndeterminate = false;
  },

  handleContentCheckAllChange: function(val) {
    this.permissionInfo.checkedContentPermissions = [];
    if (val) {
      for (var i = 0; i < this.permissionInfo.contentPermissions.length; i++){
        this.permissionInfo.checkedContentPermissions.push(this.permissionInfo.contentPermissions[i].name)
      }
    }
    this.permissionInfo.isContentIndeterminate = false;
  },

  handleTreeChanged: function() {
    this.permissionInfo.checkedChannelIds = this.$refs.tree.getCheckedKeys();
  },

  handleCheckedChannelPermissionsChange: function(value) {
    var checkedCount = value.length;
    this.permissionInfo.channelCheckAll = checkedCount === this.permissionInfo.channelPermissions.length;
    this.permissionInfo.isChannelIndeterminate = checkedCount > 0 && checkedCount < this.permissionInfo.channelPermissions.length;
  },

  handleCheckedContentPermissionsChange: function(value) {
    var checkedCount = value.length;
    this.permissionInfo.contentCheckAll = checkedCount === this.permissionInfo.contentPermissions.length;
    this.permissionInfo.isContentIndeterminate = checkedCount > 0 && checkedCount < this.permissionInfo.contentPermissions.length;
  },

  getChannelPermissionText: function(name) {
    for (var i = 0; i < this.permissionInfo.channelPermissions.length; i++){
      if (this.permissionInfo.channelPermissions[i].name === name) {
        return this.permissionInfo.channelPermissions[i].text;
      }
    }
    return '';
  },

  getContentPermissionText: function(name) {
    for (var i = 0; i < this.permissionInfo.contentPermissions.length; i++){
      if (this.permissionInfo.contentPermissions[i].name === name) {
        return this.permissionInfo.contentPermissions[i].text;
      }
    }
    return '';
  },

  setValues: function(res, sitePermission) {
    this.site = res.site;
    this.permissionInfo = this.getPermissionInfo(res);
    this.treeData = [this.permissionInfo.channel];
    this.defaultExpandedKeys = [this.permissionInfo.channel.id];

    utils.loading(this, false);
    if (!sitePermission) {
      this.isSite = false;
    } else {
      this.isSite = true;
    }

    if (!this.isSite) return;

    this.permissionInfo.checkedChannelIds = sitePermission.channelIds;
    this.permissionInfo.checkedChannelPermissions = sitePermission.channelPermissions;
    this.permissionInfo.checkedContentPermissions = sitePermission.contentPermissions;
    this.permissionInfo.checkedSitePermissions = sitePermission.permissions;
  },

  btnSubmitClick: function () {
    var sitePermissions = [];
    for(var i = 0; i < parent.$vue.sitePermissions.length; i++) {
      if (parent.$vue.sitePermissions[i].siteId != this.siteId) {
        sitePermissions.push(parent.$vue.sitePermissions[i]);
      }
    }

    if (this.isSite) {
      sitePermissions.push({
        siteId: this.siteId,
        channelIds: this.permissionInfo.checkedChannelIds,
        channelPermissions: this.permissionInfo.checkedChannelPermissions,
        contentPermissions: this.permissionInfo.checkedContentPermissions,
        permissions: _.union(this.permissionInfo.checkedSitePermissions),
      });
    }
    
    parent.$vue.sitePermissions = sitePermissions;
    utils.closeLayer();
  },

  btnCancelClick: function () {
    utils.closeLayer();
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods
});