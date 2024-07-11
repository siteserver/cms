var $url = '/settings/usersLayerGroups';

var data = utils.init({
  page: utils.getQueryInt('page'),
  userIds: utils.getQueryString('userIds'),
  users: null,
  groups: null,
  isAddForm: false,
  form: {
    userIds: utils.getQueryString('userIds'),
    isCancel: false,
    groupIds: [],
  },
  addForm: {
    userIds: utils.getQueryString('userIds'),
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

  apiSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, {
      userIds: this.userIds,
      groupIds: this.form.groupIds,
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

  btnViewClick: function(userId) {
    utils.openLayer({
      title: '查看资料',
      url: utils.getCommonUrl('userLayerView', {userId: userId})
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
