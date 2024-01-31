var $url = '/cms/contents/contentsLayerImport';

var data = utils.init({
  checkedLevels: null,
  form: {
    siteId: utils.getQueryInt('siteId'),
    channelId: utils.getQueryInt('channelId'),
    importType: 'zip',
    checkedLevel: null,
    isOverride: false,
    fileNames: [],
    fileUrls: [],
    attributes: [],
  },
  uploadUrl: null,
  uploadList: [],
  orderedFileNames: [],
  columns: [],
  styles: [],
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
    }).then(function (response) {
      var res = response.data;

      $this.checkedLevels = res.checkedLevels;
      $this.form.checkedLevel = res.value;
      $this.form.importType = res.options.importType;
      $this.form.isOverride = res.options.isOverride;

      $this.btnRadioInput();
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function () {
    var $this = this;

    var fileNames = [];
    var fileUrls = [];
    for (var i = 0; i < this.orderedFileNames.length; i++) {
      var fileName = this.orderedFileNames[i];
      var index = this.form.fileNames.indexOf(fileName);
      if (index !== -1) {
        fileNames.push(this.form.fileNames[index]);
        fileUrls.push(this.form.fileUrls[index]);
      }
    }
    for (var i = 0; i < this.form.fileNames.length; i++) {
      var fileName = this.form.fileNames[i];
      var index = this.orderedFileNames.indexOf(fileName);
      if (index === -1) {
        fileNames.push(this.form.fileNames[i]);
        fileUrls.push(this.form.fileUrls[i]);
      }
    }
    fileNames.reverse();
    fileUrls.reverse();

    utils.loading(this, true);
    $api.post($url, {
      siteId: this.form.siteId,
      channelId: this.form.channelId,
      importType: this.form.importType,
      checkedLevel: this.form.checkedLevel,
      isOverride: this.form.isOverride,
      fileNames: fileNames,
      fileUrls: fileUrls,
      attributes: this.form.attributes,
    }).then(function (response) {
      var res = response.data;

      utils.closeLayer();
      parent.$vue.apiList($this.form.channelId, 1, '文件导入成功！', true);
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnRadioInput: function () {
    this.uploadUrl = $apiUrl + $url + '/actions/upload?siteId=' + this.form.siteId + '&channelId=' + this.form.channelId + '&importType=' + this.form.importType;
  },

  btnSubmitClick: function () {
    if (this.form.fileNames.length === 0) {
      return utils.error('请选择需要导入的文件！');
    }

    this.apiSubmit();
  },

  btnCancelClick: function () {
    utils.closeLayer();
  },

  uploadBefore(file) {
    this.orderedFileNames.push(file.name);
    var re = /(\.zip|\.xlsx|\.txt)$/i;
    if (this.form.importType === 'zip') {
      re = /(\.zip)$/i;
    } else if (this.form.importType === 'xlsx') {
      re = /(\.xlsx)$/i;
    } else if (this.form.importType === 'image') {
      re = /(\.jpg|\.jpeg|\.bmp|\.gif|\.png|\.webp)$/i;
    } else if (this.form.importType === 'txt') {
      re = /(\.txt)$/i;
    }
    if(!re.exec(file.name)) {
      utils.error('请选择有效的文件上传!');
      return false;
    }
    return true;
  },

  uploadProgress: function() {
    utils.loading(this, true);
  },

  uploadRemove(file) {
    if (file.response) {
      var index = this.form.fileNames.indexOf(file.response.name);
      this.form.fileNames.splice(index, 1);
      this.form.fileUrls.splice(index, 1);

      var selectedIndex = this.orderedFileNames.indexOf(file.response.name);
      this.orderedFileNames.splice(selectedIndex, 1);
    }
  },

  uploadSuccess: function(res) {
    if (this.form.importType === 'excel') {
      this.form.fileNames = [];
      this.form.fileUrls = [];
      this.columns = res.columns;
      this.styles = res.styles;
      this.form.attributes = [];
      for (var i = 0; i < this.columns.length; i++) {
        this.form.attributes[i] = '';
      }
    }
    this.form.fileNames.push(res.name);
    this.form.fileUrls.push(res.url);
    utils.loading(this, false);
  },

  uploadError: function(err) {
    utils.loading(this, false);
    var error = JSON.parse(err.message);
    utils.error(error.message);
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    utils.keyPress(this.btnSubmitClick, this.btnCancelClick);
    this.apiGet();
  }
});
