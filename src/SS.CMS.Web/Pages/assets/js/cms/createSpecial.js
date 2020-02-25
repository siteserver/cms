var $url = '/admin/cms/create/createSpecial';

var data = utils.initData({
  siteId: utils.getQueryInt('siteId'),
  isAllChecked: false,
  allSpecials: null,
  specials: null,
  checkedSpecialIds: [],
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

      $this.allSpecials = $this.specials = res.specials;
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiCreate: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, {
      siteId: this.siteId,
      specialIds: this.checkedSpecialIds
    }).then(function (response) {
      location.href = 'createStatus.cshtml?siteId=' + $this.siteId;
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
      this.checkedSpecialIds = _.map(this.specials, function(t) {
        return t.id;
      });
    } else {
      this.checkedSpecialIds = [];
    }
  },

  btnCreateClick: function() {
    this.apiCreate();
  },

  getLabel: function(special) {
    return special.title + '(' + special.url.replace('@', '') + ')';
  },

  handleCheckedSpecialsChange: function(value) {
    var checkedCount = value.length;
    this.isAllChecked = checkedCount === this.specials.length;
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  watch: {
    filterText: function(val) {
      if (val) {
        this.specials = _.filter(this.allSpecials, function(o) { return o.url.indexOf(val) !== -1; });
      } else {
        this.specials = this.allSpecials
      }
    }
  },
  created: function () {
    this.apiGet();
  }
});