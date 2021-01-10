var $url = '/cms/templates/templatesSpecial';
var $urlUpload = $apiUrl + $url + '/actions/upload';
var $urlDownload = $url + '/actions/download';

var data = utils.init({
  siteId: utils.getQueryInt('siteId'),
  specials: null,
  siteUrl: null,
  uploadUrl: null,

  panel: false,
  form: null,
  uploadList: []
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.siteId
      }
    }).then(function (response) {
      var res = response.data;

      $this.specials = res.specials;
      $this.siteUrl = res.siteUrl;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiGetSpecial: function (specialId) {
    var $this = this;

    utils.loading(this, true);
    $api.get($url + '/' + this.siteId + '/' + specialId).then(function (response) {
      var res = response.data;

      $this.panel = true;
      $this.uploadUrl = $urlUpload + '?siteId=' + $this.siteId + '&guid=' + res.guid;
      if (specialId === 0) {
        $this.form = {
          siteId: $this.siteId,
          id: 0,
          guid: res.guid,
          title: '',
          url: '/special/' + $this.formatDate() + '/',
          fileNames: [],
          isEditOnly: false,
          isUploadOnly: false
        };
      } else {
        $this.form = {
          siteId: $this.siteId,
          id: specialId,
          guid: res.guid,
          title: res.special.title,
          url: res.special.url,
          fileNames: [],
          isEditOnly: false,
          isUploadOnly: true
        };
      }
      $this.uploadList = [];
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiDelete: function (specialId) {
    var $this = this;

    utils.loading(this, true);
    $api.delete($url, {
      data: {
        siteId: this.siteId,
        specialId: specialId
      }
    }).then(function (response) {
      var res = response.data;

      $this.specials = res.specials;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, this.form).then(function (response) {
      var res = response.data;

      utils.success($this.form.id === 0 ? '专题添加成功！' : '专题修改成功！');
      $this.form = null;
      $this.specials = res.specials;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnEditClick: function (special) {
    this.form = {
      siteId: this.siteId,
      id: special.id,
      guid: '',
      title: special.title,
      url: special.url,
      fileNames: [],
      isEditOnly: true,
      isUploadOnly: false
    };
    this.panel = true;
  },

  btnUploadClick: function (special) {
    this.apiGetSpecial(special.id);
  },

  btnDownloadClick: function(item){
    var $this = this;

    utils.loading(this, true);
    $api.post($urlDownload, {
      siteId: this.siteId,
      specialId: item.id
    }).then(function (response) {
      var res = response.data;

      window.location.href = res.value;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnAddClick: function () {
    this.apiGetSpecial(0);
  },

  btnDeleteClick: function (item) {
    var $this = this;

    utils.alertDelete({
      title: '删除专题',
      text: '此操作将删除专题 ' + item.title + '，确定吗？',
      callback: function () {
        $this.apiDelete(item.id);
      }
    });
  },

  btnSubmitClick: function () {
    if (!this.form.isEditOnly && this.form.fileNames.length === 0) {
      return utils.error('请上传专题文件');
    }
    var $this = this;
    this.$refs.form.validate(function(valid) {
      if (valid) {
        $this.apiSubmit();
      }
    });
  },

  formatDate: function () {
    var d = new Date(),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();
  
    if (month.length < 2) 
        month = '0' + month;
    if (day.length < 2) 
        day = '0' + day;
  
    return [year, month, day].join('-');
  },

  btnCancelClick: function () {
    this.panel = false;
  },

  uploadBefore(file) {
    // var isExcel = file.name.indexOf('.xlsx', file.name.length - '.xlsx'.length) !== -1;
    // if (!isExcel) {
    //   utils.error('用户导入文件只能是 Excel 格式!');
    // }
    // return isExcel;
    return true;
  },

  uploadProgress: function() {
    utils.loading(this, true);
  },

  uploadSuccess: function(res, file) {
    utils.loading(this, false);
    this.form.fileNames.push(res.value);
  },

  uploadError: function(err) {
    utils.loading(this, false);
    var error = JSON.parse(err.message);
    utils.error(error.message);
  },

  uploadRemove: function(file, fileList) {
    this.form.fileNames.splice(this.form.fileNames.indexOf(file.name), 1);
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiGet();
  }
});