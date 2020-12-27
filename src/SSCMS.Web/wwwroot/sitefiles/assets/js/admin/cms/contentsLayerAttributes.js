var $url = '/cms/contents/contentsLayerAttributes';

var data = utils.init({
  page: utils.getQueryInt('page'),
  form: {
    siteId: utils.getQueryInt('siteId'),
    channelId: utils.getQueryInt('channelId'),
    channelContentIds: utils.getQueryString('channelContentIds'),
    isCancel: false,
    isTop: false,
    isRecommend: false,
    isHot: false,
    isColor: false
  }
});

var methods = {
  apiSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, this.form).then(function (response) {
      var res = response.data;

      utils.success('内容属性设置成功!');
      parent.$vue.apiList($this.form.channelId, $this.page);
      utils.closeLayer();
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnSubmitClick: function () {
    if (!this.form.isTop && !this.form.isRecommend && !this.form.isHot && !this.form.isColor){
      return utils.error('请选择内容属性！');
    }
    this.apiSubmit();
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