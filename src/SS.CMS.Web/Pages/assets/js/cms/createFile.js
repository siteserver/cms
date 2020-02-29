var $url = '/admin/cms/create/createFile';

var data = utils.initData({
  siteId: utils.getQueryInt('siteId'),
  isAllChecked: false,
  allTemplates: null,
  templates: null,
  checkedTemplateIds: [],
  filterText: ''
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

      $this.allTemplates = $this.templates = res.templates;
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    })
  },

  apiCreate: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, {
      siteId: this.siteId,
      templateIds: this.checkedTemplateIds
    }).then(function (response) {
      location.href = utils.getCmsUrl('createStatus', {siteId: $this.siteId});
    }).catch(function (error) {
      utils.error($this, error);
    });
  },

  filterNode: function(value, data) {
    if (!value) return true;
    if (value.text) {
      return data.label.indexOf(value.text) !== -1;
    }
    return true;
  },

  selectAll: function (val) {
    if (val) {
      this.checkedTemplateIds = _.map(this.templates, function(t) {
        return t.id;
      });
    } else {
      this.checkedTemplateIds = [];
    }
  },

  btnCreateClick: function() {
    this.apiCreate();
  },

  getLabel: function(template) {
    return template.templateName + '(' + template.createdFileFullName.replace('@', '') + ')';
  },

  handleCheckedTemplatesChange: function(value) {
    var checkedCount = value.length;
    this.isAllChecked = checkedCount === this.templates.length;
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  watch: {
    filterText: function(val) {
      if (val) {
        this.templates = _.filter(this.allTemplates, function(o) { return o.createdFileFullName.indexOf(val) !== -1; });
      } else {
        this.templates = this.allTemplates
      }
    }
  },
  created: function () {
    this.apiGet();
  }
});