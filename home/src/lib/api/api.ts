// https://github.com/michael/github

import {Options} from '../models'
import {WebRequest, APIRequest} from './http'
import * as client from './client'

export class API {
  request: WebRequest
  users: client.Users
  files: client.Files
  writing: client.Writing

  constructor(public options: Options) {
    this.request = new WebRequest()
    const apiRequest = new APIRequest(options)
    
    this.files = new client.Files(apiRequest)
    this.users = new client.Users(apiRequest)
    this.writing = new client.Writing(apiRequest)
  }
}
