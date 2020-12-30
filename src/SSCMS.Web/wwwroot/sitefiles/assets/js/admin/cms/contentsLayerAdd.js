var $url = '/cms/contents/contentsLayerAdd';

var data = utils.init({
  checkedLevels: null,
  form: {
    siteId: utils.getQueryInt('siteId'),
    channelId: utils.getQueryInt('channelId'),
    titles: null,
    checkedLevel: null,
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
    }).then(function(response) {
      var res = response.data;

      $this.checkedLevels = res.checkedLevels;
      $this.form.checkedLevel = res.checkedLevel;
    }).catch(function(error) {
      utils.error(error);
    }).then(function() {
      utils.loading($this, false);
    });
  },

  apiSubmit: function() {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, this.form).then(function(response) {
      var res = response.data;

      utils.closeLayer();
      parent.$vue.apiList($this.form.channelId, 1, '批量添加成功！', true);
    }).catch(function(error) {
      utils.error(error);
    }).then(function() {
      utils.loading($this, false);
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
    this.apiGet();
  }
});