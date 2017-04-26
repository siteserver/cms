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
}

interface S {
  logs: Array<models.UserLog>
}

class LogsLoginPage extends React.Component<P, S> {
  constructor(props: P) {
    super(props)
    this.state = {
      logs: null
    }
  }

  componentDidMount() {
    client.users.getLogs(20, 'Login', (err: models.Error, res: Array<models.UserLog>) => {
      this.state.logs = res
      this.setState(this.state)
    })
  }

  render() {
    let logsEl = []
    if (this.state.logs) {
      logsEl = this.state.logs.map((log: models.UserLog) => {
        return (
          <tr key={log.id}>
            <td>{utils.toDateString(log.addDate)}</td>
            <td>{utils.toTimeString(log.addDate)}</td>
            <td>{log.ipAddress}</td>
          </tr>
        )
      })
    }

    return (
      <div className="article">
        <div className="mod-3">
          <div className="art-mod">
            <div className="mod-reslut-t2">
              <h3 className="f-s18"><i className="ico ico-succ" />您的帐号近期没有异常登录</h3>
              <p className="f-s14">请您通过登录时间判断是否为您本人操作，如确定非您本人登录，建议您 <Link to="/security/password" className="lnk">修改密码</Link></p>
            </div>
            <div className="mod-reslut-t2">
              <table className="table2" width="100%">
                <thead>
                  <tr>
                    <th>日期</th>
                    <th>时间</th>
                    <th>IP</th>
                  </tr>
                </thead>
                <tbody>
                  {logsEl}
                </tbody>
              </table>
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

export default connect(mapStateToProps, null)(LogsLoginPage)
