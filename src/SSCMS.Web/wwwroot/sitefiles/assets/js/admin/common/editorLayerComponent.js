var $url = '/common/editor/layerComponent';

var data = utils.init({
  attributeName: utils.getQueryString('attributeName'),
  siteId: utils.getQueryInt("siteId"),

  isSiteOnly: false,
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
  },

  isSubmitForm: false,
  submitForm: {
    componentId: 0,
    parameters: []
  }
});

var methods = {
  insert: function(html) {
    parent.$vue.insertEditor(this.attributeName, html);
  },

  apiGet: function (page) {
    var $this = this;
    this.form.page = page;

    utils.loading(this, true);
    $api.get($url, {
      params: this.form
    }).then(function (response) {
      var res = response.data;

      $this.isSiteOnly = res.isSiteOnly;
      if ($this.isSiteOnly) {
        $this.form.groupId = -$this.siteId;
      }

      $this.groups = res.groups;
      $this.count = res.count;
      $this.items = res.items;
      $this.urlList = _.map($this.items, function (item) {
        return item.imageUrl;
      });
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  getGroupName: function() {
    var $this = this;
    if (this.form.groupId > 0) {
      var group = _.find(this.groups, function(o) { return o.id === $this.form.groupId; });
      return group.groupName;
    }
    return '';
  },

  btnSelectClick: function(component) {
    this.isSubmitForm = true;
    this.submitForm.componentId = component.id;
    this.submitForm.parameters = [];
    for (const item of (component.parameters || '').split(',')) {
      this.submitForm.parameters.push({
        key: item,
        value: ''
      });
    }
  },

  btnSelectGroupClick: function (groupId) {
    this.selectedGroupId = (this.selectedGroupId === groupId) ? 0 :groupId;
  },

  btnGroupClick: function(groupId) {
    var $this = this;

    this.form.groupId = groupId;
    this.form.page = 1;

    utils.loading(this, true);
    $api.get($url, {
      params: this.form
    }).then(function (response) {
      var res = response.data;

      $this.groups = res.groups;
      $this.count = res.count;
      $this.items = res.items;
      $this.urlList = _.map($this.items, function (item) {
        return item.imageUrl;
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

  apiSubmit: function() {
    var $this = this;

    var parameters = [];
    for (const item of this.submitForm.parameters) {
      if (item.key) {
        parameters.push({
          key: item.key,
          value: item.value
        });
      }
    }

    utils.loading(this, true);
    $api.post($url, {
      siteId: this.siteId,
      componentId: this.submitForm.componentId,
      parameters: parameters
    })
    .then(function(response) {
      var res = response.data;

      $this.insert(res.value);
      $this.isSubmitForm = false;
      utils.success('组件添加成功!');
      utils.closeLayer();
    })
    .catch(function(error) {
      utils.error(error);
    })
    .then(function() {
      utils.loading($this, false);
    });
  },

  btnSubmitClick: function() {
    var $this = this;

    this.$refs.submitForm.validate(function(valid) {
      if (valid) {
        $this.apiSubmit();
      } else {
        return false;
      }
    });
  },

  btnSearchClick() {
    utils.loading(this, true);
    this.apiGet(1);
  },

  btnPageClick: function(val) {
    utils.loading(this, true);
    this.apiGet(val);
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
    var $this = this;
    utils.keyPress(function () {
      if ($this.isSubmitForm) {
        $this.btnSubmitClick();
      }
    }, function () {
      if ($this.isSubmitForm) {
        $this.isSubmitForm = false;
      } else {
        $this.btnCancelClick();
      }
    });
    this.apiGet(1);
  }
});
