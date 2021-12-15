sap.ui.define([
  'app/service/BaseService'
], function (BaseService) {
  "use strict";

    return $.extend(BaseService, {

    authentication: function(requestData) {
      let me = this
      global.request({
        context: me,
        url: '/token',
        method: 'POST',
        type: 'POST',
        dataType: 'json',
        headers: {
          "content-type": "application/x-www-form-urlencoded",
          "Authorization": "Basic " + Base64.encode('SYSTEM:B1Admin00')
        },
        data: requestData.data,
        error: function(xhr, status, error) {
          requestData.failure.call(requestData.scope, { msg: treatError.getErrorMsg(xhr, status, error) })
        },
        success: function(response) {
          requestData.success.call(requestData.scope, response)
        }
      })
    },
    
  })
})
  