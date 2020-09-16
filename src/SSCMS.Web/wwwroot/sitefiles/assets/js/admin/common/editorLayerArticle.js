var $url = '/common/editor/layerArticle';

var data = utils.init({
  attributeName: utils.getQueryString('attributeName'),
  siteId: utils.getQueryInt("siteId"),
  pageType: 'card',

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
    textId: 0,
    linkText: ''
  }
});

var methods = {
  insert: function(result) {
    var vueHtml = '' + 
    '<el-popover' + 
    '  width="600"' + 
    '  trigger="click">' + 
    '   ' + result.content +
    '  <el-button size="small" type="primary" slot="reference">' + result.linkText + '</el-button>' + 
    '</el-popover>'
    var html = '<a href="javascript:;" data-vue="' + encodeURIComponent(vueHtml) + '">' + result.linkText + '</a>';
    parent.$vue.insertEditor(this.attributeName, html);
  },

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

  btnSelectClick: function(article) {
    this.isSubmitForm = true;
    this.submitForm.textId = article.id;
  },

  btnSelectGroupClick: function (groupId) {
    this.selectedGroupId = (this.selectedGroupId === groupId) ? 0 :groupId;
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

  btnDropdownClick: function(command) {
    this.pageType = command;
  },

  apiSubmit: function() {
    var $this = this;

    utils.loading(this, true);
    $api.get($url + '/' + this.submitForm.textId, {
      params: {
        siteId: this.siteId
      }
    })
    .then(function(response) {
      var res = response.data;

      $this.insert({
        linkText: $this.submitForm.linkText,
        content: res.content
      });
      $this.isSubmitForm = false;
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
    this.apiList(1);
  },

  btnPageClick: function(val) {
    utils.loading(this, true);
    this.apiList(val);
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