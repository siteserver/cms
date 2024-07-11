var $url = '/settings/usersLayerDepartments';

var data = utils.init({
  page: utils.getQueryInt('page'),
  departmentId: utils.getQueryInt('departmentId'),
  userIds: utils.getQueryString('userIds'),
  users: null,
  transDepartments: null,
  form: {
    transDepartmentIds: null,
  }
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        departmentId: this.departmentId,
        userIds: this.userIds
      }
    }).then(function (response) {
      var res = response.data;

      $this.users = res.users;
      $this.apiGetOptions();
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
      siteId: this.siteId,
      departmentId: this.departmentId,
      userIds: this.userIds,
      transDepartmentId: this.form.transDepartmentIds[this.form.transDepartmentIds.length - 1],
    }).then(function (response) {
      var res = response.data;

      parent.$vue.runReload('成员转移成功!');
      utils.closeLayer();
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiGetOptions: function() {
    var $this = this;

    $api.post($url + '/actions/options', {
      departmentId: this.departmentId,
    }).then(function (response) {
      var res = response.data;

      $this.transDepartments = res.transDepartments;
      $this.form.transDepartmentIds = null;
    }).catch(function (error) {
      utils.loading($this, false);
      utils.error(error);
    });
  },

  handleTransSiteIdChange: function() {
    this.apiGetOptions();
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
