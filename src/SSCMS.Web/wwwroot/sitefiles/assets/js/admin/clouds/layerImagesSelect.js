var $urlCloud = 'cms/images';

var data = utils.init({
  inputType: utils.getQueryString('inputType'),
  attributeName: utils.getQueryString('attributeName'),
  no: utils.getQueryInt('no'),

  count: 0,
  images: [],
  formInline: {
    keyword: "",
    page: 1,
    perPage: 28,
  },
  imageData: [],
  dialogVisible: false,
  image: null,
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

  getSmallUrl: function (image) {
    return cloud.hostImages + '/' + image.smallUrl;
  },

  getRegularUrl: function (image) {
    return cloud.hostImages + '/' + image.regularUrl;
  },

  apiCloudGet: function () {
    var $this = this;

    utils.loading(this, true);
    cloud.get($urlCloud, {
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
      utils.error(error, {
        ignoreAuth: true,
      });
    })
    .then(function () {
      utils.loading($this, false);
    });
  },

  btnSearchClick: function () {
    this.count = 0;
    this.images = [];
    this.apiCloudGet();
  },

  isMore: function () {
    return this.count > this.formInline.page * this.formInline.perPage;
  },

  btnMoreClick: function() {
    this.formInline.page++;
    this.apiCloudGet();
  },

  btnImageClick: function(image) {
    this.image = image;
    this.imageData = [{
      type: 'thumb',
      size: '200 x ' + (this.image.height * (200 / this.image.width)).toFixed(0),
    }, {
      type: 'small',
      size: '400 x ' + (this.image.height * (400 / this.image.width)).toFixed(0),
    }, {
      type: 'regular',
      size: '1080 x ' + (this.image.height * (1080 / this.image.width)).toFixed(0),
    }];
    this.dialogVisible = true;
  },

  getImageType: function(type) {
    if (type == 'thumb') {
      return '小尺寸';
    } else if (type == 'small') {
      return '中尺寸';
    } else if (type == 'regular') {
      return '大尺寸';
    }
    return '';
  },

  getImageUrl: function (type) {
    var url = this.image.regularUrl;
    if (type == 'thumb') {
      url = this.image.thumbUrl;
    } else if (type == 'small') {
      url = this.image.smallUrl;
    } else if (type == 'regular') {
      url = this.image.regularUrl;
    } else if (type == 'full') {
      url = this.image.fullUrl;
    }
    return cloud.hostImages + '/' + url;
  },

  btnViewClick: function(type) {
    window.open(this.getImageUrl(type));
  },

  btnSelectClick: function(type) {
    this.insert(this.getImageUrl(type));
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
    this.apiCloudGet();
  }
});
