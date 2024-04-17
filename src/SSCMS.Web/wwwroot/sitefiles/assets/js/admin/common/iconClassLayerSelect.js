var data = utils.init({
  activeName: 'first',
});

var methods = {
  btnIconClick: function (iconClass) {
    parent.$vue.runIconClassSelect(iconClass);
    utils.closeLayer();
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
    utils.keyPress(null, this.btnCancelClick);
    utils.loading(this, false);
  }
});
