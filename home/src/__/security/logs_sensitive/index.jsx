import * as React from 'react';
import { Link } from 'react-router';
import { connect } from 'react-redux';
import * as utils from '../../../lib/utils';
import client from '../../../lib/client';
class LogsLoginPage extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            logs: null
        };
    }
    componentDidMount() {
        client.users.getLogs(20, '', (err, res) => {
            this.state.logs = res;
            this.setState(this.state);
        });
    }
    render() {
        let logsEl = [];
        if (this.state.logs) {
            logsEl = this.state.logs.map((log) => {
                return (<tr key={log.id}>
            <td>{utils.toDateString(log.addDate)}</td>
            <td>{utils.toTimeString(log.addDate)}</td>
            <td>{log.ipAddress}</td>
            <td>{log.action}</td>
            <td>{log.summary}</td>
          </tr>);
            });
        }
        return (<div className="article">
        <div className="mod-3">
          <div className="art-mod">
            <div className="art-hd clearfix"><h2>敏感操作</h2></div>
            <div className="art-bd">
              <div className="mod-reslut-t3">
                <p className="f-s14">敏感操作是指修改密码、设置手机或邮箱、新增/编辑/删除稿件等可能影响到您帐号安全的操作；</p>
                <p className="f-s14">如确定非您本人操作，建议您 <Link to="/security/password" className="lnk">修改密码</Link></p>
                <table className="table2" width="100%">
                  <thead>
                    <tr>
                      <th>日期</th>
                      <th>时间</th>
                      <th>IP</th>
                      <th>操作</th>
                      <th>备注</th>
                    </tr>
                    {logsEl}
                  </thead>
                  <tbody>
                  </tbody>
                </table>
              </div>
            </div>
          </div>
        </div>
      </div>);
    }
}
function mapStateToProps(state) {
    return {
        config: state.config,
        account: state.account,
    };
}
export default connect(mapStateToProps, null)(LogsLoginPage);
//# sourceMappingURL=index.jsx.map