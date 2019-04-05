import Vue from 'vue'
import cn from 'vee-validate/dist/locale/zh_CN'
import VeeValidate, { Validator } from 'vee-validate'

Vue.use(VeeValidate, { inject: false })
Validator.localize('zh_CN', cn)
Validator.localize({
  zh_CN: {
    messages: {
      required: (name) => {
        return name + '为必填项'
      }
    }
  }
})
Validator.extend('mobile', {
  getMessage: () => {
    return ' 请输入正确的手机号码'
  },
  validate: (value, args) => {
    return (
      value.length === 11 &&
        /^((13|14|15|16|17|18|19)[0-9]{1}\d{8})$/.test(value)
    )
  }
})
