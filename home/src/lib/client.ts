import * as api from './api/api'

declare var config: {
    domainAPI: string
}

const client: api.API = new api.API({
    api: config.domainAPI
})

export default client
