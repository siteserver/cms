var $url = '/cms/library/layerGroupAdd';

var data = utils.init({
  siteId: utils.getQueryInt('siteId'),
  groupId: utils.getQueryInt('groupId'),
  libraryType: utils.getQueryString('libraryType'),
  form: {
    groupName: ''
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

      $this.form.groupName = res.value;
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
        libraryType: this.libraryType,
        groupName: this.form.groupName,
      }).then(function (response) {
        var res = response.data;
  
        utils.success('素材分组添加成功！');
        parent.$vue.runUpdateGroups(res.groups);
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
        groupName: this.form.groupName
      }).then(function (response) {
        var res = response.data;
  
        utils.success('素材分组修改成功！');
        parent.$vue.runUpdateGroups(res.groups);
        utils.closeLayer();
      }).catch(function (error) {
        utils.error(error);
      }).then(function () {
        utils.loading($this, false);
      });
    }
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