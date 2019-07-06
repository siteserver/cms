<template>
  <div>
    <div v-if="pageType !== 'installed'" class="card-box m-3">
      <h4 class="m-l-5">
        SS CMS 安装向导
      </h4>
      <p class="m-b-25 font-13">
        欢迎来到SiteServer CMS 安装向导！只要进行以下几步操作，你就可以开始使用强大且可扩展的CMS系统了。
      </p>
      <ul class="nav nav-pills nav-fill bg-muted m-b-20">
        <li class="nav-item" :class="{active: pageType === 'agreement'}">
          <a class="nav-link" href="javascript:;">许可协议</a>
        </li>
        <li class="nav-item" :class="{active: pageType === 'environment'}">
          <a class="nav-link" href="javascript:;">环境检测</a>
        </li>
        <li class="nav-item" :class="{active: pageType === 'database'}">
          <a class="nav-link" href="javascript:;">数据库设置</a>
        </li>
        <li class="nav-item" :class="{active: pageType === 'settings'}">
          <a class="nav-link" href="javascript:;">安装产品</a>
        </li>
        <li class="nav-item" :class="{active: pageType === 'success'}">
          <a class="nav-link" href="javascript:;">安装完成</a>
        </li>
      </ul>

      <b-alert :show="errorMessage" variant="danger">
        {{ errorMessage }}
      </b-alert>

      <template v-if="pageType === 'agreement'">
        <div class="form-group">
          <label class="col-form-label">
            <a href="https://github.com/siteserver/cms/blob/staging/LICENSE" target="_blank">SiteServer CMS 开源协议（GPL-3.0）</a>
          </label>
        </div>

        <iframe style="border-color:#F5F5F5; border-width:1px;" scrolling="yes" src="agreement.html" height="320" width="100%" />

        <hr>

        <div class="text-center">
          <div class="checkbox checkbox-primary">
            <input id="autoLogin" v-model="isAgreement" type="checkbox">
            <label for="autoLogin">
              我已经阅读并同意此协议
            </label>
          </div>

          <button :disabled="!isAgreement" class="btn btn-primary btn-large btn-custom w-md" type="submit" @click="btnAgreementNextClick">
            下一步
          </button>
        </div>
      </template>
      <template v-else-if="pageType === 'environment'">
        <div class="form-group">
          <label class="col-form-label">
            服务器信息
          </label>
          <small class="form-text text-muted">下表显示当前服务器环境</small>
        </div>

        <div class="panel panel-default">
          <div class="panel-body p-0">
            <div class="table-responsive">
              <table class="tablesaw table table-hover m-b-0 tablesaw-stack">
                <thead>
                  <tr>
                    <th>参数</th>
                    <th>值</th>
                  </tr>
                </thead>
                <tbody>
                  <tr>
                    <td>API 地址</td>
                    <td>
                      {{ environment.apiUrl }}
                    </td>
                  </tr>
                  <tr>
                    <td>SS CMS 版本</td>
                    <td>
                      {{ environment.productVersion }}
                    </td>
                  </tr>
                  <tr>
                    <td>系统根目录</td>
                    <td>
                      {{ environment.contentRootPath }}
                    </td>
                  </tr>
                  <tr>
                    <td>Web 根目录</td>
                    <td>
                      {{ environment.webRootPath }}
                    </td>
                  </tr>
                  <tr>
                    <td>运行环境</td>
                    <td>
                      {{ environment.targetFramework }}
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>

        <div class="form-group">
          <label class="col-form-label">
            目录权限检测
          </label>
          <small class="form-text text-muted">
            系统要求必须满足下列所有的目录权限全部可读写的需求才能使用，如果没有相关权限请添加。
          </small>
        </div>

        <div class="panel panel-default">
          <div class="panel-body p-0">
            <div class="table-responsive">
              <table class="tablesaw table table-hover m-b-0 tablesaw-stack">
                <thead>
                  <tr>
                    <th>目录名</th>
                    <th>读写权限</th>
                  </tr>
                </thead>
                <tbody>
                  <tr>
                    <td>
                      {{ environment.contentRootPath }}
                    </td>
                    <td>
                      <font v-if="environment.isContentRootPathWritable" color="green">
                        [√]
                      </font>
                      <font v-else color="red">
                        [×]
                      </font>
                    </td>
                  </tr>
                  <tr>
                    <td>
                      {{ environment.webRootPath }}
                    </td>
                    <td>
                      <font v-if="environment.isWebRootPathWritable" color="green">
                        [√]
                      </font>
                      <font v-else color="red">
                        [×]
                      </font>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>

        <hr>

        <div class="text-center">
          <button class="btn btn-default btn-large btn-custom w-md mr-2" type="submit" @click="btnEnvironmentPrevClick">
            上一步
          </button>
          <button :disabled="!environment.isContentRootPathWritable || !environment.isWebRootPathWritable" class="btn btn-primary btn-large btn-custom w-md" type="submit" @click="btnEnvironmentNextClick">
            下一步
          </button>
        </div>
      </template>
      <template v-if="pageType === 'database'">
        <form class="form-horizontal" @submit="btnDatabaseNextClick">
          <div class="form-group">
            <label class="col-form-label">
              数据库类型
              <small v-show="errors.has('databaseType')" class="text-danger">
                {{ errors.first('databaseType') }}
              </small>
            </label>
            <select
              v-model="databaseType"
              v-validate="'required'"
              name="databaseType"
              data-vv-as="数据库类型"
              class="form-control"
              :class="{'is-invalid': errors.has('databaseType') }"
            >
              <option value="MySql">
                MySQL
              </option>
              <option value="SqlServer">
                SQL Server
              </option>
              <option value="PostgreSql">
                PostgreSQL
              </option>
              <option value="SQLite">
                SQLite
              </option>
              <option value="Oracle">
                Oracle
              </option>
            </select>
            <small class="form-text text-muted">
              请选择需要安装的数据库类型。
            </small>
          </div>

          <template v-if="databaseType && databaseType !== 'SQLite'">
            <div class="form-group">
              <label class="col-form-label">
                数据库主机
                <small v-show="errors.has('server')" class="text-danger">
                  {{ errors.first('server') }}
                </small>
              </label>
              <input
                v-model="server"
                v-validate="'required'"
                name="server"
                type="text"
                data-vv-as="数据库主机"
                :class="{'is-invalid': errors.has('server') }"
                class="form-control"
              >
              <small class="form-text text-muted">
                IP地址或者服务器名
              </small>
            </div>

            <div class="form-group">
              <label class="col-form-label">
                数据库端口
                <small v-show="errors.has('isDefaultPort')" class="text-danger">
                  {{ errors.first('isDefaultPort') }}
                </small>
              </label>
              <select
                v-model="isDefaultPort"
                v-validate="'required'"
                name="isDefaultPort"
                data-vv-as="数据库端口"
                class="form-control"
                :class="{'is-invalid': errors.has('isDefaultPort') }"
              >
                <option :value="true">
                  默认数据库端口
                </option>
                <option :value="false">
                  自定义数据库端口
                </option>
              </select>
            </div>

            <div v-if="!isDefaultPort" class="form-group">
              <label class="col-form-label">
                自定义端口
                <small v-show="errors.has('port')" class="text-danger">
                  {{ errors.first('port') }}
                </small>
              </label>
              <input
                v-model="port"
                v-validate="'required|numeric'"
                name="port"
                type="text"
                data-vv-as="自定义端口"
                :class="{'is-invalid': errors.has('port') }"
                class="form-control"
              >
              <small class="form-text text-muted">
                连接数据库的端口
              </small>
            </div>

            <div class="form-group">
              <label class="col-form-label">
                数据库用户名
                <small v-show="errors.has('uid')" class="text-danger">
                  {{ errors.first('uid') }}
                </small>
              </label>
              <input
                v-model="uid"
                v-validate="'required'"
                name="uid"
                type="text"
                data-vv-as="数据库用户名"
                :class="{'is-invalid': errors.has('uid') }"
                class="form-control"
              >
              <small class="form-text text-muted">
                连接数据库的用户名
              </small>
            </div>

            <div class="form-group">
              <label class="col-form-label">
                数据库密码
                <small v-show="errors.has('pwd')" class="text-danger">
                  {{ errors.first('pwd') }}
                </small>
              </label>
              <input
                v-model="pwd"
                v-validate="'required'"
                name="pwd"
                type="password"
                data-vv-as="数据库密码"
                :class="{'is-invalid': errors.has('pwd') }"
                class="form-control"
              >
              <small class="form-text text-muted">
                连接数据库的密码
              </small>
            </div>

            <div v-if="databaseType === 'Oracle'" class="form-group">
              <label class="col-form-label">
                数据库名称
                <small v-show="errors.has('databaseName')" class="text-danger">
                  {{ errors.first('databaseName') }}
                </small>
              </label>
              <input
                v-model="databaseName"
                v-validate="'required'"
                name="databaseName"
                type="text"
                data-vv-as="数据库名称"
                :class="{'is-invalid': errors.has('databaseName') }"
                class="form-control"
              >
              <small class="form-text text-muted">
                指定需要安装的Oracle数据库名称
              </small>
            </div>
          </template>

          <hr>

          <div class="text-center">
            <button class="btn btn-default btn-large btn-custom w-md mr-2" type="submit" @click="btnDatabasePrevClick">
              上一步
            </button>
            <button class="btn btn-primary btn-large btn-custom w-md" type="submit" @click="btnDatabaseNextClick">
              下一步
            </button>
          </div>
        </form>
      </template>
      <template v-else-if="pageType === 'settings'">
        <form class="form-horizontal" @submit="btnSettingsNextClick">
          <div class="form-group">
            <label class="col-form-label">
              超级管理员用户名
              <small v-show="errors.has('adminName')" class="text-danger">
                {{ errors.first('adminName') }}
              </small>
            </label>
            <input
              v-model="adminName"
              v-validate="'required'"
              name="adminName"
              type="text"
              data-vv-as="超级管理员用户名"
              :class="{'is-invalid': errors.has('adminName') }"
              class="form-control"
            >
            <small class="form-text text-muted">
              在此设置超级管理员的登录用户名
            </small>
          </div>

          <div class="form-group">
            <label class="col-form-label">
              超级管理员密码
              <small v-show="errors.has('adminPassword')" class="text-danger">
                {{ errors.first('adminPassword') }}
              </small>
            </label>
            <input
              ref="adminPassword"
              v-model="adminPassword"
              v-validate="'required|min:6'"
              name="adminPassword"
              type="password"
              data-vv-as="超级管理员密码"
              :class="{'is-invalid': errors.has('adminPassword') }"
              class="form-control"
            >
            <small class="form-text text-muted">
              6-16个字符，支持大小写字母、数字和符号
            </small>

            <small v-if="passwordScore > 80" class="form-text text-primary">
              密码强度：极好
            </small>
            <small v-else-if="passwordScore > 60" class="form-text text-primary">
              密码强度：强
            </small>
            <small v-else-if="passwordScore > 0" class="form-text text-danger">
              密码强度：弱
            </small>
          </div>

          <div class="form-group">
            <label class="col-form-label">
              确认密码
              <small v-show="errors.has('adminPasswordConfirm')" class="text-danger">
                {{ errors.first('adminPasswordConfirm') }}
              </small>
            </label>
            <input
              v-model="adminPasswordConfirm"
              v-validate="'required|confirmed:adminPassword'"
              name="adminPasswordConfirm"
              type="password"
              data-vv-as="确认密码"
              :class="{'is-invalid': errors.has('adminPasswordConfirm') }"
              class="form-control"
            >
            <small class="form-text text-muted">
              6-16个字符，支持大小写字母、数字和符号
            </small>
          </div>

          <h4 class="m-t-20 m-b-20">
            安装选项
          </h4>
          <hr>

          <div class="form-group">
            <label class="col-form-label">
              默认语言
              <small v-show="errors.has('language')" class="text-danger">
                {{ errors.first('language') }}
              </small>
            </label>
            <select
              v-model="language"
              v-validate="'required'"
              name="language"
              data-vv-as="默认语言"
              class="form-control"
              :class="{'is-invalid': errors.has('language') }"
            >
              <option value="en">
                English
              </option>
              <option value="zh-CN">
                简体中文
              </option>
            </select>
            <small class="form-text text-muted">
              Redis缓存能够实现API分离部署
            </small>
          </div>

          <div class="form-group">
            <label class="col-form-label">
              是否启用Redis缓存
              <small v-show="errors.has('redisIsEnabled')" class="text-danger">
                {{ errors.first('redisIsEnabled') }}
              </small>
            </label>
            <select
              v-model="redisIsEnabled"
              v-validate="'required'"
              name="redisIsEnabled"
              data-vv-as="是否启用Redis缓存"
              class="form-control"
              :class="{'is-invalid': errors.has('redisIsEnabled') }"
            >
              <option :value="true">
                启用Redis缓存
              </option>
              <option :value="false">
                不启用Redis缓存
              </option>
            </select>
            <small class="form-text text-muted">
              Redis缓存能够实现API分离部署
            </small>
          </div>

          <div v-if="redisIsEnabled" class="form-group">
            <label class="col-form-label">
              Redis连接字符串
              <small v-show="errors.has('redisConnectionString')" class="text-danger">
                {{ errors.first('redisConnectionString') }}
              </small>
            </label>
            <input
              v-model="redisConnectionString"
              v-validate="'required'"
              name="redisConnectionString"
              type="text"
              data-vv-as="Redis连接字符串"
              :class="{'is-invalid': errors.has('redisConnectionString') }"
              class="form-control"
            >
            <small class="form-text text-muted">
              设置Redis连接字符串
            </small>
          </div>

          <div class="form-group">
            <label class="col-form-label">
              是否加密连接字符串
              <small v-show="errors.has('isProtectData')" class="text-danger">
                {{ errors.first('isProtectData') }}
              </small>
            </label>
            <select
              v-model="isProtectData"
              v-validate="'required'"
              name="isProtectData"
              data-vv-as="是否加密连接字符串"
              class="form-control"
              :class="{'is-invalid': errors.has('isProtectData') }"
            >
              <option :value="true">
                加密连接字符串
              </option>
              <option :value="false">
                不加密连接字符串
              </option>
            </select>
            <small class="form-text text-muted">
              设置是否加密 appsettings.json 中的数据库连接字符串
            </small>
          </div>

          <hr>

          <div class="text-center">
            <button class="btn btn-default btn-large btn-custom w-md mr-2" type="submit" @click="btnSettingsPrevClick">
              上一步
            </button>
            <button class="btn btn-primary btn-large btn-custom w-md" type="submit" @click="btnSettingsNextClick">
              下一步
            </button>
          </div>
        </form>
      </template>
      <template v-else-if="pageType === 'success'">
        <div class="alert alert-success">
          <h4 class="alert-heading">
            安装完成！
          </h4>
          <p>
            恭喜，您已经完成了 SS CMS 的安装
            <n-link to="../">
              进入管理后台
            </n-link>
          </p>
          <hr>
          <p class="mb-0">
            获取更多使用帮助请访问
            <a href="https://www.sscms.com/docs/" target="_blank">SS CMS 文档中心</a>
          </p>
        </div>
      </template>
    </div>

    <div v-if="pageType === 'installed'">
      <div class="alert alert-danger m-3">
        <h4 class="alert-heading">
          系统已安装，向导被禁用
        </h4>
        <hr>
        <p class="mb-0">
          获取更多使用帮助请访问
          <a href="https://www.sscms.com/docs/" target="_blank">SS CMS 文档中心</a>
        </p>
      </div>
    </div>
  </div>
</template>

<script>
import _ from 'lodash'

export default {
  $_veeValidate: {
    validator: 'new' // give me my own validator scope.
  },
  data() {
    return {
      pageType: 'agreement',
      isAgreement: true,
      environment: {},
      databaseType: null,
      server: null,
      isDefaultPort: true,
      port: null,
      uid: null,
      pwd: null,
      databaseName: null,
      adminUrl: null,
      homeUrl: null,
      language: null,
      isNightlyUpdate: null,
      isProtectData: null,
      redisIsEnabled: null,
      redisConnectionString: null,
      adminName: null,
      adminPassword: null,
      adminPasswordConfirm: null,
      errorMessage: null
    }
  },
  computed: {
    passwordScore() {
      return this.scorePassword(this.adminPassword)
    }
  },
  mounted() {
    this.apiGetInstall()
  },
  methods: {
    async apiGetInstall() {
      this.$store.commit('loading', true)
      this.environment = await this.$api.install.get()
      if (this.environment.isInstalled) {
        this.pageType = 'installed'
      }
      this.$store.commit('loading', false)
    },

    btnAgreementNextClick() {
      if (this.isAgreement) {
        this.pageType = 'environment'
      }
    },

    btnEnvironmentPrevClick() {
      this.pageType = 'agreement'
    },

    btnEnvironmentNextClick() {
      if (this.environment.isContentRootPathWritable && this.environment.isWebRootPathWritable) {
        this.pageType = 'database'
      }
    },

    async apiTryDatabase() {
      this.$store.commit('loading', true)
      const data = await this.$api.install.postAt('actions/tryDatabase', {
        databaseType: this.databaseType,
        server: this.server,
        port: this.isDefaultPort ? null : this.port,
        uid: this.uid,
        pwd: this.pwd,
        databaseName: this.databaseName
      })
      this.$store.commit('loading', false)

      if (data.isSuccess) {
        this.pageType = 'settings'
      } else {
        this.errorMessage = data.errorMessage
      }
    },

    async apiTryRedis() {
      this.$store.commit('loading', true)
      const data = await this.$api.install.postAt('actions/tryRedis', {
        redisConnectionString: this.redisConnectionString
      })
      this.$store.commit('loading', false)

      if (data.isSuccess) {
        this.apiSaveSettings()
      } else {
        this.errorMessage = data.errorMessage
      }
    },

    async apiSaveSettings() {
      this.$store.commit('loading', true)
      const adminUrl = _.trimEnd(location.href.toLowerCase(), '/').replace('/install/database', '')
      const homeUrl = adminUrl.substring(0, adminUrl.lastIndexOf('/')) + '/home'

      const data = await this.$api.install.postAt('actions/saveSettings', {
        databaseType: this.databaseType,
        server: this.server,
        port: this.isDefaultPort ? null : this.port,
        uid: this.uid,
        pwd: this.pwd,
        databaseName: this.databaseName,
        adminUrl: adminUrl,
        homeUrl: homeUrl,
        language: this.language,
        isNightlyUpdate: this.isNightlyUpdate,
        isProtectData: this.isProtectData,
        redisIsEnabled: this.redisIsEnabled,
        redisConnectionString: this.redisConnectionString,
        adminName: this.adminName,
        adminPassword: this.adminPassword
      })
      this.$store.commit('loading', false)

      if (data.isSuccess) {
        this.pageType = 'success'
      } else {
        this.errorMessage = data.errorMessage
      }
    },

    scorePassword(pass) {
      let score = 0
      if (!pass) return 0

      // award every unique letter until 5 repetitions
      const letters = {}
      for (let i = 0; i < pass.length; i++) {
        letters[pass[i]] = (letters[pass[i]] || 0) + 1
        score += 5.0 / letters[pass[i]]
      }

      // bonus points for mixing it up
      const variations = {
        digits: /\d/.test(pass),
        lower: /[a-z]/.test(pass),
        upper: /[A-Z]/.test(pass),
        nonWords: /\W/.test(pass)
      }

      let variationCount = 0
      for (const check in variations) {
        variationCount += variations[check] ? 1 : 0
      }
      score += (variationCount - 1) * 10

      return score
    },

    btnDatabasePrevClick(e) {
      e.preventDefault()
      this.pageType = 'environment'
    },

    btnDatabaseNextClick(e) {
      e.preventDefault()
      this.$validator.validateAll().then((result) => {
        if (result) {
          this.apiTryDatabase()
        }
      })
    },

    btnSettingsPrevClick(e) {
      e.preventDefault()
      this.pageType = 'database'
    },

    btnSettingsNextClick(e) {
      e.preventDefault()
      this.$validator.validateAll().then((result) => {
        if (result) {
          if (this.redisIsEnabled) {
            this.apiTryRedis()
          } else {
            this.apiSaveSettings()
          }
        }
      })
    }
  }
}
</script>
