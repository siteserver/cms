var $url = '/admin/cms/settings/settingsContentGroup';

var data = utils.initData({
  siteId: utils.getQueryInt("siteId"),
  groups: null
});

var methods = {
  updateGroups: function(res, message) {
    this.groups = res.groups;
    this.$message.success(message);
  },
  
  apiList: function (message) {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.siteId
      }
    }).then(function (response) {
      var res = response.data;

      $this.groups = res.groups;
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
      if (message) {
        $this.$message.success(message);
      }
    });
  },

  apiDelete: function (groupName) {
    var $this = this;

    utils.loading(this, true);
    $api.delete($url, {
      data: {
        siteId: this.siteId,
        groupName: groupName
      }
    }).then(function (response) {
      var res = response.data;

      $this.groups = res.groups;
      $this.$message.success('内容组删除成功！');
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnEditClick: function (group) {
    utils.openLayer({
      title: '编辑内容组',
      url: utils.getSharedUrl('groupContentLayerAdd', {
        siteId: this.siteId,
        groupId: group.id
      }),
      width: 500,
      height: 300
    });
  },

  btnAddClick: function () {
    utils.openLayer({
      title: '新增内容组',
      url: utils.getSharedUrl('groupContentLayerAdd', {
        siteId: this.siteId
      }),
      width: 500,
      height: 300
    });
  },

  btnDeleteClick: function (group) {
    var $this = this;

    utils.alertDelete({
      title: '删除内容组',
      text: '此操作将删除内容组 ' + group.groupName + '，确定吗？',
      callback: function () {
        $this.apiDelete(group.groupName);
      }
    });
  },

  btnOrderClick: function(group, isUp) {
    var $this = this;

    utils.loading(this, true);
    $api.post($url + '/actions/order', {
      siteId: this.siteId,
      groupId: group.id,
      taxis: group.taxis,
      isUp: isUp
    }).then(function (response) {
      var res = response.data;
      
      $this.groups = res.groups;
      $this.$message.success('内容组排序成功！');
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
    this.apiList();
  }
});