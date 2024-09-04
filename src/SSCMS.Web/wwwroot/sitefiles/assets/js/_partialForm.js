var _partialForm = {
  btnExtendAddClick: function (style) {
    var no = this.form[utils.getCountName(style.attributeName)] + 1;
    this.form[utils.getCountName(style.attributeName)] = no;
    this.form[utils.getExtendName(style.attributeName, no)] = "";
    this.form = _.assign({}, this.form);
  },

  btnExtendRemoveClick: function (style) {
    var no = this.form[utils.getCountName(style.attributeName)];
    this.form[utils.getCountName(style.attributeName)] = no - 1;
    this.form[utils.getExtendName(style.attributeName, no)] = "";
    this.form = _.assign({}, this.form);
  },

  btnExtendImageEditorClick: function (attributeName, no) {
    var imageUrl = this.form[utils.getExtendName(attributeName, no)];
    imageUrl = utils.getUrl(this.siteUrl, imageUrl);
    utils.addTab('编辑图片', utils.getCmsUrl('imageEditor', {
      siteId: this.siteId,
      attributeName: attributeName,
      no: no,
      imageUrl: imageUrl,
      tabName: utils.getTabName()
    }));
  },

  btnExtendPreviewClick: function (style, no) {
    var count = this.form[utils.getCountName(style.attributeName)];
    if (style.inputType === 'Image') {
      var data = [];
      for (var i = 0; i <= count; i++) {
        var imageUrl = this.form[utils.getExtendName(style.attributeName, i)];
        imageUrl = utils.getUrl(this.siteUrl, imageUrl);
        data.push({
          src: imageUrl,
        });
      }
      layer.photos({
        photos: {
          start: no,
          data: data,
        },
        anim: 5,
      });
    } else {
      var fileUrl = this.form[utils.getExtendName(style.attributeName, no)];
      if (fileUrl) {
        fileUrl = utils.getUrl(this.siteUrl, fileUrl);
        var fileName = fileUrl.split('/').pop();
        utils.openLayer({
          title: "文件预览：" + fileName,
          url: utils.getRootUrl('common/formLayerFilePreview', {
            fileUrl: fileUrl,
          }),
          full: true
        });
      }
    }
  },
}
