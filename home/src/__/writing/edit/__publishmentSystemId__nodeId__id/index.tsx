import * as React from 'react'
import { IndexLink, Link } from 'react-router'
import { bindActionCreators } from 'redux'
import { connect } from 'react-redux'
import * as models from '../../../../lib/models'
import { Loading } from '../../../../components'
import { Attributes } from '../../components'
import client from '../../../../lib/client'
import * as utils from '../../../../lib/utils'

interface P {
  config: models.Config
  account: models.Account
  params: {
    publishmentSystemId: string
    nodeId: string
    id: string
  }
}

interface S {
  content: models.Content
}

class IndexPage extends React.Component<P, S> {
  constructor(props: P) {
    super(props)
    this.state = {
      content: null
    }
  }

  componentDidMount() {
    client.writing.getContent(utils.parseInt(this.props.params.publishmentSystemId), utils.parseInt(this.props.params.nodeId), utils.parseInt(this.props.params.id), (err: models.Error, res: {
      content: models.Content
    }) => {
      this.state.content = res.content
      this.setState(this.state)
    })
  }

  render() {
    if (!this.state.content) return <Loading />

    return (
      <div className="article">
        <div className="mod-3">
          <div className="art-mod">
            <div className="art-hd clearfix"><h2>编辑稿件</h2></div>
            <Attributes publishmentSystemId={utils.parseInt(this.props.params.publishmentSystemId)} nodeId={utils.parseInt(this.props.params.nodeId)} id={utils.parseInt(this.props.params.id)} content={this.state.content} />
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
