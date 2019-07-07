<template>
  <div class="install-container">

    <el-alert
      v-if="environment.isInstalled"
      :title="$t('install.forbid')"
      type="error"
      effect="dark"
      :closable="false"
    />
    <el-card v-else v-loading="loading" class="box-card">
      <div slot="header" class="clearfix">
        <span>SS CMS {{ $t('install.title') }}</span>
        <lang-select style="float: right; padding: 3px 0" class="set-language" />
      </div>
      <div class="text item">

        <el-steps :active="active" finish-status="success">
          <el-step :title="$t('install.environment')" />
          <el-step :title="$t('install.database')" />
          <el-step :title="$t('install.cache')" />
          <el-step :title="$t('install.settings')" />
          <el-step :title="$t('install.installation')" />
        </el-steps>

        <el-alert
          v-if="errorMessage"
          :title="errorMessage"
          type="error"
          effect="dark"
          :closable="false"
        />

        <hr>

        <template v-if="active === 0">
          <div class="panel panel-default">
            <div class="panel-body p-0">
              <div class="table-responsive">
                <table class="tablesaw table table-hover m-b-0 tablesaw-stack">
                  <thead>
                    <tr>
                      <th>{{ $t('name') }}</th>
                      <th>{{ $t('value') }}</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr>
                      <td>{{ $t('tools.info.apiUrl') }}</td>
                      <td>
                        {{ environment.apiUrl }}
                      </td>
                    </tr>
                    <tr>
                      <td>{{ $t('tools.info.productVersion') }}</td>
                      <td>
                        {{ environment.productVersion }}
                      </td>
                    </tr>
                    <tr>
                      <td>{{ $t('tools.info.contentRootPath') }}</td>
                      <td>
                        {{ environment.contentRootPath }}
                      </td>
                    </tr>
                    <tr>
                      <td>{{ $t('tools.info.webRootPath') }}</td>
                      <td>
                        {{ environment.webRootPath }}
                      </td>
                    </tr>
                    <tr>
                      <td>{{ $t('tools.info.targetFramework') }}</td>
                      <td>
                        {{ environment.targetFramework }}
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>
          </div>
        </template>

        <template v-else-if="active === 1">
          <form class="form-horizontal" @submit="btnNextClick">
            <div class="form-group">
              <label class="col-form-label">
                {{ $t('install.databaseType') }}
                <small v-show="errors.has('databaseType')" class="text-danger">
                  {{ errors.first('databaseType') }}
                </small>
              </label>
              <select
                key="databaseType"
                v-model="databaseType"
                v-validate="'required'"
                name="databaseType"
                :data-vv-as="$t('install.databaseType')"
                class="form-control"
                :class="{'is-invalid': errors.has('databaseType') }"
              >
                <option value="MySql">
                  MySQL 8+
                </option>
                <option value="SqlServer">
                  SQL Server 2012+
                </option>
                <option value="PostgreSql">
                  PostgreSQL
                </option>
                <option value="SQLite">
                  SQLite
                </option>
                <option value="Oracle">
                  Oracle 12c+
                </option>
              </select>
              <small class="form-text text-muted">
                {{ $t('install.databaseTypeTips') }}
              </small>
            </div>

            <template v-if="databaseType && databaseType !== 'SQLite'">

              <div class="form-group">
                <el-switch
                  v-model="databaseIsConnectionString"
                  :active-text="$t('install.setConnectionString')"
                />
              </div>

              <template v-if="databaseIsConnectionString">
                <div class="form-group">
                  <label class="col-form-label">
                    {{ $t('install.connectionString') }}
                    <small v-show="errors.has('databaseConnectionString')" class="text-danger">
                      {{ errors.first('databaseConnectionString') }}
                    </small>
                  </label>
                  <input
                    key="databaseConnectionString"
                    v-model="databaseConnectionString"
                    v-validate="'required'"
                    name="databaseConnectionString"
                    type="text"
                    :data-vv-as="$t('install.connectionString')"
                    :class="{'is-invalid': errors.has('databaseConnectionString') }"
                    class="form-control"
                  >
                </div>
              </template>
              <template v-else>
                <div class="form-group">
                  <label class="col-form-label">
                    {{ $t('install.server') }}
                    <small v-show="errors.has('databaseServer')" class="text-danger">
                      {{ errors.first('databaseServer') }}
                    </small>
                  </label>
                  <input
                    key="databaseServer"
                    v-model="databaseServer"
                    v-validate="'required'"
                    name="databaseServer"
                    type="text"
                    :data-vv-as="$t('install.server')"
                    :class="{'is-invalid': errors.has('databaseServer') }"
                    class="form-control"
                  >
                </div>

                <div class="form-group">
                  <label class="col-form-label">
                    {{ $t('install.port') }}
                    <small v-show="errors.has('databaseIsDefaultPort')" class="text-danger">
                      {{ errors.first('databaseIsDefaultPort') }}
                    </small>
                  </label>
                  <select
                    key="databaseIsDefaultPort"
                    v-model="databaseIsDefaultPort"
                    v-validate="'required'"
                    name="databaseIsDefaultPort"
                    :data-vv-as="$t('install.port')"
                    class="form-control"
                    :class="{'is-invalid': errors.has('databaseIsDefaultPort') }"
                  >
                    <option :value="true">
                      {{ $t('install.defaultPort') }}
                    </option>
                    <option :value="false">
                      {{ $t('install.customPort') }}
                    </option>
                  </select>
                </div>

                <div v-if="!databaseIsDefaultPort" class="form-group">
                  <label class="col-form-label">
                    {{ $t('install.customPort') }}
                    <small v-show="errors.has('databasePort')" class="text-danger">
                      {{ errors.first('databasePort') }}
                    </small>
                  </label>
                  <input
                    key="databasePort"
                    v-model="databasePort"
                    v-validate="'required|numeric'"
                    name="databasePort"
                    type="text"
                    :data-vv-as="$t('install.customPort')"
                    :class="{'is-invalid': errors.has('databasePort') }"
                    class="form-control"
                  >
                </div>

                <div class="form-group">
                  <label class="col-form-label">
                    {{ $t('install.databaseUid') }}
                    <small v-show="errors.has('databaseUid')" class="text-danger">
                      {{ errors.first('databaseUid') }}
                    </small>
                  </label>
                  <input
                    key="databaseUid"
                    v-model="databaseUid"
                    v-validate="'required'"
                    name="databaseUid"
                    type="text"
                    :data-vv-as="$t('install.databaseUid')"
                    :class="{'is-invalid': errors.has('databaseUid') }"
                    autocomplete="false"
                    class="form-control"
                  >
                </div>

                <div class="form-group">
                  <label class="col-form-label">
                    {{ $t('install.databasePwd') }}
                    <small v-show="errors.has('databasePwd')" class="text-danger">
                      {{ errors.first('databasePwd') }}
                    </small>
                  </label>
                  <input
                    key="databasePwd"
                    v-model="databasePwd"
                    v-validate="'required'"
                    name="databasePwd"
                    type="password"
                    :data-vv-as="$t('install.databasePwd')"
                    :class="{'is-invalid': errors.has('databasePwd') }"
                    autocomplete="false"
                    class="form-control"
                  >
                </div>
                <div v-if="databaseNames" class="form-group">
                  <label class="col-form-label">
                    {{ $t('install.database') }}
                    <small v-show="errors.has('databaseName')" class="text-danger">
                      {{ errors.first('databaseName') }}
                    </small>
                  </label>
                  <select
                    key="databaseName"
                    v-model="databaseName"
                    v-validate="'required'"
                    name="databaseName"
                    :data-vv-as="$t('install.database')"
                    class="form-control"
                    :class="{'is-invalid': errors.has('databaseName') }"
                  >
                    <option v-for="name in databaseNames" :key="name" :value="name">
                      {{ name }}
                    </option>
                  </select>
                  <small class="form-text text-muted">
                    {{ $t('install.databaseNameTips') }}
                  </small>
                </div>
              </template>
            </template>
          </form>
        </template>

        <template v-else-if="active === 2">
          <form class="form-horizontal" @submit="btnNextClick">
            <div class="form-group">
              <label class="col-form-label">
                {{ $t('install.cacheType') }}
                <small v-show="errors.has('cacheType')" class="text-danger">
                  {{ errors.first('cacheType') }}
                </small>
              </label>
              <select
                key="cacheType"
                v-model="cacheType"
                v-validate="'required'"
                name="cacheType"
                :data-vv-as="$t('install.cacheType')"
                class="form-control"
                :class="{'is-invalid': errors.has('cacheType') }"
              >
                <option value="Memory">
                  Memory
                </option>
                <option value="SqlServer">
                  SQL Server
                </option>
                <option value="Redis">
                  Redis
                </option>
              </select>
              <small class="form-text text-muted">
                {{ $t('install.cacheTypeTips') }}
              </small>
            </div>

            <template v-if="cacheType && cacheType !== 'Memory'">
              <div class="form-group">
                <el-switch
                  v-model="cacheIsConnectionString"
                  :active-text="$t('install.setConnectionString')"
                />
              </div>

              <template v-if="cacheIsConnectionString">
                <div class="form-group">
                  <label class="col-form-label">
                    {{ $t('install.connectionString') }}
                    <small v-show="errors.has('cacheConnectionString')" class="text-danger">
                      {{ errors.first('cacheConnectionString') }}
                    </small>
                  </label>
                  <input
                    key="cacheConnectionString"
                    v-model="cacheConnectionString"
                    v-validate="'required'"
                    name="cacheConnectionString"
                    type="text"
                    :data-vv-as="$t('install.connectionString')"
                    :class="{'is-invalid': errors.has('cacheConnectionString') }"
                    class="form-control"
                  >
                </div>
              </template>
              <template v-else-if="cacheType === 'SqlServer'">
                <div class="form-group">
                  <label class="col-form-label">
                    {{ $t('install.server') }}
                    <small v-show="errors.has('cacheServer')" class="text-danger">
                      {{ errors.first('cacheServer') }}
                    </small>
                  </label>
                  <input
                    key="cacheServer"
                    v-model="cacheServer"
                    v-validate="'required'"
                    name="cacheServer"
                    type="text"
                    :data-vv-as="$t('install.server')"
                    :class="{'is-invalid': errors.has('cacheServer') }"
                    class="form-control"
                  >
                </div>

                <div class="form-group">
                  <label class="col-form-label">
                    {{ $t('install.port') }}
                    <small v-show="errors.has('cacheIsDefaultPort')" class="text-danger">
                      {{ errors.first('cacheIsDefaultPort') }}
                    </small>
                  </label>
                  <select
                    key="cacheIsDefaultPort"
                    v-model="cacheIsDefaultPort"
                    v-validate="'required'"
                    name="cacheIsDefaultPort"
                    :data-vv-as="$t('install.port')"
                    class="form-control"
                    :class="{'is-invalid': errors.has('cacheIsDefaultPort') }"
                  >
                    <option :value="true">
                      {{ $t('install.defaultPort') }}
                    </option>
                    <option :value="false">
                      {{ $t('install.customPort') }}
                    </option>
                  </select>
                </div>

                <div v-if="!cacheIsDefaultPort" class="form-group">
                  <label class="col-form-label">
                    {{ $t('install.customPort') }}
                    <small v-show="errors.has('cachePort')" class="text-danger">
                      {{ errors.first('cachePort') }}
                    </small>
                  </label>
                  <input
                    key="cachePort"
                    v-model="cachePort"
                    v-validate="'required|numeric'"
                    name="cachePort"
                    type="text"
                    :data-vv-as="$t('install.customPort')"
                    :class="{'is-invalid': errors.has('cachePort') }"
                    class="form-control"
                  >
                </div>

                <div class="form-group">
                  <label class="col-form-label">
                    {{ $t('install.databaseUid') }}
                    <small v-show="errors.has('cacheUid')" class="text-danger">
                      {{ errors.first('cacheUid') }}
                    </small>
                  </label>
                  <input
                    key="cacheUid"
                    v-model="cacheUid"
                    v-validate="'required'"
                    name="cacheUid"
                    type="text"
                    :data-vv-as="$t('install.databaseUid')"
                    :class="{'is-invalid': errors.has('cacheUid') }"
                    autocomplete="false"
                    class="form-control"
                  >
                </div>

                <div class="form-group">
                  <label class="col-form-label">
                    {{ $t('install.databasePwd') }}
                    <small v-show="errors.has('cachePwd')" class="text-danger">
                      {{ errors.first('cachePwd') }}
                    </small>
                  </label>
                  <input
                    key="cachePwd"
                    v-model="cachePwd"
                    v-validate="'required'"
                    name="cachePwd"
                    type="password"
                    :data-vv-as="$t('install.databasePwd')"
                    :class="{'is-invalid': errors.has('cachePwd') }"
                    autocomplete="false"
                    class="form-control"
                  >
                </div>
                <div v-if="cacheNames" class="form-group">
                  <label class="col-form-label">
                    $t('install.database')
                    <small v-show="errors.has('cacheName')" class="text-danger">
                      {{ errors.first('cacheName') }}
                    </small>
                  </label>
                  <select
                    key="cacheName"
                    v-model="cacheName"
                    v-validate="'required'"
                    name="cacheName"
                    :data-vv-as="$t('install.database')"
                    class="form-control"
                    :class="{'is-invalid': errors.has('cacheName') }"
                  >
                    <option v-for="name in cacheNames" :key="name" :value="name">
                      {{ name }}
                    </option>
                  </select>
                </div>
              </template>
              <template v-else-if="cacheType === 'Redis'">
                <div class="form-group">
                  <label class="col-form-label">
                    {{ $t('install.server') }}
                    <small v-show="errors.has('cacheServer')" class="text-danger">
                      {{ errors.first('cacheServer') }}
                    </small>
                  </label>
                  <input
                    key="cacheServer"
                    v-model="cacheServer"
                    v-validate="'required'"
                    name="cacheServer"
                    type="text"
                    :data-vv-as="$t('install.server')"
                    :class="{'is-invalid': errors.has('cacheServer') }"
                    class="form-control"
                  >
                </div>

                <div class="form-group">
                  <label class="col-form-label">
                    {{ $t('install.port') }}
                    <small v-show="errors.has('cacheIsDefaultPort')" class="text-danger">
                      {{ errors.first('cacheIsDefaultPort') }}
                    </small>
                  </label>
                  <select
                    key="cacheIsDefaultPort"
                    v-model="cacheIsDefaultPort"
                    v-validate="'required'"
                    name="cacheIsDefaultPort"
                    :data-vv-as="$t('install.port')"
                    class="form-control"
                    :class="{'is-invalid': errors.has('cacheIsDefaultPort') }"
                  >
                    <option :value="true">
                      {{ $t('install.defaultPort') }}
                    </option>
                    <option :value="false">
                      {{ $t('install.customPort') }}
                    </option>
                  </select>
                </div>

                <div v-if="!cacheIsDefaultPort" class="form-group">
                  <label class="col-form-label">
                    {{ $t('install.customPort') }}
                    <small v-show="errors.has('cachePort')" class="text-danger">
                      {{ errors.first('cachePort') }}
                    </small>
                  </label>
                  <input
                    key="cachePort"
                    v-model="cachePort"
                    v-validate="'required|numeric'"
                    name="cachePort"
                    type="text"
                    :data-vv-as="$t('install.customPort')"
                    :class="{'is-invalid': errors.has('cachePort') }"
                    class="form-control"
                  >
                </div>

                <div class="form-group">
                  <label class="col-form-label">
                    {{ $t('install.redisPwd') }}
                    <small v-show="errors.has('cachePwd')" class="text-danger">
                      {{ errors.first('cachePwd') }}
                    </small>
                  </label>
                  <input
                    key="cachePwd"
                    v-model="cachePwd"
                    v-validate="'required'"
                    name="cachePwd"
                    type="password"
                    :data-vv-as="$t('install.redisPwd')"
                    :class="{'is-invalid': errors.has('cachePwd') }"
                    autocomplete="false"
                    class="form-control"
                  >
                </div>
              </template>

            </template>
          </form>
        </template>

        <template v-else-if="active === 3">
          <form class="form-horizontal" @submit="btnNextClick">
            <h4>{{ $t('install.administratorSettings') }}</h4>
            <hr>
            <div class="form-group">
              <label class="col-form-label">
                {{ $t('install.superAdminUsername') }}
                <small v-show="errors.has('adminName')" class="text-danger">
                  {{ errors.first('adminName') }}
                </small>
              </label>
              <input
                key="adminName"
                v-model="adminName"
                v-validate="'required'"
                name="adminName"
                type="text"
                :data-vv-as="$t('install.superAdminUsername')"
                :class="{'is-invalid': errors.has('adminName') }"
                autocomplete="false"
                class="form-control"
              >
            </div>

            <div class="form-group">
              <label class="col-form-label">
                {{ $t('install.superAdminPassword') }}
                <small v-show="errors.has('adminPassword')" class="text-danger">
                  {{ errors.first('adminPassword') }}
                </small>
              </label>
              <input
                ref="adminPassword"
                key="adminPassword"
                v-model="adminPassword"
                v-validate="'required|min:6'"
                name="adminPassword"
                type="password"
                :data-vv-as="$t('install.superAdminPassword')"
                :class="{'is-invalid': errors.has('adminPassword') }"
                autocomplete="false"
                class="form-control"
              >

              <small v-if="passwordScore > 80" class="form-text text-primary">
                {{ $t('install.passwordStrengthGreat') }}
              </small>
              <small v-else-if="passwordScore > 60" class="form-text text-primary">
                {{ $t('install.passwordStrengthStrong') }}
              </small>
              <small v-else-if="passwordScore > 0" class="form-text text-danger">
                {{ $t('install.passwordStrengthWeak') }}
              </small>
            </div>

            <div class="form-group">
              <label class="col-form-label">
                {{ $t('install.confirmPassword') }}
                <small v-show="errors.has('adminPasswordConfirm')" class="text-danger">
                  {{ errors.first('adminPasswordConfirm') }}
                </small>
              </label>
              <input
                key="adminPasswordConfirm"
                v-model="adminPasswordConfirm"
                v-validate="'required|confirmed:adminPassword'"
                name="adminPasswordConfirm"
                type="password"
                :data-vv-as="$t('install.confirmPassword')"
                :class="{'is-invalid': errors.has('adminPasswordConfirm') }"
                autocomplete="false"
                class="form-control"
              >
            </div>

            <h4>{{ $t('install.securitySettings') }}</h4>
            <hr>

            <div class="form-group">
              <el-switch
                v-model="isProtectData"
                :active-text="$t('install.encryptConnectionString')"
              />
            </div>

          </form>
        </template>

        <template v-else-if="active >= 4">

          <el-alert
            v-if="active > 4"
            :title="$t('install.complete')"
            type="success"
            show-icon
          >
            <p>
              <el-button plain>
                <router-link to="../">
                  {{ $t('install.goToLogin') }}
                </router-link>
              </el-button>
              <el-button plain>
                <a href="https://www.sscms.com/docs/" target="_blank">SS CMS {{ $t('docs') }}</a>
              </el-button>
            </p>
          </el-alert>

        </template>

        <hr>

        <div v-if="active < 4" class="text-center">
          <el-button :disabled="active === 0" type="primary" @click="btnPrevClick">
            {{ $t('previous') }}
          </el-button>

          <el-button type="primary" @click="btnNextClick">
            {{ $t('next') }}
          </el-button>
        </div>

      </div>
    </el-card>
  </div>
</template>

<script>
import { getInfo, installTryDatabase, installTryCache, installSaveSettings, install } from '@/api/cms'
import LangSelect from '@/components/LangSelect'
import '@/assets/css/bootstrap.min.css'
import '@/assets/css/icons.min.css'
import '@/assets/css/siteserver.min.css'

export default {
  name: 'Install',
  components: { LangSelect },
  data() {
    return {
      loading: true,
      active: 0,
      isAgreement: true,
      environment: {},
      databaseType: null,
      databaseIsConnectionString: false,
      databaseServer: null,
      databaseIsDefaultPort: true,
      databasePort: null,
      databaseUid: null,
      databasePwd: null,
      databaseNames: null,
      databaseName: null,
      databaseConnectionString: null,
      cacheType: null,
      cacheIsConnectionString: false,
      cacheServer: null,
      cacheIsDefaultPort: true,
      cachePort: null,
      cacheUid: null,
      cachePwd: null,
      cacheNames: null,
      cacheName: null,
      cacheConnectionString: null,
      adminName: null,
      adminPassword: null,
      adminPasswordConfirm: null,
      isProtectData: null,
      errorMessage: null
    }
  },
  computed: {
    passwordScore() {
      return this.scorePassword(this.adminPassword)
    }
  },
  watch: {
    databaseType() {
      this.databaseNames = null
      this.databaseName = null
    },
    databaseServer() {
      this.databaseNames = null
      this.databaseName = null
    },
    databaseIsDefaultPort() {
      this.databaseNames = null
      this.databaseName = null
    },
    databasePort() {
      this.databaseNames = null
      this.databaseName = null
    },
    databaseUid() {
      this.databaseNames = null
      this.databaseName = null
    },
    databasePwd() {
      this.databaseNames = null
      this.databaseName = null
    },
    cacheType() {
      this.cacheNames = null
      this.cacheName = null
    },
    cacheServer() {
      this.cacheNames = null
      this.cacheName = null
    },
    cacheIsDefaultPort() {
      this.cacheNames = null
      this.cacheName = null
    },
    cachePort() {
      this.cacheNames = null
      this.cacheName = null
    },
    cacheUid() {
      this.cacheNames = null
      this.cacheName = null
    },
    cachePwd() {
      this.cacheNames = null
      this.cacheName = null
    }
  },
  mounted() {
    getInfo().then(response => {
      this.loading = false
      this.environment = response
    })
  },
  methods: {
    apiTryDatabase() {
      this.loading = true
      if (this.databaseIsConnectionString) {
        installTryDatabase({
          databaseType: this.databaseType,
          connectionString: this.databaseConnectionString
        }).then(response => {
          this.loading = false
          if (response.isSuccess) {
            this.active = 2
          } else {
            this.errorMessage = response.errorMessage
          }
        })
      } else {
        installTryDatabase({
          databaseType: this.databaseType,
          server: this.databaseServer,
          port: this.databaseIsDefaultPort ? null : this.databasePort,
          uid: this.databaseUid,
          pwd: this.databasePwd
        }).then(response => {
          this.loading = false
          if (response.isSuccess) {
            this.databaseNames = response.databaseNames
            this.databaseName = null
          } else {
            this.errorMessage = response.errorMessage
          }
        })
      }
    },

    apiTryCache() {
      this.loading = true
      if (this.cacheIsConnectionString) {
        installTryCache({
          cacheType: this.cacheType,
          connectionString: this.cacheConnectionString
        }).then(response => {
          this.loading = false
          if (response.isSuccess) {
            this.active = 3
          } else {
            this.errorMessage = response.errorMessage
          }
        })
      } else {
        installTryCache({
          cacheType: this.cacheType,
          server: this.cacheServer,
          port: this.cacheIsDefaultPort ? null : this.cachePort,
          uid: this.cacheUid,
          pwd: this.cachePwd
        }).then(response => {
          this.loading = false
          if (response.isSuccess) {
            if (this.cacheType === 'Redis') {
              this.active = 3
            } else {
              this.cacheNames = response.databaseNames
              this.cacheName = null
            }
          } else {
            this.errorMessage = response.errorMessage
          }
        })
      }
    },

    apiSaveSettings() {
      // const adminUrl = _.trimEnd(location.href.toLowerCase(), '/').replace('/install/database', '')
      // const homeUrl = adminUrl.substring(0, adminUrl.lastIndexOf('/')) + '/home'

      this.loading = true
      this.active = 4
      installSaveSettings({
        databaseType: this.databaseType,
        databaseServer: this.databaseServer,
        databasePort: this.databaseIsDefaultPort ? null : this.databasePort,
        databaseUid: this.databaseUid,
        databasePwd: this.databasePwd,
        databaseName: this.databaseName,
        databaseConnectionString: this.databaseConnectionString,
        cacheType: this.cacheType,
        cacheServer: this.cacheServer,
        cachePort: this.cacheIsDefaultPort ? null : this.cachePort,
        cacheUid: this.cacheUid,
        cachePwd: this.cachePwd,
        cacheName: this.cacheName,
        cacheConnectionString: this.cacheConnectionString,
        isProtectData: this.isProtectData
      }).then(response => {
        setTimeout(() => {
          this.apiInstall(response.securityKey)
        }, 5000)
      })
    },

    apiInstall(securityKey) {
      install({
        securityKey: securityKey,
        adminName: this.adminName,
        adminPassword: this.adminPassword
      }).then(response => {
        this.loading = false
        if (response.isSuccess) {
          this.active = 5
        } else {
          this.errorMessage = response.errorMessage
        }
      })
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

    btnPrevClick(e) {
      e.preventDefault()
      this.active -= 1
    },

    btnNextClick(e) {
      e.preventDefault()
      if (this.active === 0) {
        this.active = 1
      } else if (this.active === 1) {
        this.$validator.validateAll().then((result) => {
          if (result) {
            if (this.databaseType === 'SQLite' || this.databaseName) {
              this.active = 2
            } else {
              this.apiTryDatabase()
            }
          }
        })
      } else if (this.active === 2) {
        this.$validator.validateAll().then((result) => {
          if (result) {
            if (this.cacheType === 'Memory' || this.cacheName) {
              this.active = 3
            } else {
              this.apiTryCache()
            }
          }
        })
      } else if (this.active === 3) {
        this.$validator.validateAll().then((result) => {
          if (result) {
            this.apiSaveSettings()
          }
        })
      }
    }
  }
}
</script>

<style lang="scss" scoped>
$bg:#32b09a;

.install-container {
  min-height: 100%;
  width: 100%;
  padding: 20px;
  overflow: hidden;
}
</style>
