var $url = '/pages/settings/userGroup';

var $data = {
  pageLoad: false,
  pageAlert: null,
  pageType: 'list',
  items: null,
  adminNames: null,
};

var $methods = {
  getList: function () {
    var $this = this;

    $api.get($url).then(function (response) {
      var res = response.data;

      $this.items = res.value;
      $this.adminNames = res.adminNames;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
    });
  },

  delete: function (id) {
    var $this = this;

    utils.loading(true);
    $api.delete($url, {
      params: {
        id: id
      }
    }).then(function (response) {
      var res = response.data;

      $this.items = res.value;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  submit: function (item) {
    var $this = this;

    utils.loading(true);
    $api.post($url, item).then(function (response) {
      var res = response.data;

      $this.pageAlert = {
        type: 'success',
        html: item.id === -1 ? '用户组添加成功！' : '用户组修改成功！'
      };
      $this.item = null;
      $this.items = res.value;
      $this.pageType = 'list';
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  btnEditClick: function (item) {
    this.pageAlert = null;
    this.pageType = 'add';
    this.item = item;
  },

  btnAddClick: function () {
    this.pageAlert = null;
    this.pageType = 'add';
    this.item = {
      id: -1,
      groupName: '',
      adminName: ''
    };
  },

  btnDeleteClick: function (item) {
    var $this = this;

    swal2({
        title: '删除用户组',
        text: '此操作将删除用户组 ' + item.groupName + '，确定吗？',
        type: 'question',
        confirmButtonText: '删 除',
        confirmButtonClass: 'btn btn-danger',
        showCancelButton: true,
        cancelButtonText: '取 消'
      })
      .then(function (result) {
        if (result.value) {
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
    this.pageAlert = null;
    this.pageType = 'list';
  }
};

new Vue({
  el: '#main',
  data: $data,
  methods: $methods,
  created: function () {
    this.getList();
  }
});