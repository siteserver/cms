import * as React from 'react'
import { IndexLink, Link, History } from 'react-router'
import cx from 'classnames'
import Upload from 'rc-upload'
import * as utils from '../../../lib/utils'
import client from '../../../lib/client'

interface P {
  publishmentSystemId: number
  imageUrls?: Array<string>
  onChange?(imageUrls: Array<string>):any;
}

export default class ImageUpload extends React.Component<P, {
  imageUrls: Array<string>
  loading: boolean
}> {
  constructor(props: P) {
    super(props)
    this.state = {
      imageUrls: props.imageUrls || [],
      loading: false
    }
  }

  render() {
    const url = client.files.getUploadSiteFilesUrl(this.props.publishmentSystemId, 'image')
    const uploadProps = utils.getUploadFilesProps(url, false, "image/png, image/jpeg, image/gif", (fileUrls: Array<string>) => {
      this.state.loading = false
      this.state.imageUrls.push(...fileUrls)
      this.setState(this.state)
      this.props.onChange(this.state.imageUrls)
    }, (errorMessage: string) => {
      this.state.loading = false
      this.setState(this.state)
      utils.Swal.warning(errorMessage)
    }, () => {
      this.state.loading = true
      this.setState(this.state)
    })

    return (
      <div>
        {this.state.imageUrls.map((imageUrl: string) => {
          return (
            <span key={imageUrl} className="mup_file">
              <a href={imageUrl} className="mup_fnm" target="_blank">{utils.getFileName(imageUrl)}</a>
              <i className="mup_del" onClick={() => {
                this.state.imageUrls.splice(this.state.imageUrls.indexOf(imageUrl), 1)
                this.setState(this.state)
              }}></i>
            </span>
          )
        })}
        <span style={{display: this.state.loading ? '' : 'none'}}>图片上传中，请稍后...</span>
        <Upload {...uploadProps}>
          <span style={{display: this.state.loading ? 'none' : ''}} href="javascript:;" className="btn btn-5">上传图片</span>
        </Upload>
      </div>
    );
  }
}
