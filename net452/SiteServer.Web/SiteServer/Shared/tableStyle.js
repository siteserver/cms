var $url = '/pages/shared/tableStyle';

var $data = {
  tableName: utils.getQueryString('tableName'),
  attributeName: utils.getQueryString('attributeName'),
  relatedIdentities: utils.getQueryString('relatedIdentities'),
  pageLoad: false,
  pageAlert: null,
  styleInfo: null,
  inputTypes: null,
  isRapid: null,
  rapidValues: null
};

var $methods = {
  getStyle: function () {
    var $this = this;

    $api.get($url, {
      params: {
        tableName: $this.tableName,
        attributeName: $this.attributeName,
        relatedIdentities: $this.relatedIdentities
      }
    }).then(function (response) {
      var res = response.data;

      $this.styleInfo = res.value;
      $this.inputTypes = res.inputTypes;
      $this.isRapid = res.isRapid;
      $this.rapidValues = res.rapidValues;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
    });
  },

  btnSubmitClick: function () {
    var $this = this;
    this.$validator.validate().then(function (result) {
      if (result) {
        utils.loading(true);
        $api.post($url, {
          tableName: $this.tableName,
          attributeName: $this.attributeName,
          relatedIdentities: $this.relatedIdentities,
          styleInfo: $this.styleInfo
        }).then(function (response) {
          var res = response.data;

          parent.location.reload();
          utils.closeLayer();
        }).catch(function (error) {
          $this.pageAlert = utils.getPageAlert(error);
        }).then(function () {
          utils.loading(false);
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
  data: $data,
  methods: $methods,
  created: function () {
    this.getStyle();
  }
});