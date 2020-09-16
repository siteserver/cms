var $url = '/common/groupContentLayerAdd';

var data = utils.init({
  siteId: utils.getQueryInt('siteId'),
  groupId: utils.getQueryInt('groupId'),
  form: {
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
        siteId: this.siteId,
        groupId: this.groupId
      }
    }).then(function (response) {
      var res = response.data;

      $this.form.groupName = res.groupName;
      $this.form.description = res.description;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    if (this.groupId === 0) {
      $api.post($url, {
        siteId: this.siteId,
        groupName: this.form.groupName,
        description: this.form.description
      }).then(function (response) {
        var res = response.data;
  
        parent.$vue.updateGroups(res, $this.getSuccessMessage());
        utils.closeLayer();
      }).catch(function (error) {
        utils.error(error);
      }).then(function () {
        utils.loading($this, false);
      });
    } else {
      $api.put($url, {
        siteId: this.siteId,
        groupId: this.groupId,
        groupName: this.form.groupName,
        description: this.form.description
      }).then(function (response) {
        var res = response.data;
  
        parent.$vue.updateGroups(res, $this.getSuccessMessage());
        utils.closeLayer();
      }).catch(function (error) {
        utils.error(error);
      }).then(function () {
        utils.loading($this, false);
      });
    }
  },

  getSuccessMessage: function () {
    return '内容组' + (this.groupId > 0 ? '修改成功！' : '添加成功！');
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
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    if (this.groupId > 0) {
      this.apiGet();
    } else {
      utils.loading(this, false);
    }
  }
});