var $url = '/pages/cms/settings/settingsContentGroup';

var data = utils.initData({
  siteId: utils.getQueryInt("siteId"),
  groups: null,

  panel: false,
  form: null
});

var methods = {
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

  apiSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    if (this.form.id === 0) {
      $api.post($url, this.form).then(function (response) {
        var res = response.data;
        
        $this.groups = res.groups;
        $this.$message.success('内容组添加成功！');
        $this.panel = false;
      }).catch(function (error) {
        utils.error($this, error);
      }).then(function () {
        utils.loading($this, false);
      });
    } else {
      $api.put($url, this.form).then(function (response) {
        var res = response.data;
        
        $this.groups = res.groups;
        $this.$message.success('内容组修改成功！');
        $this.panel = false;
      }).catch(function (error) {
        utils.error($this, error);
      }).then(function () {
        utils.loading($this, false);
      });
    }
  },

  btnEditClick: function (group) {
    this.panel = true;
    this.form = _.assign({}, group);
  },

  btnAddClick: function () {
    this.panel = true;
    this.form = {
      id: 0,
      siteId: this.siteId,
      groupName: '',
      description: ''
    };
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
      $this.panel = false;
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnSubmitClick: function () {
    var $this = this;
    this.$refs.form.validate(function(valid) {
      if (valid) {
        $this.apiSubmit();
      }
    });
  },

  btnCancelClick: function () {
    this.panel = false;
  }
};

new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiList();
  }
});