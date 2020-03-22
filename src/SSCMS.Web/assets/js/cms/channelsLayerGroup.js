var $url = '/admin/cms/channels/channelsLayerGroup';

var data = utils.initData({
  page: utils.getQueryInt('page'),
  groupNames: null,
  isAddForm: false,
  form: {
    siteId: utils.getQueryInt('siteId'),
    channelIds: utils.getQueryIntList('channelIds'),
    isCancel: false,
    groupNames: [],
  },
  addForm: {
    siteId: utils.getQueryInt('siteId'),
    channelIds: utils.getQueryIntList('channelIds'),
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
        siteId: this.form.siteId
      }
    }).then(function (response) {
      var res = response.data;

      $this.groupNames = res.value;
      if (!$this.groupNames || $this.groupNames.length === 0) {
        $this.isAddForm = true;
      }
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiAdd: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url + '/actions/add', this.addForm).then(function (response) {
      var res = response.data;

      parent.$vue.apiList('栏目组设置成功!', res);
      utils.closeLayer();
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, this.form).then(function (response) {
      var res = response.data;

      parent.$vue.apiList('栏目组设置成功!', res);
      utils.closeLayer();
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnSubmitClick: function () {
    if (this.isAddForm) {
      var $this = this;
      this.$refs.addForm.validate(function(valid) {
        if (valid) {
          $this.apiAdd();
        }
      });
    } else {
      if (this.form.groupNames.length === 0) {
        return this.$message.error('请选择栏目组！');
      }
      this.apiSubmit();
    }
  },

  btnCancelClick: function () {
    if (this.isAddForm && this.groupNames) {
      this.isAddForm = false;
    } else {
      utils.closeLayer();
    }
  },
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiGet();
  }
});