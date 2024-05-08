$apiUrl = utils.getQueryString('apiUrl') || $formConfigApiUrl;
$rootUrl = "/";
$token = localStorage.getItem(ACCESS_TOKEN_NAME);

var $api = axios.create({
  baseURL: $apiUrl,
  headers: {
    Authorization: "Bearer " + $token,
  },
});

var $url = '/v1/forms';

var data = utils.init({
  siteId: utils.getQueryInt('siteId') || $formConfigSiteId,
  channelId: utils.getQueryInt('channelId'),
  contentId: utils.getQueryInt('contentId'),
  formId: utils.getQueryInt('formId') || $formConfigFormId,
  word: utils.getQueryString('word'),
  pageType: 'loading',
  styleList: null,
  allAttributeNames: [],
  listAttributeNames: [],
  isReply: false,
  total: null,
  pageSize: null,
  page: 1,
  items: [],
  columns: null,
});

var methods = {
  apiGet: function (page) {
    var $this = this;

    this.pageType = 'loading';
    $api.get($url, {
      params: {
        siteId: this.siteId,
        channelId: this.channelId,
        contentId: this.contentId,
        formId: this.formId,
        page: page,
        word: this.word
      }
    }).then(function (response) {
      var res = response.data;

      $this.page = page;
      $this.styleList = res.styleList;
      $this.allAttributeNames = res.allAttributeNames;
      $this.listAttributeNames = res.listAttributeNames;
      $this.isReply = res.isReply;
      $this.items = res.items;
      $this.total = res.total;
      $this.pageSize = res.pageSize;
      $this.columns = res.columns;

      $this.pageType = 'list';
      document.documentElement.scrollTop = document.body.scrollTop = 0;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  getAttributeText: function (attributeName) {
    var column = this.columns.find(function (x) {
      return x.attributeName === attributeName;
    })
    return column.displayName;
  },

  getAttributeType: function(attributeName) {
    var style = _.find(this.styleList, function(o) {return o.title === attributeName});
    if (style && style.fieldType) return style.fieldType;
    return 'Text';
  },

  getAttributeValue: function (item, attributeName) {
    return item[utils.toCamelCase(attributeName)];
  },

  largeImage: function(item, attributeName) {
    var imageUrl = this.getAttributeValue(item, attributeName);
    Swal.fire({
      imageUrl: imageUrl,
      showConfirmButton: false,
    })
  },

  handleCurrentChange: function(val) {
    this.apiGet(val);
  },
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    this.apiGet(1);
  }
});
