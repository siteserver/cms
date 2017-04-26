import * as React from 'react'
import cx from 'classnames'
import { IndexLink, Link, hashHistory } from 'react-router'
import { connect } from 'react-redux'
import { bindActionCreators } from 'redux'
import Upload from 'rc-upload'
import { Action } from '../../../lib/actions/types'
import { update } from '../../../lib/actions/account'
import { Loading } from '../../../components'
import * as models from '../../../lib/models'
import * as utils from '../../../lib/utils'
import client from '../../../lib/client'
import Select from 'react-select'

interface P {
  config: models.Config
  account: models.Account
  update: (account: models.Account) => Action
}

interface S {
  loading: boolean
  user: models.User
  year: number
  month: number
  date: number
  errors: {[index: string]: boolean}
}

class BasicPage extends React.Component<P, S> {
  constructor(props: P) {
    super(props)
    const user = utils.assign({}, props.account.user)
    const year = user.birthday ? utils.toDate(user.birthday).getFullYear() : 0
    const month = user.birthday ? utils.toDate(user.birthday).getMonth() + 1 : 0
    const date = user.birthday ? utils.toDate(user.birthday).getDate() : 0

    this.state = {
      loading: false,
      user: user,
      year: year,
      month: month,
      date: date,
      errors: {}
    }
  }

  loading(loading: boolean) {
    this.state.loading = loading
    this.setState(this.state)
  }

  render() {
    const now = new Date()
    const yearOptions = []
    for (let y = now.getFullYear(); y >= 1900; y--) {
      yearOptions.push({
        value: y, label: y.toString() + '年'
      })
    }
    const monthOptions = []
    for (let m = 1; m <= 12; m++) {
      monthOptions.push({
        value: m, label: m.toString() + '月'
      })
    }
    const dayOptions = []
    for (let d = 1; d <= 31; d++) {
      dayOptions.push({
        value: d, label: d.toString() + '日'
      })
    }

    return (
      <div className="article">
        <div className="mod-3">
          <div className="art-mod">
            <div className="art-hd clearfix"><h2>基本资料</h2></div>
            <div className="art-bd">
              <div className="form form-1 mod-reslut-t2">
                <ul style={{ marginBottom: 52 }}>
                  <li className="fm-item">
                    <label htmlFor="#" className="k">姓 名：</label>
                    <span className="v">
                      <input value={this.state.user.displayName} onChange={(e: any) => {
                        this.state.user.displayName = e.target.value
                        this.setState(this.state)
                      }} type="text" className={cx('text input-xxl', {'error': this.state.errors['displayName']})} />
                    </span>
                    <span className="text-error" style={{display: this.state.errors['displayName'] ? '' : 'none'}}><i className="ico ico-err-2" /><em className="error-message">请设置姓名</em></span>
                  </li>
                  <li className="fm-item">
                    <label htmlFor="#" className="k">性 别：</label>
                    <span className="v">
                      <label className="item"><input onChange={() => {
                        this.state.user.gender = '男'
                        this.setState(this.state)
                      } } type="radio" className="radio" checked={this.state.user.gender === '男'} /> <span>男</span></label>
                      <label className="item"><input onChange={() => {
                        this.state.user.gender = '女'
                        this.setState(this.state)
                      } } type="radio" className="radio" checked={this.state.user.gender === '女'} /> <span>女</span></label>
                    </span>
                    <span className="text-error" style={{display: this.state.errors['gender'] ? '' : 'none'}}><i className="ico ico-err-2" /><em className="error-message">请设置性别</em></span>
                  </li>
                  <li className="fm-item" style={{marginBottom: 10}}>
                    <label htmlFor="#" className="k">生 日：</label>
                    <span className="v">
                      <span style={{ width: 80, float: 'left' }}>
                        <Select
                          clearable={false}
                          placeholder="选择..."
                          className="select input-s"
                          optionClassName="select input-s"
                          style={{width: 70}}
                          value={this.state.year}
                          options={yearOptions}
                          onChange={(item: {
                            value: number
                            label: string
                          }) => {
                            this.state.year = item.value
                            this.setState(this.state)
                          } }
                          />
                      </span>
                      <span style={{ width: 80, float: 'left' }}>
                        <Select
                          clearable={false}
                          placeholder="选择..."
                          className="select input-s"
                          optionClassName="select input-s"
                          style={{width: 70}}
                          value={this.state.month}
                          options={monthOptions}
                          onChange={(item: {
                            value: number
                            label: string
                          }) => {
                            this.state.month = item.value
                            this.setState(this.state)
                          } }
                          />
                      </span>
                      <span style={{ width: 80, float: 'left' }}>
                        <Select
                          placeholder="选择..."
                          clearable={false}
                          className="select input-s"
                          optionClassName="select input-s"
                          style={{width: 70}}
                          value={this.state.date}
                          options={dayOptions}
                          onChange={(item: {
                            value: number
                            label: string
                          }) => {
                            this.state.date = item.value
                            this.setState(this.state)
                          } }
                          />
                      </span>
                    </span>
                    <span className="text-error" style={{display: this.state.errors['year'] || this.state.errors['month'] || this.state.errors['date'] ? '' : 'none'}}><i className="ico ico-err-2" /><em className="error-message">请设置生日</em></span>
                  </li>
                  <li className="fm-item">
                    <label htmlFor="#" className="k">个性签名：</label>
                    <span className="v">
                      <textarea maxLength={100} placeholder="不超过100个汉字（支持中英文、数字与标点）" className="textarea" value={this.state.user.signature} onChange={(e: any) => {
                        this.state.user.signature = e.target.value
                        this.setState(this.state)
                      }} />
                    </span>
                  </li>
                </ul>
                <div className="btns">
                  <span className="btn btn-2 modify-basic-info-btn" onClick={() => {
                    let errors: {[index: string]: boolean} = {}

                    if (!this.state.user.gender) {
                      errors['gender'] = true
                    }
                    if (!this.state.year) {
                      errors['year'] = true
                    }
                    if (!this.state.month) {
                      errors['month'] = true
                    }
                    if (!this.state.date) {
                      errors['date'] = true
                    }

                    this.state.errors = errors
                    this.setState(this.state)

                    if (utils.keys(errors).length > 0) return

                    this.loading(true)
                    client.users.edit({
                      displayName: this.state.user.displayName,
                      gender: this.state.user.gender,
                      birthday: utils.toDateString(new Date(this.state.year, this.state.month - 1, this.state.date)),
                      signature: this.state.user.signature
                    }, (err: models.Error, res: models.User) => {
                      this.loading(false)
                      if (!err) {
                        this.props.account.user.displayName = res.displayName
                        this.props.account.user.gender = res.gender
                        this.props.account.user.birthday = res.birthday
                        this.props.account.user.signature = res.signature

                        this.props.update(this.props.account)
                        utils.Swal.success('基本资料修改成功', () => {
                          hashHistory.push('/')
                        })
                      } else {
                        utils.Swal.error(err)
                      }
                    })
                  }}>保存修改</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    );
  }
}

function mapStateToProps(state) {
  return {
    config: state.config,
    account: state.account,
  }
}

function mapDispatchToProps(dispatch) {
  return {
    update: bindActionCreators(update, dispatch),
  }
}

export default connect(mapStateToProps, mapDispatchToProps)(BasicPage)
