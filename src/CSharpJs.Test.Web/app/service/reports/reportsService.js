sap.ui.define([
    'app/service/BaseService'
], function (baseService) {
    "use strict";

    return $.extend(baseService, {
        api: 'reports',

        reports: function (requestData) {
            var me = this;
            global.request({
                type: 'GET',
                url: '/api/reports/GetReports',
                dataType: 'json',
                context: me,
                async: false,
                error: function (xhr, status, error) {
                    requestData.failure.call(requestData.scope, { msg: treatError.getErrorMsg(xhr, status, error) });
                },
                success: function (response) {
                    requestData.success.call(requestData.scope, response);
                }
            });
        },

        reportbyName: function (requestData) {
            var me = this;
            global.request({
                type: 'POST',
                url: String.format('/api/reports/getReportByName'),
                dataType: 'json',
                context: me,
                data: JSON.stringify(requestData.data),
                async: false,
                error: function (xhr, status, error) {
                    requestData.failure.call(requestData.scope, { msg: treatError.getErrorMsg(xhr, status, error) });
                },
                success: function (response) {
                    requestData.success.call(requestData.scope, response);
                }
            });
        },

        GetLayouts: function(requestData) {
            var me = this;
            global.request({
                type: 'GET',
                url: String.format('/api/reports/GetLayouts/{0}', requestData.data),
                dataType: 'json',
                context: me,
                async: false,
                error: function (xhr, status, error) {
                    requestData.failure.call(requestData.scope, { msg: treatError.getErrorMsg(xhr, status, error) });
                },
                success: function (response) {
                    requestData.success.call(requestData.scope, response);
                }
            });
        },

        selectItem: function (requestData) {
            var me = this;
            global.request({
                type: 'GET',
                url: String.format('/api/reports/SelectItem/{0}', requestData.data),
                dataType: 'json',
                context: me,
                async: false,
                error: function (xhr, status, error) {
                    requestData.failure.call(requestData.scope, { msg: treatError.getErrorMsg(xhr, status, error) });
                },
                success: function (response) {
                    requestData.success.call(requestData.scope, response);
                }
            });
        },
        
    });
});




