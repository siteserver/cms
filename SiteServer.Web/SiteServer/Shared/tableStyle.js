var $api = new apiUtils.Api(apiUrl + '/pages/shared/tableStyle');

var data = {
  tableName: utils.getQueryString('tableName'),
  attributeName: utils.getQueryString('attributeName'),
  relatedIdentities: utils.getQueryString('relatedIdentities'),
  pageLoad: false,
  pageAlert: null,
  styleInfo: null,
  inputTypes: null,
  isRapid: null,
  rapidValues: null,
};

var methods = {
  getStyle: function () {
    var $this = this;

    $api.get({
      tableName: this.tableName,
      attributeName: this.attributeName,
      relatedIdentities: this.relatedIdentities
    }, function (err, res) {
      $this.pageLoad = true;
      if (err || !res) return;

      $this.styleInfo = res.value;
      $this.inputTypes = res.inputTypes;
      $this.isRapid = res.isRapid;
      $this.rapidValues = res.rapidValues;
    });
  },
  btnSubmitClick: function () {
    var $this = this;
    this.$validator.validate().then(function (result) {
      if (result) {
        utils.loading(true);
        $api.post({
          tableName: $this.tableName,
          attributeName: $this.attributeName,
          relatedIdentities: $this.relatedIdentities,
          styleInfo: $this.styleInfo
        }, function (err, res) {
          utils.loading(false);
          if (err || !res) {
            $this.pageAlert = {
              type: 'danger',
              html: err.message
            }
            return;
          }

          parent.reloadPage();
          utils.closeLayer();
        });
      }
    });
  },
  btnStyleItemRemoveClick: function (index) {
    this.styleInfo.styleItems.splice(index, 1);
    if (this.styleInfo.styleItems.length === 0) {
      this.btnStyleItemAddClick();
    }
  },
  btnStyleItemAddClick: function () {
    this.styleInfo.styleItems.push({
      itemTitle: '',
      itemValue: '',
      isSelected: false
    })
  },
  btnRadioClick: function (index) {
    for (var i = 0; i < this.styleInfo.styleItems.length; i++) {
      var element = this.styleInfo.styleItems[i];
      element.isSelected = false;
    }
    this.styleInfo.styleItems[index].isSelected = true;
  }
};

new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.getStyle();
  }
});