sap.ui.define([
  "app/controller/BaseController",
  "app/scripts/Common",
  "sap/m/MessageToast",
  "sap/m/MessageBox",
  "app/service/BaseService",
], function(Controller, common, toast, msg, service) { 'use strict'
    return Controller.extend('app.controller.BaseDetailController', {
      
      validationFormId: null,

      onCancel: function() {
        let that = this
        if (that.hasChanged()) {
          common.question({
            title: 'Cancelar ',
            message: 'Deseja cancelar as informações?',
            scope: that,
            callback: function() {
              that.getRouter().navTo(that.backPage)
            }
          })
        } else that.getRouter().navTo(that.backPage)
      },

      getCancelElement: function() {
        return this.element
      },

      onOk: function() {
        let me = this
        if (me.validationSelecItems) {
          if (!me.validationSelecItems()) {
            return
          }
        }
        me.getRouter().navTo(that.backPage || me.backPage, me.getData())
      },

      onSave: function() {
        let me = this;

        let datas = [];
        datas.push(me.getViewData());

        common.question({
          title: 'Salvar ',
          message: 'Deseja salvar o documento?',
          scope: me,
          callback: function() {
            sap.ui.core.BusyIndicator.show()

            service.save({
              scope: me,
              api: `${me.api}/Create`,
              data: datas,
              success: function(response) {
                sap.ui.core.BusyIndicator.hide()
                toast.show('Operação realizada com sucesso')
                this.getRouter().navTo(me.backPage)
              },
              failure: function(response) {
                sap.ui.core.BusyIndicator.hide()
                msg.error(response.msg)
              }
            })
            
          }
        })
      },
      
      onUpdate: function() {
        let me = this;

        let datas = [];
        datas.push(me.getViewData());

        common.question({
          title: 'Salvar ',
          message: 'Deseja realmente salvar o documento?',
          scope: me,
          callback: function() {

            sap.ui.core.BusyIndicator.show()
            service.update({
              scope: me,
              api: `${me.api}/Update`,
              data: datas,
              success: function(response) {
                sap.ui.core.BusyIndicator.hide()
                toast.show('Operação realizada com sucesso')
                this.getRouter().navTo(me.backPage)
              },
              failure: function(response) {
                sap.ui.core.BusyIndicator.hide()
                msg.error(response.msg)
              }
            })

          }
        })
      },
      
    })
  }
)
