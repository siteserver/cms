var $url = '/admin/cms/library/libraryImage';

var data = utils.initData({
  siteId: utils.getQueryInt("siteId"),
  pageType: 'card',
  drawer: false,
  isGroupForm: false,
  groupForm: {
    siteId: utils.getQueryInt('siteId'),
    name: '',
  },

  groups: null,
  count: null,
  items: null,
  urlList: null,
  renameId: 0,
  renameTitle: '',
  deleteId: 0,
  selectedGroupId: 0,
  
  form: {
    siteId: utils.getQueryInt("siteId"),
    keyword: '',
    groupId: 0,
    page: 1,
    perPage: 24
  }
});

var methods = {
  apiList: function (page) {
    var $this = this;
    this.form.page = page;

    utils.loading(this, true);
    $api.post($url + '/list', this.form).then(function (response) {
      var res = response.data;

      $this.groups = res.groups;
      $this.count = res.count;
      $this.items = res.items;
      $this.urlList = _.map($this.items, function (item) {
        return item.url;
      });
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiCreateGroup: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url + '/groups', this.groupForm).then(function (response) {
      var res = response.data;

      $this.groups.push(res);
    }).catch(function (error) {
      $this.$notify.error({
          title: '错误',
          message: error.message
        });
    }).then(function () {
      $this.isGroupForm = false;
      utils.loading($this, false);
    });
  },

  apiRenameGroup: function () {
    var $this = this;

    utils.loading(this, true);
    $api.put($url + '/groups/' + this.form.groupId, this.groupForm).then(function (response) {
      var res = response.data;

      var group = _.find($this.groups, function(o) { return o.id === $this.form.groupId; });
      group.groupName = res.groupName;
    }).catch(function (error) {
      $this.$notify.error({
          title: '错误',
          message: error.message
        });
    }).then(function () {
      $this.isGroupForm = false;
      utils.loading($this, false);
    });
  },

  apiDeleteGroup: function () {
    var $this = this;

    utils.loading(this, true);
    $api.delete($url + '/groups/' + this.form.groupId).then(function (response) {
      var res = response.data;

      _.remove($this.groups, function(n) {
        return n.id === $this.form.groupId;
      });
    }).catch(function (error) {
      $this.$notify.error({
          title: '错误',
          message: error.message
        });
    }).then(function () {
      $this.isGroupForm = false;
      $this.form.groupId = 0;
      utils.loading($this, false);
    });
  },

  apiDelete: function (library) {
    var $this = this;

    utils.loading(this, true);
    $api.delete($url + '/' + library.id).then(function (response) {
      var res = response.data;

      $this.items.splice($this.items.indexOf(library), 1);
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  getLinkUrl: function(libraryType) {
    return 'library' + libraryType + '.cshtml?siteId=' + this.siteId;
  },

  getGroupName: function() {
    var $this = this;
    if (this.form.groupId > 0) {
      var group = _.find(this.groups, function(o) { return o.id === $this.form.groupId; });
      return group.groupName;
    }
    return '';
  },

  getUploadUrl: function() {
    return $apiUrl + $url + '?siteId=' + this.siteId + '&groupId=' + this.form.groupId
  },

  btnTitleClick: function(library) {
    var $this = this;
    this.renameId = library.id;
    this.renameTitle = library.title;
    setTimeout(function() {
      var el = $this.$refs['renameInput' + library.id][0];
      if (el) {
        el.focus();
        el.select();
      }
    }, 100);
  },

  btnSelectGroupClick: function (groupId) {
    this.selectedGroupId = (this.selectedGroupId === groupId) ? 0 :groupId;
  },

  btnSelectGroupSubmit: function(library) {
    library.groupId = this.selectedGroupId;
    library.isSelectGroups = false;

    var $this = this;
    $api.put($url + '/' + library.id, library).then(function (response) {
      var res = response.data;

      $this.$message.success('转移分组成功');

      if ($this.selectedGroupId !== $this.form.groupId && $this.form.groupId !== 0) {
        $this.btnSearchClick();
      }
    }).catch(function (error) {
      utils.error($this, error);
    });
  },

  rename: function(library) {
    if (this.renameId === 0) return;
    if (!this.renameTitle) {
      this.$message.error('名称不能为空');
      return false;
    }
    
    this.renameId = 0;
    if (library.title === this.renameTitle) return false;
    library.title = this.renameTitle;

    var $this = this;
    $api.put($url + '/' + library.id, library).then(function (response) {
      var res = response.data;

      $this.$message.success('编辑名称成功');
    }).catch(function (error) {
      utils.error($this, error);
    });

    return false;
  },

  btnGroupClick: function(groupId) {
    var $this = this;

    this.form.groupId = groupId;
    this.form.page = 1;

    utils.loading(this, true);
    $api.post($url + '/list', this.form).then(function (response) {
      var res = response.data;

      $this.groups = res.groups;
      $this.count = res.count;
      $this.items = res.items;
      $this.urlList = _.map($this.items, function (item) {
        return item.url;
      });
    }).catch(function (error) {
      $this.$notify.error({
          title: '错误',
          message: error.message
        });
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnDropdownClick: function(command) {
    this.pageType = command;
  },

  btnCreateGroupClick: function() {
    this.groupForm.name = '';
    this.isGroupForm = true;
  },

  btnRenameGroupClick: function() {
    var $this = this;
    var group = _.find(this.groups, function(o) { return o.id === $this.form.groupId; });
    this.groupForm.name = group.groupName;
    this.isGroupForm = true;
  },

  btnDeleteGroupClick: function () {
    var $this = this;

    utils.alertDelete({
      title: '删除分组',
      text: '仅删除分组，不删除图片，组内图片将自动归入未分组',
      callback: function () {
        $this.apiDeleteGroup();
      }
    });
  },

  btnDeleteClick: function (library) {
    var $this = this;

    utils.alertDelete({
      title: '删除素材',
      text: '确定删除此素材吗？',
      callback: function () {
        $this.apiDelete(library);
      }
    });
  },

  btnGroupSubmitClick: function() {
    var $this = this;
    this.$refs.groupForm.validate(function(valid) {
      if (valid) {
        if ($this.form.groupId > 0) {
          $this.apiRenameGroup();
        } else {
          $this.apiCreateGroup();
        }
      } else {
        return false;
      }
    });
  },

  btnSearchClick() {
    utils.loading(this, true);
    this.apiList(1);
  },

  btnPageClick: function(val) {
    utils.loading(this, true);
    this.apiList(val);
  },

  uploadBefore(file) {
    var re = /(\.jpg|\.jpeg|\.bmp|\.gif|\.svg|\.png|\.webp|\.jfif)$/i;
    if(!re.exec(file.name))
    {
      this.$message.error('文件只能是图片格式，请选择有效的文件上传!');
      return false;
    }

    var isLt10M = file.size / 1024 / 1024 < 10;
    if (!isLt10M) {
      this.$message.error('上传图片大小不能超过 10MB!');
      return false;
    }
    return true;
  },

  uploadProgress: function() {
    utils.loading(this, true);
  },

  uploadSuccess: function(res) {
    this.items.splice(0, 0, res);
    this.count++;
    utils.loading(this, false);
  },

  uploadError: function(err) {
    utils.loading(this, false);
    var error = JSON.parse(err.message);
    this.$message.error(error.message);
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiList(1);
  }
});