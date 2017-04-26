import * as React from 'react'
import { IndexLink, Link } from 'react-router'
import { bindActionCreators } from 'redux'
import { connect } from 'react-redux'
import Select from 'react-select'
import cx from 'classnames'
import * as models from '../../../lib/models'
import { Loading } from '../../../components'
import client from '../../../lib/client'
import * as utils from '../../../lib/utils'

interface P {
  config: models.Config
  account: models.Account
  publishmentSystems: Array<models.PublishmentSystem>
}

interface S {
  publishmentSystem: models.PublishmentSystem
  nodes: Array<{
    nodeId: number
    nodeName: string
  }>
  node: {
    nodeId: number
    nodeName: string
  }
  contents: Array<models.Content>
  currentPage: number
  totalPage: number
}

class IndexPage extends React.Component<P, S> {
  constructor(props: P) {
    super(props)
    this.state = {
      publishmentSystem: null,
      nodes: null,
      node: null,
      contents: null,
      currentPage: 1,
      totalPage: 1,
    }
  }

  componentDidMount() {
    let publishmentSystem = null
    for (let ps of this.props.publishmentSystems) {
      if (this.props.account.user.additional.lastWritingPublishmentSystemId === ps.publishmentSystemId) {
        publishmentSystem = ps
      }
    }
    if (!publishmentSystem) publishmentSystem = this.props.publishmentSystems[0]

    this.loadPublishmentSystem(publishmentSystem)
  }

  loadPublishmentSystem(publishmentSystem: models.PublishmentSystem) {
    if (this.state.publishmentSystem && publishmentSystem.publishmentSystemId === this.state.publishmentSystem.publishmentSystemId) return

    client.writing.getChannels(publishmentSystem.publishmentSystemId, (err: models.Error, res: Array<{
      nodeId: number
      nodeName: string
    }>) => {
      this.state.publishmentSystem = publishmentSystem
      this.state.nodes = res
      this.setState(this.state)
      this.loadContents(publishmentSystem.publishmentSystemId, publishmentSystem.publishmentSystemId, 1)
    })
  }

  loadContents(publishmentSystemId: number, nodeId: number, page: number) {
    this.state.currentPage = page
    this.state.contents = null
    this.setState(this.state)
    const searchType = ''
    const keyword = ''
    const dateFrom = ''
    const dateTo = ''
    client.writing.getContents(publishmentSystemId, nodeId, searchType, keyword, dateFrom, dateTo, page, (err: models.Error, res: {
      results: Array<models.Content>
      totalPage: number
    }) => {
      this.state.contents = res.results
      this.state.totalPage = res.totalPage
      this.setState(this.state)
      if (page > 1) {
        //utils.DOM.scrollTo('header')
      }
      this.setState(this.state)
    })
  }

  onPageClick(page: number) {
    this.loadContents(this.state.publishmentSystem.publishmentSystemId, this.state.node ? this.state.node.nodeId : this.state.publishmentSystem.publishmentSystemId, page)
  }

  render() {
    if (!this.state.contents) return <Loading />

    let publishmentSystemsEl = null
    if (this.props.publishmentSystems.length > 0) {
      const options = this.props.publishmentSystems.map((publishmentSystem) => {
        return { value: publishmentSystem.publishmentSystemId, label: publishmentSystem.publishmentSystemName }
      })

      publishmentSystemsEl = (
        <Select
          clearable={false}
          value={this.state.publishmentSystem.publishmentSystemId}
          options={options}
          onChange={(item: {
            value: number
            label: string
          }) => {
            const publishmentSystem = utils.find(this.props.publishmentSystems, (publishmentSystem: models.PublishmentSystem) => {
              return publishmentSystem.publishmentSystemId === item.value
            })
            this.loadPublishmentSystem(publishmentSystem)
          } }
          />
      )
    }

    let nodesEl = null
    if (this.state.nodes.length > 0) {
      const options = this.state.nodes.map((node) => {
        return { value: node.nodeId, label: node.nodeName }
      })
      nodesEl = (
        <Select
          placeholder="全部栏目"
          clearable={true}
          options={options}
          value={this.state.node ? this.state.node.nodeId : 0}
          onChange={(item: {
            value: number
            label: string
          }) => {
            if (item) {
              this.state.node = {
                nodeId: item.value,
                nodeName: item.label
              }
            } else {
              this.state.node = null
            }
            this.setState(this.state)
            this.loadContents(this.state.publishmentSystem.publishmentSystemId, item ? item.value : this.state.publishmentSystem.publishmentSystemId, 1)
          } }
          />
      )
    }

    const contentsEl = this.state.contents.map((content: models.Content) => {
      const node = utils.find(this.state.nodes, (n: {
        nodeId: number
        nodeName: string
      }) => {
        return n.nodeId === content.nodeId
      })
      if (!node) return null

      var publishmentSystemId = content.publishmentSystemId
      var nodeId = content.nodeId
      var id = content.id
      var title = content.title
      var addDate = content.addDate
      var isChecked = content.isChecked

      let state = '审核通过'
      let deleteEl = null
      if (!isChecked) {
        state = '待审核'
        deleteEl = <a href="javascript:;" className="lnk" onClick={() => {
          utils.Swal.confirm('此操作将删除稿件，确认吗？', (isConfirm: boolean) => {
            if (isConfirm) {
              client.writing.deleteContent(publishmentSystemId, nodeId, id, (err: models.Error, res: {}) => {
                this.state.contents.splice(this.state.contents.indexOf(content), 1)
                this.setState(this.state)
              })
            }
          })
        } }>删除</a>
      }

      return (
        <tr key={id}>
          <td>{title}</td>
          <td>{node.nodeName}</td>
          <td>{addDate}</td>
          <td style={{ color: isChecked ? '#4CAF50' : '#f00' }}>【{state}】</td>
          <td>
            <Link to={`/writing/edit/${publishmentSystemId}/${nodeId}/${id}`} className="lnk">编辑</Link>
            &nbsp;&nbsp;&nbsp;&nbsp;
            {deleteEl}
          </td>
        </tr>
      )
    })

    //https://codyhouse.co/demo/pagination/index.html
    let pagerEl = null
    if (this.state.totalPage > 1) {
      const prevEl = this.state.currentPage > 1 ? (
        <li className="button"><a href="javascript:;" onClick={this.onPageClick.bind(this, this.state.currentPage - 1)}>Prev</a></li>
      ) : (
        <li className="button disabled"><a href="javascript:;">Prev</a></li>
      )
      const nextEl = this.state.currentPage !== this.state.totalPage ? (
        <li className="button"><a href="javascript:;" onClick={this.onPageClick.bind(this, this.state.currentPage + 1)}>Next</a></li>
      ) :  (
        <li className="button disabled"><a href="javascript:;">Next</a></li>
      )

      const n = 4
      const keys: Array<number> = []
      if (this.state.totalPage > 10) {
        for (let i = 1; i <= n; i++) {
          if (keys.indexOf(i) === -1) keys.push(i)
        }
        if (this.state.currentPage < (this.state.totalPage - 5)) {
          if (this.state.currentPage > n + 1) {
            if (this.state.currentPage > n + 3) {
              keys.push(-1)
            }
            for (let i = this.state.currentPage - 2; i < this.state.currentPage; i++) {
              if (keys.indexOf(i) === -1) keys.push(i)
            }
          }
          for (let i = this.state.currentPage; i <= this.state.currentPage + 2; i++) {
            if (keys.indexOf(i) === -1) keys.push(i)
          }
          keys.push(-2)
          for (let i = this.state.totalPage - 1; i <= this.state.totalPage; i++) {
            if (keys.indexOf(i) === -1) keys.push(i)
          }
        } else {
          keys.push(-3)
          for (let i = this.state.currentPage - 2; i < this.state.currentPage; i++) {
            if (keys.indexOf(i) === -1) keys.push(i)
          }
          for (let i = this.state.currentPage; i <= this.state.totalPage; i++) {
            if (keys.indexOf(i) === -1) keys.push(i)
          }
        }
      } else {
        for (let i = 1; i <= this.state.totalPage; i++) {
          if (keys.indexOf(i) === -1) keys.push(i)
        }
      }

      const pagesEl = keys.map(i => {
        if (i > 0) {
          return <li key={i}><a className={cx({'current': this.state.currentPage === i})} onClick={this.onPageClick.bind(this, i)} href="javascript:;">{i}</a></li>
        } else {
          return <li key={i}><span>...</span></li>
        }
      })

      pagerEl = (
        <nav className="navigation">
          <ul className="cd-pagination custom-buttons">
            {prevEl}
            {pagesEl}
            {nextEl}
          </ul>
        </nav>
      )
    }

    return (
      <div className="article">
        <div className="mod-3">
          <div className="art-mod">
            <div className="art-hd clearfix"><h2>稿件管理</h2></div>
            <div className="form form-1" style={{ marginTop: 25 }}>
              <ul>
                <li className="fm-item">
                  <label htmlFor="#" className="k">投稿站点：</label>
                  <span className="v" style={{ width: 365 }}>
                    {publishmentSystemsEl}
                  </span>
                </li>
                <li className="fm-item">
                  <label htmlFor="#" className="k">投稿栏目：</label>
                  <span className="v" style={{ width: 365 }}>
                    {nodesEl}
                  </span>
                </li>
                <li className="fm-item">
                  <hr className="hr" />
                </li>
              </ul>
            </div>
            <div className="mod-reslut-t2">
              <table className="table2" width="100%">
                <thead>
                  <tr>
                    <th>内容标题</th>
                    <th>栏目</th>
                    <th>投稿时间</th>
                    <th>状态</th>
                    <th>操作</th>
                  </tr>
                </thead>
                <tbody>
                  {contentsEl}
                </tbody>
              </table>
              {pagerEl}
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
    account: state.account
  };
}

export default connect(mapStateToProps, null)(IndexPage);
