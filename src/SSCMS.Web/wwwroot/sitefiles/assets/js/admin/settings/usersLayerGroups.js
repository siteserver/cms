var $url = '/settings/usersLayerGroups';
var $urlAdd = $url + '/actions/add';

var data = utils.init({
  page: utils.getQueryInt('page'),
  userIds: utils.getQueryString('userIds'),
  users: null,
  groups: null,
  isAddForm: false,
  form: {
    isCancel: false,
    groupIds: [],
  },
  addForm: {
    groupName: '',
    description: ''
  }
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        userIds: this.userIds
      }
    }).then(function (response) {
      var res = response.data;

      $this.users = res.users;
      $this.groups = res.groups;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiAdd: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlAdd, {
      userIds: this.userIds,
      groupName: this.addForm.groupName,
      description: this.addForm.description,
    }).then(function (response) {
      var res = response.data;

      parent.$vue.runReload('用户组设置成功!');
      utils.closeLayer();
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, {
      userIds: this.userIds,
      groupIds: this.form.groupIds,
      isCancel: this.form.isCancel,
    }).then(function (response) {
      var res = response.data;

      parent.$vue.runReload('用户组设置成功!');
      utils.closeLayer();
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnViewClick: function(user) {
    utils.openLayer({
      title: '查看资料',
      url: utils.getCommonUrl('userLayerView', { guid: user.guid })
    });
  },

  btnSubmitClick: function () {
    var $this = this;
    if (this.isAddForm)  {
      this.$refs.addForm.validate(function(valid) {
        if (valid) {
          $this.apiAdd();
        }
      });
    } else {
      this.$refs.form.validate(function(valid) {
        if (valid) {
          $this.apiSubmit();
        }
      });
    }
  },

  btnCancelClick: function () {
    utils.closeLayer();
  },
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    utils.keyPress(this.btnSubmitClick, this.btnCancelClick);
    this.apiGet();
  }
});
