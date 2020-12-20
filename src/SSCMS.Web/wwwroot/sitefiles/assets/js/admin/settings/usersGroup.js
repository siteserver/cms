var $url = '/settings/usersGroup';

var data = utils.init({
  groups: null,
  adminNames: null,

  panel: false,
  form: null
});

var methods = {
  apiList: function (message) {
    var $this = this;

    utils.loading(this, true);
    $api.get($url).then(function (response) {
      var res = response.data;

      $this.groups = res.groups;
      $this.adminNames = res.adminNames;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
      if (message) {
        utils.success(message);
      }
    });
  },

  apiDelete: function (id) {
    var $this = this;

    utils.loading(this, true);
    $api.delete($url, {
      data: { id: id}
    }).then(function (response) {
      var res = response.data;

      $this.groups = res.groups;
      utils.success('用户组删除成功！');
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, this.form).then(function (response) {
      var res = response.data;
      
      $this.groups = res.groups;
      utils.success($this.form.id === -1 ? '用户组添加成功！' : '用户组修改成功！');
      $this.panel = false;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnAdminClick: function(adminName) {
    utils.openLayer({
      title: "管理员查看",
      url: utils.getCommonUrl('adminLayerView', {userName: adminName}),
      full: true
    });
  },

  btnEditClick: function (group) {
    this.panel = true;
    this.form = _.assign({}, group);
  },

  btnAddClick: function () {
    this.panel = true;
    this.form = {
      id: -1,
      groupName: '',
      adminName: ''
    };
  },

  btnDeleteClick: function (group) {
    var $this = this;

    utils.alertDelete({
      title: '删除用户组',
      text: '此操作将删除用户组 ' + group.groupName + '，确定吗？',
      callback: function () {
        $this.apiDelete(group.id);
      }
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

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiList();
  }
});