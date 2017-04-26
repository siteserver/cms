import * as http from '../../http'
import * as models from '../../../models'

export class Users {
  private request: http.APIRequest

  constructor(request: http.APIRequest) {
    this.request = request
  }

  loadConfig(cb?: (err: models.Error, res: models.Config) => void) {
    this.request.post('/users/actions/load_config', {}, cb)
  }

  login(account: string, password: string, cb?: (err: models.Error, res: models.Account) => void) {
    this.request.post('/users/actions/login', {
      account, password
    }, cb)
  }

  logout(cb?: (err: models.Error, res: {}) => void) {
    this.request.post('/users/actions/logout', null, cb)
  }

  resetPassword(password: string, newPassword: string, confirmPassword: string, cb?: (err: models.Error, res: { lastResetPasswordDate: Date }) => void) {
    this.request.post('/users/actions/reset_password', {
      password, newPassword, confirmPassword
    }, cb)
  }

  resetPasswordByToken(token: string, password: string, cb?: (err: models.Error, res: {
    isSuccess: boolean
    errorMessage: string
  }) => void) {
    this.request.post('/users/actions/reset_password_by_token', {
      token, password
    }, cb)
  }

  edit(data: Object, cb: (err: models.Error, res: models.User) => void) {
    this.request.post(`/users/actions/edit`, data, cb)
  }

  getLogs(totalNum: number, action: string, cb: (err: models.Error, res: Array<models.UserLog>) => void) {
    this.request.post(`/users/actions/get_logs`, {
      totalNum, action
    }, cb)
  }

  isMobileExists(mobile: string, cb: (err: models.Error, res: {
    exists: boolean
  }) => void) {
    this.request.post(`/users/actions/is_mobile_exists/`, {
      mobile
    }, cb)
  }

  isPasswordCorrect(password: string, cb: (err: models.Error, res: {
    isCorrect: boolean
    errorMessage: string
  }) => void) {
    this.request.post(`/users/actions/is_password_correct/`, {
      password
    }, cb)
  }

  isCodeCorrect(mobile: string, code: string, cb: (err: models.Error, res: {
    isCorrect: boolean
    token: string
  }) => void) {
    this.request.post(`/users/actions/is_code_correct/`, {
      mobile, code
    }, cb)
  }

  sendSmsOrRegister(mobile: string, password: string, cb: (err: models.Error, res: {
    isSms: boolean
    isRegister: boolean
    errorMessage: string
  }) => void) {
    this.request.post(`/users/actions/send_sms_or_register/`, {
      mobile, password
    }, cb)
  }

  sendSms(account: string, cb: (err: models.Error, res: {
    isSuccess: boolean
    mobile: string
    errorMessage: string
  }) => void) {
    this.request.post(`/users/actions/send_sms/`, {
      account
    }, cb)
  }

  registerWithCode(mobile: string, password: string, code: string, cb: (err: models.Error, res: {
    isRegister: boolean
    errorMessage: string
  }) => void) {
    this.request.post(`/users/actions/register_with_code/`, {
      mobile, password, code
    }, cb)
  }

  getCaptchaUrl(code: string) {
    return this.request.getURL(`/users/captcha/${code}`)
  }
}
