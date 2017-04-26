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
}

class DetailPage extends React.Component<P, S> {
  constructor(props: P) {
    super(props)
    const user = utils.assign({}, props.account.user)

    this.state = {
      loading: false,
      user: user
    }
  }

  loading(loading: boolean) {
    this.state.loading = loading
    this.setState(this.state)
  }

  render() {
    const interests = (this.state.user.interests || '').split(',')

    return (
      <div className="article">
        <div className="mod-3">
          <div className="art-mod">
            <div className="art-hd clearfix"><h2>详细资料</h2></div>
            <div>
              <div className="form form-1 mod-reslut-t2">
                <form>
                  <ul>
                    <li className="fm-item">
                      <label htmlFor="#" className="k">所在单位：</label>
                      <span className="v">
                        <input value={this.state.user.organization} onChange={(e: any) => {
                          this.state.user.organization = e.target.value
                          this.setState(this.state)
                        } } type="text" className="text input-xxl" />
                      </span>
                    </li>
                    <li className="fm-item">
                      <label htmlFor="#" className="k">所在部门：</label>
                      <span className="v">
                        <input value={this.state.user.department} onChange={(e: any) => {
                          this.state.user.department = e.target.value
                          this.setState(this.state)
                        } } type="text" className="text input-xxl" />
                      </span>
                    </li>
                    <li className="fm-item">
                      <label htmlFor="#" className="k">职位：</label>
                      <span className="v">
                        <input value={this.state.user.position} onChange={(e: any) => {
                          this.state.user.position = e.target.value
                          this.setState(this.state)
                        } } type="text" className="text input-xxl" />
                      </span>
                    </li>
                    <li className="fm-item">
                      <label htmlFor="#" className="k">教育程度：</label>
                      <span className="v" style={{ width: 380 }}>
                        <Select
                          placeholder="选择..."
                          clearable={false}
                          className="select input-xl"
                          style={{ width: 380 }}
                          value={this.state.user.education}
                          options={[
                            { value: '初中/小学', label: '初中/小学' },
                            { value: '高中/中专', label: '高中/中专' },
                            { value: '本科/专科', label: '本科/专科' },
                            { value: '研究生以上', label: '研究生以上' },
                          ]}
                          onChange={(item: {
                            value: string
                            label: string
                          }) => {
                            this.state.user.education = item.value
                            this.setState(this.state)
                          } }
                          />
                      </span>
                    </li>
                    <li className="fm-item">
                      <label htmlFor="#" className="k">毕业院校：</label>
                      <span className="v">
                        <input value={this.state.user.graduation} onChange={(e: any) => {
                          this.state.user.graduation = e.target.value
                          this.setState(this.state)
                        } } type="text" className="text input-xxl" />
                      </span>
                    </li>
                    <li className="fm-item">
                      <label htmlFor="#" className="k">地址：</label>
                      <span className="v">
                        <input value={this.state.user.address} onChange={(e: any) => {
                          this.state.user.address = e.target.value
                          this.setState(this.state)
                        } } type="text" className="text input-xxl" />
                      </span>
                    </li>
                    <li className="fm-item">
                      <label htmlFor="#" className="k">兴趣爱好：</label>
                      <span className="v">
                        <p className="hobby-cont">
                          <label htmlFor="cbox_1" className="item hobby-item"><input id="cbox_1" type="checkbox" checked={interests.indexOf('财经股市') !== -1} onChange={() => {
                            if (interests.indexOf('财经股市') !== -1) {
                              interests.splice(interests.indexOf('财经股市'), 1)
                            } else {
                              interests.push('财经股市')
                            }
                            this.state.user.interests = interests.join(',')
                            this.setState(this.state)
                          }} /> 财经股市</label>
                          <label htmlFor="cbox_2" className="item hobby-item"><input id="cbox_2" type="checkbox" checked={interests.indexOf('房产家居') !== -1} onChange={() => {
                            if (interests.indexOf('房产家居') !== -1) {
                              interests.splice(interests.indexOf('房产家居'), 1)
                            } else {
                              interests.push('房产家居')
                            }
                            this.state.user.interests = interests.join(',')
                            this.setState(this.state)
                          }} /> 房产家居</label>
                          <label htmlFor="cbox_3" className="item hobby-item"><input id="cbox_3" type="checkbox" checked={interests.indexOf('图书') !== -1} onChange={() => {
                            if (interests.indexOf('图书') !== -1) {
                              interests.splice(interests.indexOf('图书'), 1)
                            } else {
                              interests.push('图书')
                            }
                            this.state.user.interests = interests.join(',')
                            this.setState(this.state)
                          }} /> 图书</label>
                          <label htmlFor="cbox_4" className="item hobby-item"><input id="cbox_4" type="checkbox" checked={interests.indexOf('娱乐') !== -1} onChange={() => {
                            if (interests.indexOf('娱乐') !== -1) {
                              interests.splice(interests.indexOf('娱乐'), 1)
                            } else {
                              interests.push('娱乐')
                            }
                            this.state.user.interests = interests.join(',')
                            this.setState(this.state)
                          }} /> 娱乐</label>
                          <label htmlFor="cbox_5" className="item hobby-item"><input id="cbox_5" type="checkbox" checked={interests.indexOf('旅游') !== -1} onChange={() => {
                            if (interests.indexOf('旅游') !== -1) {
                              interests.splice(interests.indexOf('旅游'), 1)
                            } else {
                              interests.push('旅游')
                            }
                            this.state.user.interests = interests.join(',')
                            this.setState(this.state)
                          }} /> 旅游</label>
                        </p>
                        <p className="hobby-cont">
                          <label htmlFor="cbox_6" className="item hobby-item"><input id="cbox_6" type="checkbox" checked={interests.indexOf('体育') !== -1} onChange={() => {
                            if (interests.indexOf('体育') !== -1) {
                              interests.splice(interests.indexOf('体育'), 1)
                            } else {
                              interests.push('体育')
                            }
                            this.state.user.interests = interests.join(',')
                            this.setState(this.state)
                          }} /> 体育</label>
                          <label htmlFor="cbox_7" className="item hobby-item"><input id="cbox_7" type="checkbox" checked={interests.indexOf('汽车') !== -1} onChange={() => {
                            if (interests.indexOf('汽车') !== -1) {
                              interests.splice(interests.indexOf('汽车'), 1)
                            } else {
                              interests.push('汽车')
                            }
                            this.state.user.interests = interests.join(',')
                            this.setState(this.state)
                          }} /> 汽车</label>
                          <label htmlFor="cbox_8" className="item hobby-item"><input id="cbox_8" type="checkbox" checked={interests.indexOf('游戏聊天') !== -1} onChange={() => {
                            if (interests.indexOf('游戏聊天') !== -1) {
                              interests.splice(interests.indexOf('游戏聊天'), 1)
                            } else {
                              interests.push('游戏聊天')
                            }
                            this.state.user.interests = interests.join(',')
                            this.setState(this.state)
                          }} /> 游戏聊天</label>
                          <label htmlFor="cbox_9" className="item hobby-item"><input id="cbox_9" type="checkbox" checked={interests.indexOf('IT数码') !== -1} onChange={() => {
                            if (interests.indexOf('IT数码') !== -1) {
                              interests.splice(interests.indexOf('IT数码'), 1)
                            } else {
                              interests.push('IT数码')
                            }
                            this.state.user.interests = interests.join(',')
                            this.setState(this.state)
                          }} /> IT数码</label>
                          <label htmlFor="cbox_10" className="item hobby-item"><input id="cbox_10" type="checkbox" checked={interests.indexOf('购物') !== -1} onChange={() => {
                            if (interests.indexOf('购物') !== -1) {
                              interests.splice(interests.indexOf('购物'), 1)
                            } else {
                              interests.push('购物')
                            }
                            this.state.user.interests = interests.join(',')
                            this.setState(this.state)
                          }} /> 购物</label>
                        </p></span>
                    </li>
                  </ul>
                  <div className="btns"><span className="btn btn-2 modify-detail-user-info" onClick={() => {
                    this.loading(true)
                    client.users.edit({
                      organization: this.state.user.organization,
                      department: this.state.user.department,
                      position: this.state.user.position,
                      education: this.state.user.education,
                      graduation: this.state.user.graduation,
                      address: this.state.user.address,
                      interests: utils.trim(this.state.user.interests, ','),
                    }, (err: models.Error, res: models.User) => {
                      this.loading(false)
                      if (!err) {
                        this.props.account.user.organization = res.organization
                        this.props.account.user.department = res.department
                        this.props.account.user.position = res.position
                        this.props.account.user.education = res.education
                        this.props.account.user.graduation = res.graduation
                        this.props.account.user.address = res.address
                        this.props.account.user.interests = res.interests

                        this.props.update(this.props.account)
                        utils.Swal.success('详细资料修改成功', () => {
                          hashHistory.push('/')
                        })
                      } else {
                        utils.Swal.error(err)
                      }
                    })
                  } }>保存修改</span></div>
                </form>
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

export default connect(mapStateToProps, mapDispatchToProps)(DetailPage)
