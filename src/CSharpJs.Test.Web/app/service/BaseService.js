sap.ui.define([], function () {
  "use strict";
  return {
    api: null,

    list: function(requestData) {
      let me = this
      global.request({
        type: 'GET',
        url: '/api/' + requestData.api,
        data: requestData.data,
        dataType: 'json',
        context: me,
        error: function(xhr, status, error) {
          requestData.failure.call(requestData.scope, {
            msg: treatError.getErrorMsg(xhr, status, error)
          })
        },
        success: function(response) {
          let data = new sap.ui.model.json.JSONModel()
          data.setData(response)
          requestData.success.call(requestData.scope, data)
        }
      })
    },

    get: function(requestData) {
      let me = this
      global.request({
        type: 'GET',
        url: '/api/' + requestData.api,
        data: requestData.data,
        dataType: 'json',
        context: me,
        error: function(xhr, status, error) {
          requestData.failure.call(requestData.scope, {
            msg: treatError.getErrorMsg(xhr, status, error)
          })
        },
        success: function(response) {
          requestData.success.call(requestData.scope, response)
        }
      })
    },

    save: function(requestData) {
      let that = this
      global.request({
        type: 'POST',
        url: '/api/' + requestData.api,
        data: JSON.stringify(requestData.data),
        dataType: 'json',
        context: that,
        error: function(xhr, status, error) {
          requestData.failure.call(requestData.scope, {
            msg: treatError.getErrorMsg(xhr, status, error)
          })
        },
        success: function(response) {
          requestData.success.call(requestData.scope, response)
        }
      })
    },

    update: function(requestData) {
      let me = this
      global.request({
        type: 'PATCH',
        url: '/api/' + requestData.api,
        data: JSON.stringify(requestData.data),
        dataType: 'json',
        context: me,
        error: function(xhr, status, error) {
          requestData.failure.call(requestData.scope, {
            msg: treatError.getErrorMsg(xhr, status, error)
          })
        },
        success: function(response) {
          requestData.success.call(requestData.scope, response)
        }
      })
    },

    delete: function(requestData) {
      let me = this
      global.request({
        type: 'DELETE',
        url: '/api/' + requestData.api,
        data: JSON.stringify(requestData.data),
        dataType: 'json',
        context: me,
        error: function(xhr, status, error) {
          requestData.failure.call(requestData.scope, {
            msg: treatError.getErrorMsg(xhr, status, error)
          })
        },
        success: function(response) {
          requestData.success.call(requestData.scope, response)
        }
      })
    },
    
  }
})
  