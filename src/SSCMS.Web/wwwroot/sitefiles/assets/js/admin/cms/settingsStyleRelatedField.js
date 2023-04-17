var $url = '/cms/settings/settingsStyleRelatedField';
var $urlUpdate = $url + '/actions/update';
var $urlDelete = $url + '/actions/delete';
var $urlImport = $url + '/actions/import';
var $urlItems = $url + '/items';
var $urlItemsUpdate = $urlItems + '/actions/update';
var $urlItemsDelete = $urlItems + '/actions/delete';
var $urlItemsOrder = $urlItems + '/actions/order';

var data = utils.init({
  siteId: utils.getQueryInt('siteId'),
  urlUpload: null,
  relatedFields: null,

  editorPanel: false,
  editorForm: null,

  itemsPanel: false,
  itemsRelatedField: null,
  itemsParentId: 0,
  itemsParentName: null,
  itemsTree: null,
  itemsExpandedIds: [],
  itemsAddDialog: false,
  itemsAddForm: {
    isRapid: true,
    rapidValues: '',
    items: [{
      key: '',
      value: ''
    }]
  },
  itemsEditDialog: false,
  itemsEditForm: null,

  uploadPanel: false,
  uploadLoading: false,
  uploadList: []
});

var methods = {
  apiList: function (message) {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.siteId
      }
    }).then(function (response) {
      var res = response.data;

      $this.relatedFields = res;
      $this.urlUpload = $apiUrl + $urlImport + '?siteId=' + $this.siteId;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
      if (message) {
        utils.success(message);
      }
    });
  },

  apiGetItems: function (relatedFieldId) {
    var $this = this;

    utils.loading(this, true);
    $api.get($urlItems, {
      params: {
        siteId: this.siteId,
        relatedFieldId: relatedFieldId
      }
    }).then(function (response) {
      var res = response.data;

      $this.itemsTree = res.tree;
      $this.itemsPanel = true;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiDelete: function (relatedFieldId) {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlDelete, {
      siteId: this.siteId,
      relatedFieldId: relatedFieldId
    }).then(function (response) {
      var res = response.data;

      $this.relatedFields = res;
      utils.success('联动字段删除成功！');
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiAdd: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, this.editorForm).then(function (response) {
      var res = response.data;

      $this.relatedFields = res;
      $this.editorPanel = false;
      utils.success('联动字段新增成功！');
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiEdit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlUpdate, this.editorForm).then(function (response) {
      var res = response.data;

      $this.relatedFields = res;
      $this.editorPanel = false;
      utils.success('联动字段编辑成功！');
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnDeleteClick: function (id, title) {
    var $this = this;

    utils.alertDelete({
      title: '删除联动字段',
      text: '此操作将删除联动字段 ' + title + '，确定吗？',
      callback: function () {
        $this.apiDelete(id);
      }
    });
  },

  btnCommandClick: function(command){
    if (command === 'Add') {
      this.btnAddClick();
    } else if (command === 'Import') {
      this.btnImportClick();
    } else if (command === 'Export') {
      this.btnExportClick();
    }
  },

  btnCancelClick: function() {
    this.editorPanel = false;
    this.itemsPanel = false;
  },

  btnAddClick: function () {
    this.editorForm = {
      id: 0,
      siteId: this.siteId,
      title: null
    };
    this.editorPanel = true;
  },

  btnEditClick: function (relatedField) {
    this.editorForm = _.assign({}, relatedField);
    this.editorPanel = true;
  },

  btnEditorSubmitClick: function() {
    var $this = this;
    this.$refs.editorForm.validate(function(valid) {
      if (valid) {
        if ($this.editorForm.id) {
          $this.apiEdit();
        } else {
          $this.apiAdd();
        }
      }
    });
  },

  btnItemsClick: function (relatedField) {
    this.itemsRelatedField = _.assign({}, relatedField);
    this.apiGetItems(relatedField.id);
  },

  btnImportClick: function() {
    this.uploadPanel = true;
  },

  uploadBefore(file) {
    var isZip = file.name.indexOf('.zip', file.name.length - '.zip'.length) !== -1;
    if (!isZip) {
      utils.error('样式导入文件只能是 Zip 格式!');
    }
    return isZip;
  },

  uploadProgress: function() {
    utils.loading(this, true);
  },

  uploadSuccess: function(res, file) {
    this.uploadList = [];
    this.uploadPanel = false;
    this.apiList('字段导入成功！');
  },

  uploadError: function(err) {
    this.uploadList = [];
    utils.loading(this, false);
    var error = JSON.parse(err.message);
    utils.error(error.message);
  },

  btnExportClick: function() {
    var $this = this;

    utils.loading(this, true);
    $api.post($url + '/actions/export', {
      siteId: this.siteId
    }).then(function (response) {
      var res = response.data;

      window.open(res.value);
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnItemsAddClick: function(parentName, parentId) {
    this.itemsAddDialog = true;
    this.itemsParentName = parentName;
    this.itemsParentId = parentId;
    this.itemsAddForm.isRapid = true;
    this.itemsAddForm.rapidValues = '';
    this.itemsAddForm.items = [{
      key: '',
      value: ''
    }];
  },

  btnItemRemoveClick: function (index) {
    this.itemsAddForm.items.splice(index, 1);
    if (this.itemsAddForm.items.length === 0) {
      this.btnItemsAddClick();
    }
  },

  btnItemAddClick: function () {
    this.itemsAddForm.items.push({
      label: '',
      value: ''
    });
  },

  apiItemsDelete: function (id) {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlItemsDelete, {
      siteId: this.siteId,
      relatedFieldId: this.itemsRelatedField.id,
      id: id
    }).then(function (response) {
      var res = response.data;

      $this.itemsTree = res.tree;
      utils.success('联动字段项删除成功！');
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiItemsAdd: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlItems, {
      siteId: this.siteId,
      relatedFieldId: this.itemsRelatedField.id,
      parentId: this.itemsParentId,
      isRapid: this.itemsAddForm.isRapid,
      rapidValues: this.itemsAddForm.rapidValues,
      items: this.itemsAddForm.items
    }).then(function (response) {
      var res = response.data;

      $this.itemsTree = res.tree;
      utils.success('联动字段项添加成功！');
      $this.itemsAddDialog = false;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiItemsOrder: function (id, up) {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlItemsOrder, {
      siteId: this.siteId,
      relatedFieldId: this.itemsRelatedField.id,
      id: id,
      up: up
    }).then(function (response) {
      var res = response.data;

      $this.itemsTree = res.tree;
      utils.success('联动字段项排序成功！');
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiItemsEdit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlItemsUpdate, {
      siteId: this.siteId,
      relatedFieldId: this.itemsRelatedField.id,
      id: this.itemsEditForm.id,
      label: this.itemsEditForm.label,
      value: this.itemsEditForm.value
    }).then(function (response) {
      var res = response.data;

      $this.itemsTree = res.tree;
      utils.success('联动字段项编辑成功！');
      $this.itemsEditDialog = false;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnItemsAddSubmitClick: function () {
    var $this = this;

    this.$refs.itemsAddForm1.validate(function(valid1) {
      if (valid1) {
        $this.$refs.itemsAddForm2.validate(function(valid2) {
          if (valid2) {
            $this.apiItemsAdd();
          }
        });
      }
    });
  },

  btnItemsAddCancelClick: function () {
    this.itemsAddDialog = false;
  },

  btnItemsDeleteClick: function (label, id) {
    var $this = this;

    utils.alertDelete({
      title: '删除联动字段',
      text: '此操作将删除联动字段项 ' + label + '及其子项，确定吗？',
      callback: function () {
        $this.apiItemsDelete(id);
      }
    });
  },

  btnItemsUpClick: function(item) {
    this.apiItemsOrder(item.value, true);
  },

  btnItemsDownClick: function(item) {
    this.apiItemsOrder(item.value, false);
  },

  btnItemsEditClick: function(item) {
    this.itemsEditDialog = true;
    this.itemsEditForm = {
      id: item.value,
      label: item.label,
      value: item.itemValue
    };
  },

  btnItemsEditSubmitClick: function () {
    var $this = this;

    this.$refs.itemsEditForm.validate(function(valid) {
      if (valid) {
        $this.apiItemsEdit();
      }
    });
  },

  btnItemsEditCancelClick: function () {
    this.itemsEditDialog = false;
  },

  btnCloseClick: function() {
    utils.removeTab();
  },
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    var $this = this;
    utils.keyPress(function () {
      if ($this.editorPanel) {
        $this.btnEditorSubmitClick();
      }
    }, function() {
      if ($this.editorPanel || $this.itemsPanel || $this.uploadPanel) {
        $this.btnCancelClick();
      } else {
        $this.btnCloseClick();
      }
    });
    this.apiList();
  }
});
