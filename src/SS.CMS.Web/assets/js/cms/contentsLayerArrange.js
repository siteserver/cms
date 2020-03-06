var $url = '/admin/cms/contents/contentsLayerArrange';

var data = utils.initData({
  page: utils.getQueryInt('page'),
  form: {
    siteId: utils.getQueryInt('siteId'),
    channelId: utils.getQueryInt('channelId'),
    attributeName: 'Id',
    isDesc: true
  }
});

var methods = {
  apiSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, this.form).then(function (response) {
      var res = response.data;

      parent.$vue.apiList($this.form.channelId, $this.page, '整理排序成功!');
      utils.closeLayer();
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
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
    utils.loading(this, false);
  }
});