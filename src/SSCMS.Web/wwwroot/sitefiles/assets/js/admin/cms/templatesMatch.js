var $url = '/cms/templates/templatesMatch';

var data = utils.init({
  siteId: utils.getQueryInt("siteId"),
  channels: null,
  channelTemplates: null,
  contentTemplates: null,

  expandedChannelIds: [],
  filterText: '',
  filterChannelTemplateId: utils.getQueryInt("channelTemplateId"),
  filterContentTemplateId: utils.getQueryInt("contentTemplateId"),

  defaultChannelTemplate: null,
  defaultContentTemplate: null,

  channelIds: [],
  channelTemplateId: 0,
  contentTemplateId: 0
});

var methods = {
  apiConfig: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.siteId
      }
    }).then(function (response) {
      var res = response.data;

      $this.channels = [res.channels];
      $this.channelTemplates = res.channelTemplates;
      $this.contentTemplates = res.contentTemplates;

      $this.expandedChannelIds = [$this.siteId];
      $this.defaultChannelTemplate = _.find($this.channelTemplates, function(o) { return o.defaultTemplate; }) || {id: 0};
      $this.defaultContentTemplate = _.find($this.contentTemplates, function(o) { return o.defaultTemplate; }) || {id: 0};
      $this.channelTemplateId = $this.defaultChannelTemplate.id;
      $this.contentTemplateId = $this.defaultContentTemplate.id;

      if ($this.filterChannelTemplateId > 0 || $this.filterContentTemplateId > 0) {
        setTimeout(function() {
          $this.filter($this.filterText, $this.filterChannelTemplateId, $this.filterContentTemplateId);
        }, 100);
      }
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function (data) {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, data).then(function (response) {
      var res = response.data;

      $this.channels = [res.value];

      $this.expandedChannelIds = $this.channelIds;
      $this.filterText = '';
      $this.filterChannelTemplateId = 0;
      $this.filterContentTemplateId = 0;
      utils.success('模板匹配成功！');
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiCreate: function (data) {
    var $this = this;

    utils.loading(this, true);
    $api.post($url + '/actions/create', data).then(function (response) {
      var res = response.data;

      $this.channels = [res.channels];
      $this.channelTemplates = res.channelTemplates;
      $this.contentTemplates = res.contentTemplates;

      $this.expandedChannelIds = $this.channelIds;
      $this.filterText = '';
      $this.filterChannelTemplateId = 0;
      $this.filterContentTemplateId = 0;
      utils.success('模板创建并匹配成功！');
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  getChannelUrl: function(data) {
    return utils.getRootUrl('redirect', {
      siteId: this.siteId,
      channelId: data.value
    });
  },

  getChannelTemplateName: function(channelTemplateId) {
    var template = _.find(this.channelTemplates, function(o) { return o.id === channelTemplateId; });
    return template ? template.templateName : this.defaultChannelTemplate.templateName;
  },

  getContentTemplateName: function(contentTemplateId) {
    var template = _.find(this.contentTemplates, function(o) { return o.id === contentTemplateId; });
    return template ? template.templateName : this.defaultContentTemplate.templateName;
  },

  filterNode: function(value, data) {
    if (!value) return true;
    if (value.channelName && value.channelTemplateId && value.contentTemplateId) {
      return data.label.indexOf(value.channelName) !== -1 && data.channelTemplateId === value.channelTemplateId && data.contentTemplateId === value.contentTemplateId;
    } else if (value.channelName && value.channelTemplateId) {
      return data.label.indexOf(value.channelName) !== -1 && data.channelTemplateId === value.channelTemplateId;
    } else if (value.channelName && value.contentTemplateId) {
      return data.label.indexOf(value.channelName) !== -1 && data.contentTemplateId === value.contentTemplateId;
    } else if (value.channelTemplateId && value.contentTemplateId) {
      return data.channelTemplateId === value.channelTemplateId && data.contentTemplateId === value.contentTemplateId;
    } else if (value.channelName) {
      return data.label.indexOf(value.channelName) !== -1;
    } else if (value.contentTemplateId) {
      return data.contentTemplateId === value.contentTemplateId;
    } else if (value.channelTemplateId) {
      return data.channelTemplateId === value.channelTemplateId;
    }
    return true;
  },

  handleCheckChange() {
    this.channelIds = this.$refs.tree.getCheckedKeys();
  },

  btnCreateClick: function (command) {
    var message = '';
    var data = null;
    if (command === 'CreateChannelTemplate') {
      message = '此操作将创建空的栏目模板并匹配选中栏目, 是否继续?';
      data = {
        siteId: this.siteId,
        channelIds: this.channelIds,
        isChannelTemplate: true,
        isChildren: false,
      };
    }
    else if (command === 'CreateSubChannelTemplate') {
      message = '此操作将创建空的栏目模板并匹配选中栏目的下级栏目, 是否继续?';
      data = {
        siteId: this.siteId,
        channelIds: this.channelIds,
        isChannelTemplate: true,
        isChildren: true,
      };
    }
    else if (command === 'CreateContentTemplate') {
      message = '此操作将创建空的内容模板并匹配选中栏目, 是否继续?';
      data = {
        siteId: this.siteId,
        channelIds: this.channelIds,
        isChannelTemplate: false,
        isChildren: false,
      };
    }
    else if (command === 'CreateSubContentTemplate') {
      message = '此操作将创建空的内容模板并匹配选中栏目的下级栏目, 是否继续?';
      data = {
        siteId: this.siteId,
        channelIds: this.channelIds,
        isChannelTemplate: false,
        isChildren: true,
      };
    }

    var $this = this;
    this.$confirm(message, '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    }).then(function() {
      $this.apiCreate(data);
    });
  },

  btnChannelMatchClick: function() {
    this.apiSubmit({
      siteId: this.siteId,
      channelIds: this.channelIds,
      isChannelTemplate: true,
      templateId: this.channelTemplateId
    });
  },

  btnContentMatchClick: function() {
    this.apiSubmit({
      siteId: this.siteId,
      channelIds: this.channelIds,
      isChannelTemplate: false,
      templateId: this.contentTemplateId
    });
  },

  filter: function(filterText, channelTemplateId, contentTemplateId) {
    this.$refs.tree.filter({
      channelName: filterText,
      channelTemplateId: channelTemplateId,
      contentTemplateId: contentTemplateId
    });
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  watch: {
    filterText: function(val) {
      this.filter(val, this.filterChannelTemplateId, this.filterContentTemplateId);
    },
    filterChannelTemplateId: function(val) {
      this.filter(this.filterText, val, this.filterContentTemplateId);
    },
    filterContentTemplateId: function(val) {
      this.filter(this.filterText, this.filterChannelTemplateId, val);
    }
  },
  created: function () {
    this.apiConfig();
  }
});