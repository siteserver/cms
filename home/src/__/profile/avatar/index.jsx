import * as React from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import Upload from 'rc-upload';
import { update } from '../../../lib/actions/account';
import { Loading } from '../../../components';
import * as utils from '../../../lib/utils';
import client from '../../../lib/client';
require("../../../imgareaselect-default.css");
class AvatarPage extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            loading: false,
            avatarUrl: null,
            relatedUrl: null,
            imgSize: null
        };
    }
    componentWillUnmount() {
        $('.imgareaselect-outer, .imgareaselect-border1, .imgareaselect-border2, .imgareaselect-border3, .imgareaselect-border4, .imgareaselect-handle').hide();
    }
    loading(loading) {
        this.state.loading = loading;
        this.setState(this.state);
    }
    resizeImgSubmit() {
        if (!this.state.imgSize || !this.state.imgSize.width || !this.state.imgSize.height) {
            return utils.Swal.warning('上传头像前请设置图片区域');
        }
        this.loading(true);
        const actualWidth = $('#preCropper').width();
        const actualHeight = $('#preCropper').height();
        const fullWidth = $('#preCropper_hidden').width();
        const fullHeight = $('#preCropper_hidden').height();
        let size = this.state.imgSize.width;
        let x = this.state.imgSize.x1;
        let y = this.state.imgSize.y1;
        if (fullWidth > actualWidth || fullHeight > actualHeight) {
            const f = fullWidth / actualWidth;
            size = _.round(size * f);
            x = _.round(x * f);
            y = _.round(y * f);
        }
        $('.imgareaselect-outer, .imgareaselect-border1, .imgareaselect-border2, .imgareaselect-border3, .imgareaselect-border4, .imgareaselect-handle').hide();
        client.files.uploadAvatarResize(size, x, y, this.state.relatedUrl, (err, res) => {
            const data = { avatarUrl: res.avatarUrl };
            client.users.edit(data, (err, res) => {
                this.loading(false);
                if (!err) {
                    this.props.account.user.avatarUrl = res.avatarUrl;
                    this.props.update(this.props.account);
                    utils.Swal.success('头像修改成功', () => {
                        location.reload();
                    });
                }
                else {
                    utils.Swal.error(err);
                }
            });
        });
    }
    render() {
        const user = this.props.account.user;
        const avatarUrl = user.avatarUrl;
        const url = client.files.getUploadAvatarUrl();
        const uploadProps = utils.getUploadAvatarProps(url, (avatarUrl, relatedUrl) => {
            this.state.loading = false;
            this.state.avatarUrl = avatarUrl;
            this.state.relatedUrl = relatedUrl;
            this.setState(this.state);
        }, (errorMessage) => {
            this.state.loading = false;
            this.setState(this.state);
            utils.Swal.warning(errorMessage);
        }, () => {
            this.state.loading = true;
            this.setState(this.state);
        });
        let mainEl = null;
        if (this.state.avatarUrl && this.state.relatedUrl) {
            mainEl = (<div className="mod-setavator">
            <div className="area1">
                {this.state.loading && <Loading />}
                <div className="tit1">
                    <span>请选择图片区域后点击确认按钮上传头像</span>
                </div>
                <div className="center" style={{ marginTop: 20, marginBottom: 10 }}>
                  <div style={{ position: 'relative', marginLeft: '10px', marginRight: '10px' }}>
                    <img src={this.state.avatarUrl} style={{ maxWidth: '100%' }} id='preCropper'/>
                    <img src={this.state.avatarUrl} style={{ display: 'none' }} id='preCropper_hidden'/>
                  </div>
                  <div className="btns"><span className="btn btn-2 modify-basic-info-btn" onClick={this.resizeImgSubmit.bind(this)}>确认提交</span></div>
                </div>
            </div>
        </div>);
            setTimeout(() => {
                $('#preCropper').imgAreaSelect({
                    handles: true,
                    aspectRatio: '1:1',
                    fadeSpeed: 200,
                    onSelectEnd: (img, s) => {
                        this.state.imgSize = s;
                        this.setState(this.state);
                    }
                });
            }, 100);
        }
        else {
            mainEl = (<div className="mod-setavator">
            <div className="area1">
                {this.state.loading && <Loading />}
                <div className="photo clearfix" style={{ display: this.state.loading ? 'none' : 'block' }}>
                    <div className="photol">
                        <img id="headimg" src={avatarUrl} width={180} height={180}/>
                    </div>
                    <div className="photor">
                        <Upload {...uploadProps}>
                          <p className="iptp">
                            <input name="fileA" id="fileIdA" type="text" className="ipttext ip-1"/>
                            <input type="button" defaultValue="浏览" className="iptbut"/>
                          </p>
                        </Upload>
                        <p className="text-tip">支持JPG、PNG、GIF图片类型，不能大于3M。<br /></p>
                    </div>
                </div>
            </div>
        </div>);
        }
        return (<div className="article">
        <div className="mod-3">
          <div className="art-mod">
            <div className="art-hd clearfix"><h2>我的头像</h2></div>
            <div className="art-bd">

            {mainEl}

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
function mapDispatchToProps(dispatch) {
    return {
        update: bindActionCreators(update, dispatch),
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(AvatarPage);
//# sourceMappingURL=index.jsx.map