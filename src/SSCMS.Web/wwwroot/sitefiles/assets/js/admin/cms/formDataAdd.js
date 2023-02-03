var $url = '/cms/forms/formDataAdd';
var $urlDeleteFile = $url + '/actions/deleteFile';

var data = utils.init({
  siteId: utils.getQueryInt('siteId'),
  formId: utils.getQueryInt('formId'),
  dataId: utils.getQueryInt('dataId'),
  navType: 'Data',
  siteUrl: null,
  styles: [],
  uploadUrl: null,
  files: [],
  form: null,
});

var methods = {
  runFormLayerImageUploadText: function(attributeName, no, text) {
    this.insertText(attributeName, no, text);
  },

  runFormLayerFileUpload: function(attributeName, no, text) {
    this.insertText(attributeName, no, text);
  },

  runMaterialLayerFileSelect: function(attributeName, no, text) {
    this.insertText(attributeName, no, text);
  },

  insertText: function(attributeName, no, text) {
    var count = this.form[utils.getCountName(attributeName)] || 0;
    if (count <= no) {
      this.form[utils.getCountName(attributeName)] = no;
    }
    this.form[utils.getExtendName(attributeName, no)] = text;
    this.form = _.assign({}, this.form);
  },

  getUploadUrl: function(style) {
    return this.uploadUrl + '&fieldId=' + style.id;
  },

  imageUploaded: function(error, file) {
    if (!error) {
      var res = JSON.parse(file.serverId);
      var style = _.find(this.styles, function(o) { return o.id === res.fieldId; });
      style.value = res.value;
    }
  },

  imageRemoved: function(style) {
    style.value = [];
  },

  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.siteId,
        formId: this.formId,
        dataId: this.dataId
      }
    }).then(function (response) {
      var res = response.data;

      $this.siteUrl = res.siteUrl;
      $this.styles = res.styles;
      $this.form = utils.getForm(res.styles, res.formData);
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(true);
    $api.post($url + '?siteId=' + this.siteId, this.form).then(function (response) {
      var res = response.data;

      utils.success('数据保存成功！');
      $this.navType = 'Data';
      $this.btnNavClick();

    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  getValue: function (attributeName) {
    for (var i = 0; i < this.styles.length; i++) {
      var style = this.styles[i];
      if (style.attributeName === attributeName) {
        return style.value;
      }
    }
    return '';
  },

  setValue: function (attributeName, value) {
    for (var i = 0; i < this.styles.length; i++) {
      var style = this.styles[i];
      if (style.attributeName === attributeName) {
        style.value = value;
      }
    }
  },

  btnImageClick: function (imageUrl) {
    top.utils.openImagesLayer([imageUrl]);
  },

  btnSubmitClick: function () {
    var $this = this;
    this.$refs.form.validate(function(valid) {
      if (valid) {
        $this.apiSubmit();
      }
    });
  },

  btnReturnClick: function() {
    this.navType = 'Data';
    this.btnNavClick();
  },

  btnNavClick: function() {
    location.href = utils.getCmsUrl('form' + this.navType, {
      siteId: this.siteId,
      formId: this.formId
    });
  },

  btnExtendPreviewClick: function(attributeName, no) {
    var data = [];
    var imageUrl = this.form[utils.getExtendName(attributeName, no)];
    imageUrl = utils.getUrl(this.siteUrl, imageUrl);
    data.push({
      "src": imageUrl
    });
    layer.photos({
      photos: {
        "start": no,
        "data": data
      }
      ,anim: 5
    });
  },

  btnLayerClick: function(options) {
    var query = {
      siteId: this.siteId,
      attributeName: options.attributeName
    };
    if (options.no) {
      query.no = options.no;
    }

    var args = {
      title: options.title,
      url: utils.getCommonUrl(options.name, query)
    };
    if (!options.full) {
      args.width = options.width ? options.width : 700;
      args.height = options.height ? options.height : 500;
    }
    utils.openLayer(args);
  },
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    utils.keyPress(this.btnSubmitClick, this.btnReturnClick);
    this.apiGet();
  }
});
