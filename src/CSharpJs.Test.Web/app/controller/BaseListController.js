sap.ui.define([
  'app/controller/BaseController',
  'sap/ui/core/format/NumberFormat',
  'sap/m/MessageBox',
  'app/scripts/Common',
  'sap/m/MessageToast',
  'sap/ui/model/json/JSONModel',
  'sap/ui/model/Filter',
  'sap/ui/model/FilterOperator'
],
function(
  Controller,
  numberFormat,
  msg,
  common,
  toast,
  JSONModel,
  Filter,
  FilterOperator
) {'use strict'
    return Controller.extend('app.controller.BaseListController', {
      
      onInit: function() {
        var me = this
        me.refreshListPanel('', 0, 1)
      },

      handleListItemPress: function(oEvent) {
        var context = oEvent.getSource().getBindingContext()
        this.getRouter().navTo(this.detail, {
          code: context.getProperty('code')
        })
      },

      handleListSelect: function(oEvent) {
        var context = oEvent.getParameter('listItem').getBindingContext()
        this.getRouter().navTo(this.detail, {
          code: context.getProperty('code')
        })
      },

      handleSearch: function(oEvt) {
        // add filter for search
        let aFilters = []
        let oFilter
        let sQuery = oEvt.getSource().getValue()
        if (sQuery && sQuery.length > 0) {
          aFilters.push(new Filter('name', FilterOperator.Contains, sQuery))
          aFilters.push(new Filter('email', FilterOperator.Contains, sQuery))
          oFilter = new Filter({ filters: aFilters, and: false })
        }
        // update list binding
        let list = this.byId('list')
        let binding = list.getBinding('items')
        binding.filter(oFilter, 'Application')
      },

      refreshListPanel: function(value, skip, currentPage) {
        var me = this
        //me.byId('list').setBusy(true)
        me.service.list({
          scope: me,
          success: function(response) {
            let oView = me.getView()
            oView.setModel(response)
            //me.byId('list').setBusy(false)
          },
          failure: function(response) {
            //me.byId('list').setBusy(false)
            msg.error(response.msg)
          }
        })
      }
      
    })
  }
)
