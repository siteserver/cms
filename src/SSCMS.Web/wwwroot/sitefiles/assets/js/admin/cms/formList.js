var $url = '/cms/forms/formList';
var $urlOrder = $url + '/actions/order';
var $urlExport = $url + '/actions/export';
var $urlImport = $url + '/actions/import';
var $urlDelete = $url + '/actions/delete';

var data = utils.init({
  siteId: utils.getQueryInt('siteId'),
  pageType: 'list',
  forms: null,
  authFormIds: null,
  form: null,
  urlUpload: null,
  files: []
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

      $this.forms = res.forms;
      $this.authFormIds = res.authFormIds;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiDelete: function (formId) {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlDelete, {
      siteId: this.siteId,
      formId: formId
    }).then(function (response) {
      var res = response.data;

      utils.alertSuccess({
        title: '表单删除成功',
        text: '表单删除成功，系统需要重载页面',
        callback: function() {
          window.top.location.reload(true);
        }
      });
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnViewClick: function (form) {
    utils.addTab(form.title, utils.getCmsUrl('formData', {
      siteId: this.siteId,
      formId: form.id
    }));
  },

  isDisabled: function (form) {
    return this.authFormIds.indexOf(form.id) === -1;
  },

  onSort: function (event) {
    var form = this.forms[event.oldIndex];
    var formId = form.id;
    var isUp = event.oldIndex > event.newIndex;
    var rows = Math.abs(event.oldIndex - event.newIndex);
    this.btnOrderClick(formId, isUp, rows);
  },

  btnOrderClick: function(formId, isUp, rows) {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlOrder, {
      siteId: this.siteId,
      formId: formId,
      isUp: isUp,
      rows: rows,
    }).then(function (response) {
      var res = response.data;

      $this.forms = res.forms;
      utils.success('表单排序成功！');
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnEditClick: function (form) {
    utils.openLayer({
      title: '修改表单',
      url: utils.getCmsUrl('formListLayerAdd', {
        siteId: this.siteId,
        formId: form.id
      }),
      width: 550,
      height: 450
    });
  },

  btnAddClick: function () {
    utils.openLayer({
      title: '新增表单',
      url: utils.getCmsUrl('formListLayerAdd', {
        siteId: this.siteId
      }),
      width: 550,
      height: 450
    });
  },

  btnDeleteClick: function (form) {
    var $this = this;

    utils.alertDelete({
      title: '删除表单',
      text: '此操作将删除表单' + form.title + '，确定吗？',
      callback: function () {
        $this.apiDelete(form.id);
      }
    });
  },

  btnExportClick: function (form) {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlExport, {
      siteId: this.siteId,
      formId: form.id
    }).then(function (response) {
      var res = response.data;

      window.open(res.value);
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  uploadBefore(file) {
    var re = /(\.zip)$/i;
    if(!re.exec(file.name))
    {
      utils.error('上传格式错误，请上传zip压缩包!');
      return false;
    }

    return true;
  },

  uploadProgress: function() {
    utils.loading(this, true);
  },

  uploadSuccess: function(res, file) {
    utils.loading(this, false);

    utils.alertSuccess({
      title: '表单导入成功',
      text: '表单导入成功，系统需要重载页面',
      callback: function() {
        window.top.location.reload(true);
      }
    });
  },

  uploadError: function(err) {
    utils.loading(this, false);
    var error = JSON.parse(err.message);
    utils.error(error.message);
  },

  btnCloseClick: function() {
    utils.removeTab();
  },
};

var $vue = new Vue({
  el: '#main',
  components: {
    ElTableDraggable,
  },
  data: data,
  methods: methods,
  created: function () {
    utils.keyPress(null, this.btnCloseClick);
    this.apiGet();
    this.urlUpload = $apiUrl + $urlImport + '?siteId=' + this.siteId;
  }
});
