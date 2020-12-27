var $url = '/cms/contents/contentsLayerGroup';

var data = utils.init({
  page: utils.getQueryInt('page'),
  groupNames: null,
  isAddForm: false,
  form: {
    siteId: utils.getQueryInt('siteId'),
    channelId: utils.getQueryInt('channelId'),
    channelContentIds: utils.getQueryString('channelContentIds'),
    isCancel: false,
    groupNames: [],
  },
  addForm: {
    siteId: utils.getQueryInt('siteId'),
    channelId: utils.getQueryInt('channelId'),
    channelContentIds: utils.getQueryString('channelContentIds'),
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
        siteId: this.form.siteId,
        channelId: this.form.channelId
      }
    }).then(function (response) {
      var res = response.data;

      $this.groupNames = res.value;
      if (!$this.groupNames || $this.groupNames.length === 0) {
        $this.isAddForm = true;
      }
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiAdd: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url + '/actions/add', this.addForm).then(function (response) {
      var res = response.data;

      utils.success('内容组设置成功!');
      parent.$vue.apiList($this.form.channelId, $this.page);
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
    $api.post($url, this.form).then(function (response) {
      var res = response.data;

      utils.success('内容组设置成功!');
      parent.$vue.apiList($this.form.channelId, $this.page);
      utils.closeLayer();
    }).catch(function (error) {
      utils.error(error);
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
        return utils.error('请选择内容组！');
      }
      this.apiSubmit();
    }
  },

  btnCancelClick: function () {
    if (this.isAddForm && this.groupNames && this.groupNames.length > 0) {
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