var $api = new apiUtils.Api(apiUrl + '/pages/cms/create');
var $apiAll = new apiUtils.Api(apiUrl + '/pages/cms/create/all');
var $siteId = parseInt(pageUtils.getQueryStringByName('siteId'));
var $type = pageUtils.getQueryStringByName('type');

var data = {
  pageLoad: false,
  pageAlert: null,
  pageType: null,
  siteId: $siteId,
  type: $type,
  root: null,
  pageChannels: [],
  isAllChecked: false,
  isDescendent: false,
  isChannelPage: $type === 'channels',
  isContentPage: $type === 'contents',
  scope: 'all'
};

var methods = {
  displayChildren: function (channel, event) {
    channel.isOpen ? this.hideChildren(channel) : this.showChildren(channel);
    event.stopPropagation();
  },
  toggleChecked: function (channel) {
    channel.isChecked = !channel.isChecked;
    if (!channel.isChecked) {
      this.isAllChecked = false;
    }
  },
  selectAll: function () {
    this.isAllChecked = !this.isAllChecked;
    for (var i = 0; i < this.pageChannels.length; i++) {
      this.pageChannels[i].isChecked = this.isAllChecked;
    }
    this.reload();
  },
  create: function () {
    var $this = this;

    $this.pageLoad = false;

    var channelIdList = [];
    for (var i = 0; i < this.pageChannels.length; i++) {
      if (this.pageChannels[i].isChecked) {
        channelIdList.push(this.pageChannels[i].id);
      }
    }

    if (!$this.isAllChecked && channelIdList.length === 0) return;
    if (!$this.isChannelPage && !this.isContentPage) return;

    $api.post({
      siteId: $this.siteId,
      channelIdList: $this.isAllChecked ? [] : channelIdList,
      isAllChecked: $this.isAllChecked,
      isDescendent: $this.isDescendent,
      isChannelPage: $this.isChannelPage,
      isContentPage: $this.isContentPage,
      scope: $this.scope
    }, function () {
      location.href = 'createStatus.cshtml?siteId=' + $this.siteId;
    });
  },
  createIndex: function () {
    var $this = this;

    $api.post({
      siteId: $this.siteId,
      channelIdList: [$this.siteId],
      isAllChecked: false,
      isDescendent: false,
      isChannelPage: true,
      isContentPage: false,
    }, function () {
      location.href = 'createStatus.cshtml?siteId=' + $this.siteId;
    });
  },
  createAll: function () {
    var $this = this;

    $apiAll.post({
      siteId: $this.siteId
    }, function () {
      location.href = 'createStatus.cshtml?siteId=' + $this.siteId;
    });
  },
  loadChannels: function () {
    var $this = this;

    $api.get({
      siteId: $siteId,
      parentId: $siteId
    }, function (err, res) {
      if (err || !res || !res.value) return;

      $this.root = _.assign({}, res.parent, {
        contentNum: res.countDict[res.parent.id],
        isOpen: true,
        isLoading: false,
        isChecked: false,
        children: []
      });

      for (var i = 0; i < res.value.length; i++) {
        var channel = _.assign({}, res.value[i], {
          contentNum: res.countDict[res.value[i].id],
          isOpen: false,
          isLoading: false,
          isChecked: false,
          children: []
        });
        $this.root.children.push(channel);
      }

      $this.reload();

      $this.pageLoad = true;
    });
  },
  reload: function () {
    this.pageChannels = [];
    this.addPageChannels(this.root);
  },
  addPageChannels: function (channel) {
    this.pageChannels.push(channel);
    if (channel.isOpen) {
      for (var i = 0; i < channel.children.length; i++) {
        var child = channel.children[i];
        this.addPageChannels(child);
      }
    }
  },
  showChildren: function (channel) {
    var $this = this;

    channel.isOpen = true;
    if (channel.childrenCount > 0 && channel.children.length === 0) {
      channel.children = [];
      channel.isLoading = true;
      $api.get({
        siteId: $siteId,
        parentId: channel.id
      }, function (err, res) {
        if (err || !res || !res.value) return;

        for (var i = 0; i < res.value.length; i++) {
          var child = _.assign({}, res.value[i], {
            contentNum: res.countDict[res.value[i].id],
            isOpen: false,
            isLoading: false,
            isChecked: $this.isAllChecked,
            children: []
          });

          channel.children.push(child);
        }

        channel.isLoading = false;

        $this.reload();
      });
    } else {
      this.reload();
    }
  },
  hideChildren: function (channel) {
    channel.isOpen = false;
    this.reload();
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    if (this.type === 'index') {
      this.createIndex();
    } else if (this.type === 'all') {
      this.createAll();
    } else {
      this.loadChannels();
    }
  }
});