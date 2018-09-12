var $apiUrl = $apiConfig.apiUrl;
var $api = new apiUtils.Api($apiUrl + '/pages/cms/contents');
var $siteId = parseInt(pageUtils.getQueryStringByName('siteId'));
var $channelId = parseInt(pageUtils.getQueryStringByName('channelId'));

var data = {
  pageLoad: false,
  pageAlert: null,
  pageType: null,
  page: 1,
  pageContents: null,
  count: null,
  pages: null,
  pageOptions: null,
  isAllChecked: false
};

var methods = {
  toggleChecked: function (content) {
    content.isChecked = !content.isChecked;
    if (!content.isChecked) {
      this.isAllChecked = false;
    }
  },
  selectAll: function () {
    this.isAllChecked = !this.isAllChecked;
    for (var i = 0; i < this.pageContents.length; i++) {
      this.pageContents[i].isChecked = this.isAllChecked;
    }
  },
  create: function () {
    var $this = this;

    $this.pageLoad = false;

    var channelIdList = [];
    for (var i = 0; i < this.pageContents.length; i++) {
      if (this.pageContents[i].isChecked) {
        channelIdList.push(this.pageContents[i].id);
      }
    }

    if (!$this.isAllChecked && channelIdList.length === 0) return;

    $api.post({
      siteId: $siteId,
      channelIdList: $this.isAllChecked ? [] : channelIdList,
      isAllChecked: $this.isAllChecked,
      scope: $this.scope
    }, function () {
      location.href = '../settings/pageCreateStatus.aspx?siteId=' + $siteId;
    });
  },
  loadFirstPage: function () {
    if (this.page === 1) return;
    this.loadContents(1);
  },
  loadPrevPage: function () {
    if (this.page - 1 <= 0) return;
    this.loadContents(this.page - 1);
  },
  loadNextPage: function () {
    if (this.page + 1 > this.pages) return;
    this.loadContents(this.page + 1);
  },
  loadLastPage: function () {
    if (this.page + 1 > this.pages) return;
    this.loadContents(this.pages);
  },
  onPageSelect(option) {
    this.loadContents(option);
  },
  scrollToTop() {
    document.documentElement.scrollTop = document.body.scrollTop = 0;
  },
  loadContents: function (page) {
    var $this = this;

    if ($this.pageLoad) {
      pageUtils.loading(true);
    }

    $api.get({
      siteId: $siteId,
      channelId: $channelId,
      page: page
    }, function (err, res) {
      if (err || !res || !res.value) return;

      $this.pageContents = [];
      for (var i = 0; i < res.value.length; i++) {
        var content = _.assign({}, res.value[i], {
          isChecked: false
        });
        $this.pageContents.push(content);
      }
      $this.count = res.count;
      $this.pages = res.pages;
      $this.page = page;
      $this.pageOptions = [];
      for (var i = 1; i <= $this.pages; i++) {
        $this.pageOptions.push(i);
      }

      if ($this.pageLoad) {
        pageUtils.loading(false);
        $this.scrollToTop();
      } else {
        $this.pageLoad = true;
      }
    });
  }
};

Vue.component('multiselect', window.VueMultiselect.default)

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.loadContents(1);
  }
});