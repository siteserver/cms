var data = utils.init({
  videoUrl: utils.getQueryString('videoUrl'),
});

var methods = {
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
    utils.loading(this, false);
  }
});
