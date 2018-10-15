var $api = new utils.Api('/home/contentsLayerGroup');

var data = {
  siteId: parseInt(utils.getQueryString('siteId')),
  channelId: parseInt(utils.getQueryString('channelId')),
  contentIds: utils.getQueryString('contentIds'),
  pageLoad: false,
  pageAlert: null,
  pageType: 'setGroup',
  groupNames: null,
  selected: [],
  groupName: '',
  description: ''
};

var methods = {
  loadConfig: function () {
    var $this = this;

    $api.get({
        siteId: $this.siteId,
        channelId: $this.channelId,
        contentIds: $this.contentIds
      },
      function (err, res) {
        if (err || !res || !res.value) return;

        $this.groupNames = res.value;
        $this.pageLoad = true;
      }
    );
  },
  btnSubmitClick: function () {
    var $this = this;

    parent.utils.loading(true);
    $api.post({
        siteId: $this.siteId,
        channelId: $this.channelId,
        contentIds: $this.contentIds,
        pageType: $this.pageType,
        groupNames: $this.selected.join(','),
        groupName: $this.groupName,
        description: $this.description
      },
      function (err, res) {
        if (err || !res || !res.value) return;

        parent.location.reload(true);
      }
    );
  }
};

new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.loadConfig();
  }
});