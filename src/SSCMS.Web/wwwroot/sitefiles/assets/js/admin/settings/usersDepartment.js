var $url = '/settings/usersDepartment';
var $urlUpdate = $url + '/actions/update';
var $urlDelete = $url + '/actions/delete';

var data = utils.init({
  root: null,
  expandedNodeIds: [],
  settings: null,

  departmentIds: [],
  filterText: '',
  appendPanel: false,
  appendForm: null,
  editPanel: false,
  form: null,
  deletePanel: false,
  deleteForm: null,
});

var methods = {
  apiList: function(expandedNodeIds) {
    var $this = this;

    utils.loading(this, true);
    $api.get($url).then(function (response) {
      var res = response.data;

      $this.root = res.departments;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiGet: function(departmentId) {
    var $this = this;

    utils.loading(this, true);
    $api.get($url + '/' + departmentId).then(function (response) {
      var res = response.data;

      $this.form = _.assign({}, res.department);
      $this.editPanel = true;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiAppend: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url + '/actions/append', {
      parentId: this.appendForm.parentIds.length > 0 ? this.appendForm.parentIds[this.appendForm.parentIds.length - 1] : 0,
      departments: this.appendForm.departments,
    }).then(function (response) {
      var res = response.data;

      $this.appendPanel = false;
      $this.apiList(res);
      utils.success('部门添加成功!');
    }).catch(function (error) {
      utils.loading($this, false);
      utils.error(error);
    });
  },

  apiUpdate: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlUpdate, this.form).then(function (response) {
      var res = response.data;

      $this.editPanel = false;
      $this.apiList(res);
      utils.success('部门编辑成功!');
    }).catch(function (error) {
      utils.loading($this, false);
      utils.error(error);
    });
  },

  apiDelete: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlDelete, this.deleteForm).then(function (response) {
      var res = response.data;

      $this.deletePanel = false;
      $this.apiList(res);
      utils.success('部门删除成功!');
    }).catch(function (error) {
      utils.loading($this, false);
      utils.error(error);
    });
  },

  apiDrop: function (sourceId, targetId, dropType) {
    var $this = this;

    utils.loading(this, true);
    $api.post($url + '/actions/drop', {
      sourceId: sourceId,
      targetId: targetId,
      dropType: dropType
    }).then(function (response) {
      var res = response.data;

      utils.success('部门排序成功!');
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  handleDrop: function(draggingNode, dropNode, dropType, ev) {
    this.apiDrop(draggingNode.data.value, dropNode.data.value, dropType);
  },

  allowDrop: function(draggingNode, dropNode, type) {
    if (dropNode.data.value === 0) {
      return false;
    } else {
      return true;
    }
  },

  allowDrag: function(draggingNode) {
    return draggingNode.data.value !== 0;
  },

  handleCheckChange() {
    this.departmentIds = this.$refs.tree.getCheckedKeys();
  },

  btnCheckClick: function(row) {
    if (this.departmentIds.indexOf(row.value) !== -1) {
      this.departmentIds.splice(this.departmentIds.indexOf(row.value), 1);
    } else {
      this.departmentIds.push(row.value);
    }
    this.$refs.tree.setCheckedKeys(this.departmentIds);
  },

  filterNode: function(value, data) {
    if (!value) return true;
    if (value.name) {
      return (data.label.indexOf(value.name) !== -1 || data.value + '' === value.name);
    }
    return true;
  },

  btnCancelClick: function() {
    this.appendPanel = false;
    this.deletePanel = false;
    this.editPanel = false;
  },

  btnAppendClick: function() {
    this.appendForm = {
      parentIds: [],
      departments: ''
    };
    this.appendPanel = true;
  },

  btnAppendSubmitClick: function() {
    var $this = this;
    this.$refs.appendForm.validate(function(valid) {
      if (valid) {
        $this.apiAppend();
      }
    });
  },

  btnEditClick: function(row) {
    this.apiGet(row.value);
  },

  btnSaveClick: function() {
    var $this = this;
    this.$refs.editForm.validate(function(valid) {
      if (valid) {
        $this.apiUpdate();
      }
    });
  },

  btnDeleteClick: function(data) {
    this.deleteForm = {
      departmentId: data.value,
      label: data.label,
      name: null,
    };
    this.deletePanel = true;
  },

  btnDeleteSubmitClick: function() {
    var $this = this;
    this.$refs.deleteForm.validate(function(valid) {
      if (valid) {
        if ($this.deleteForm.name == $this.deleteForm.label) {
          $this.apiDelete();
        } else {
          utils.error('请检查您输入的部门名称是否正确');
        }
      }
    });
  },

  btnCloseClick: function() {
    utils.removeTab();
  },
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  watch: {
    filterText: function(val) {
      this.$refs.tree.filter({
        name: val,
      });
    }
  },
  created: function () {
    var $this = this;
    utils.keyPress(function () {
      if ($this.editPanel) {
        $this.btnSaveClick();
      } else if ($this.appendPanel) {
        $this.btnAppendSubmitClick();
      } else if ($this.deletePanel) {
        $this.btnDeleteSubmitClick();
      }
    }, function () {
      if ($this.editPanel || $this.appendPanel || $this.deletePanel) {
        $this.btnCancelClick();
      } else {
        $this.btnCloseClick();
      }
    });
    this.apiList();
  }
});
