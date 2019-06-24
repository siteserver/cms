var $api = new apiUtils.Api(apiUrl + '/pages/shared/tableValidate');

var data = {
  allRules: [{
    type: "required",
    text: "字段为必填项"
  }, {
    type: "numeric",
    text: "字段必须仅包含数字"
  }, {
    type: "email",
    text: "字段必须是有效的电子邮件"
  }, {
    type: "mobile",
    text: "字段必须是有效的手机号码"
  }, {
    type: "url",
    text: "字段必须是有效的url"
  }, {
    type: "alpha",
    text: "字段只能包含英文字母",
  }, {
    type: "alpha_dash",
    text: "字段只能包含英文字母、数字、破折号或下划线"
  }, {
    type: "alpha_num",
    text: "字段只能包含英文字母或数字"
  }, {
    type: "alpha_spaces",
    text: "字段只能包含英文字母或空格"
  }, {
    type: "credit_card",
    text: "字段必须是有效的信用卡"
  }, {
    type: "between",
    text: "字段必须有一个以最小值和最大值为界的数值"
  }, {
    type: "decimal",
    text: "字段必须是数字，并且可能包含指定数量的小数点"
  }, {
    type: "digits",
    text: "字段必须是整数，并且具有指定的位数"
  }, {
    type: "included",
    text: "字段必须具有指定列表中的值"
  }, {
    type: "excluded",
    text: "字段不能具有指定列表中的值"
  }, {
    type: "ip",
    text: "字段必须是一个有效的ipv4值的字符串"
  }, {
    type: "max",
    text: "字段不能超过指定的长度"
  }, {
    type: "max_value",
    text: "字段必须是数值，并且不能大于指定的值"
  }, {
    type: "min",
    text: "字段不能低于指定的长度"
  }, {
    type: "min_value",
    text: "字段必须是数值，并且不能小于指定的值"
  }, {
    type: "regex",
    text: "字段必须匹配指定的正则表达式"
  }],
  tableName: pageUtils.getQueryStringByName('tableName'),
  attributeName: pageUtils.getQueryStringByName('attributeName'),
  relatedIdentities: pageUtils.getQueryStringByName('relatedIdentities'),
  pageLoad: false,
  pageAlert: null,
  pageType: 'list',
  validateRules: [],
  ruleType: null,
  ruleValue: null,
  betweenMin: null,
  betweenMax: null,
  decimals: null,
  digitsLength: null,
  includedList: null,
  maxLength: null,
  maxValue: null,
  minLength: null,
  minValue: null,
  excludedList: null,
  regexValue: null
};

var methods = {
  getValue() {
    if (this.validateRules.length === 0) return '';
    return _.map(this.validateRules, function (rule) {
      return rule.value ? rule.type + ':' + rule.value : rule.type;
    }).join('|');
  },
  load: function () {
    var $this = this;

    $api.get({
      tableName: this.tableName,
      attributeName: this.attributeName,
      relatedIdentities: this.relatedIdentities
    }, function (err, res) {
      $this.pageLoad = true;
      if (err || !res) return;

      var val = '';
      try {
        if (res.value) {
          val = res.value;
          for (var i = 0; i < val.split('|').length; i++) {
            var element = val.split('|')[i];
            if (element.indexOf(':') === -1) {
              $this.validateRules.push({
                type: element,
                value: null
              });
            } else {
              $this.validateRules.push({
                type: element.split(':')[0],
                value: element.split(':')[1]
              });
            }
          }
        }
      } catch (e) {}
    });
  },
  btnSubmitClick: function () {
    var $this = this;
    this.$validator.validate().then(function (result) {
      if (result) {
        pageUtils.loading(true);
        $api.post({
          tableName: $this.tableName,
          attributeName: $this.attributeName,
          relatedIdentities: $this.relatedIdentities,
          value: $this.getValue()
        }, function (err, res) {
          pageUtils.loading(false);
          if (err || !res) {
            $this.pageAlert = {
              type: 'danger',
              html: err.message
            }
            return;
          }

          parent.reloadPage();
          pageUtils.closeLayer();
        });
      }
    });
  },
  btnRemoveClick: function (index) {
    this.validateRules.splice(index, 1);
  },
  btnAddClick: function () {
    this.ruleType = null;
    this.pageType = 'add';
  },
  btnSaveClick: function () {
    var $this = this;
    this.$validator.validate().then(function (result) {
      if (result) {
        $this.validateRules.push({
          type: $this.ruleType,
          value: $this.getRuleValue()
        });
        $this.pageType = 'list';
      }
    });
  },
  btnCancelClick: function () {
    this.pageType = 'list';
  },
  getDescription(type) {
    var index = _.findIndex(this.allRules, function (o) {
      return o.type == type;
    });
    return index !== -1 ? this.allRules[index].text : '';
  },
  getRuleValue() {
    if (this.ruleType === 'between') {
      return this.betweenMin + ',' + this.betweenMax;
    } else if (this.ruleType === 'decimal') {
      return this.decimals;
    } else if (this.ruleType === 'digits') {
      return this.digitsLength;
    } else if (this.ruleType === 'included') {
      return this.includedList;
    } else if (this.ruleType === 'max') {
      return this.maxLength;
    } else if (this.ruleType === 'max_value') {
      return this.maxValue;
    } else if (this.ruleType === 'min') {
      return this.minLength;
    } else if (this.ruleType === 'min_value') {
      return this.minValue;
    } else if (this.ruleType === 'excluded') {
      return this.excludedList;
    } else if (this.ruleType === 'regex') {
      return this.regexValue;
    }

    return '';
  },
  getAvaliableRules() {
    var rules = [];
    for (var i = 0; i < this.allRules.length; i++) {
      var element = this.allRules[i];
      var index = _.findIndex(this.validateRules, function (o) {
        return o.type == element.type;
      });
      if (index === -1) {
        rules.push(this.allRules[i])
      }
    }
    return rules;
  }
};

new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.load();
  }
});