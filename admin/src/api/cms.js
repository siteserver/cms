import request from '@/utils/request'

export function getInfo() {
  return request({
    url: '/cms',
    method: 'get'
  })
}

export function installTryDatabase(data) {
  return request({
    url: '/cms/install/tryDatabase',
    method: 'post',
    data
  })
}

export function installTryCache(data) {
  return request({
    url: '/cms/install/tryCache',
    method: 'post',
    data
  })
}

export function installSaveSettings(data) {
  return request({
    url: '/cms/install/saveSettings',
    method: 'post',
    data
  })
}

export function install(data) {
  return request({
    url: '/cms/install',
    method: 'post',
    data
  })
}
