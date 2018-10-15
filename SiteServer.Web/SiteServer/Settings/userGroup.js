var $api = new apiUtils.Api(apiUrl + '/pages/settings/userGroup');

var data = {
  pageLoad: false,
  pageAlert: null,
  pageType: 'list',
  items: null,
  adminNames: null,
};

var methods = {
  getList: function () {
    var $this = this;

    $api.get(null, function (err, res) {
      if (err || !res || !res.value) return;

      $this.items = res.value;
      $this.adminNames = res.adminNames;

      $this.pageLoad = true;
    });
  },
  delete: function (id) {
    var $this = this;

    pageUtils.loading(true);
    $api.delete({
      id: id
    }, function (err, res) {
      pageUtils.loading(false);
      if (err || !res || !res.value) return;

      $this.items = res.value;
    });
  },
  submit: function (item) {
    var $this = this;

    pageUtils.loading(true);
    $api.post(item, function (err, res) {
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
        html: item.id === -1 ? '用户组添加成功！' : '用户组修改成功！'
      };
      $this.item = null;
      $this.items = res.value;
      $this.pageType = 'list';
    });
  },
  btnEditClick: function (item) {
    this.pageType = 'add';
    this.item = item;
  },
  btnAddClick: function () {
    this.pageType = 'add';
    this.item = {
      id: -1,
      groupName: '',
      adminName: ''
    };
  },
  btnDeleteClick: function (item) {
    var $this = this;

    pageUtils.alertDelete({
      title: '删除用户组',
      text: '此操作将删除用户组 ' + item.groupName + '，确定吗？',
      callback: function () {
        $this.delete(item.id);
      }
    });
  },
  btnSubmitClick: function () {
    var $this = this;
    this.$validator.validate().then(function (result) {
      if (result) {
        $this.submit($this.item);
      }
    });
  },
  btnCancelClick: function () {
    this.pageType = 'list';
  }
};

new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.getList();
  }
});