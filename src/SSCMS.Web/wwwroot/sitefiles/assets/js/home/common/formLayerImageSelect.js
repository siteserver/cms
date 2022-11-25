var $url = '/common/form/layerImageSelect';

var data = utils.init({
  inputType: utils.getQueryString('inputType'),
  attributeName: utils.getQueryString('attributeName'),
  no: utils.getQueryInt('no'),

  count: 0,
  images: [],
  formInline: {
    siteId: utils.getQueryInt('siteId'),
    keyword: "",
    page: 1,
    perPage: 28,
  },
});

var methods = {
  insert: function(imageUrl) {
    if (this.inputType === 'Image') {
      if (parent.$vue.runMaterialLayerImageSelect) {
        parent.$vue.runMaterialLayerImageSelect(this.attributeName, this.no, imageUrl);
      }
    } else if (this.inputType === 'TextEditor') {
      if (parent.$vue.runEditorLayerImage) {
        var html = '<img src="' + imageUrl + '" style="border: 0; max-width: 100%" />';
        parent.$vue.runEditorLayerImage(this.attributeName, html);
      }
    }
  },

  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
        params: this.formInline,
      })
      .then(function (response) {
        var res = response.data;

        $this.count = res.count;
        for (var image of res.images) {
          $this.images.push(image);
        }
      })
      .catch(function (error) {
        utils.error(error);
      })
      .then(function () {
        utils.loading($this, false);
      });
  },

  btnSearchClick: function () {
    this.count = 0;
    this.images = [];
    this.apiGet();
  },

  isMore: function () {
    return this.count > this.formInline.page * this.formInline.perPage;
  },

  btnMoreClick: function() {
    this.formInline.page++;
    this.apiGet();
  },

  btnImageClick: function(image) {
    this.insert(image.imageUrl);
    this.btnCancelClick();
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
    utils.keyPress(null, this.btnCancelClick);
    this.apiGet();
  }
});
