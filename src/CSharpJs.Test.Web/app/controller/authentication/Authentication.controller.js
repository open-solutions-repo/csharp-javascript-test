sap.ui.define([
  "app/controller/BaseController",
  "app/service/authentication/AuthenticationService",
  "sap/ui/model/json/JSONModel",
  "sap/m/MessageBox",
  "app/service/BaseService",
], function(BaseController, AuthenticationService, JSONModel, MessageBox, BaseService) {
  'use strict' 
  
  return BaseController.extend('app.controller.authentication.authentication', {

    onInit: function() {
      sessionStorage.clear();

      let that = this;
      that._oRouter = that.getRouter();
      that._oRouter.getRoute('authentication').attachPatternMatched(that._onRouteMatched, that);
    },

    _onRouteMatched: function(oEvent) {
      let that = this;
      this._oResourceBundle = that.getView().getModel('i18n').getResourceBundle();

      sessionStorage.removeItem('open-username');
      sessionStorage.removeItem('open-company');
      sessionStorage.removeItem('open-token');

      let obj = this.defaultData();
      this.loadElement(obj);
      
      this.loadCompanyList();
    },

    defaultData: function() {
      return { 
        username: null, 
        password: null,
        company: null,
        companies: []
      }
    },

    refreshLayout: function(data) {},

    onLoginPress: function() {
      let me = this
      let obj = me.getViewData()
      
      if (!obj.username || !obj.password || !obj.company) {
        MessageBox.warning(me._oResourceBundle.getText('base.Text.Valid.Field'))
        return;
      }

      sap.ui.core.BusyIndicator.show()
      AuthenticationService.authentication({
        scope: me,
        data: {
          username: obj.username,
          password: Base64.encode(obj.password),
          company:  obj.company,
          language: 'pt-BR',
          grant_type: 'password'
        },
        success: function(response) {
          sap.ui.core.BusyIndicator.hide()

          sessionStorage.setItem('open-username', obj.username)
          sessionStorage.setItem('open-company', obj.company)
          sessionStorage.setItem('open-token', response.access_token);
          sessionStorage.setItem('open-password', Base64.encode(obj.password));

          sap.ui.core.BusyIndicator.show();
          BaseService.get({
            scope: this,
            api: `Operator/Validate/${obj.username}`,
            success: function(response) {
              sap.ui.core.BusyIndicator.hide();

              if (response.data.value == 0) {
                sessionStorage.removeItem('open-cashier-bplid');
                sessionStorage.removeItem('open-cashier-terminal');
                sessionStorage.removeItem('open-cashier-docentry');
                sessionStorage.removeItem('open-cashier-status');
                sessionStorage.removeItem('open-cashier-operator');
              
                MessageBox.warning("Usuário não autorizado!");
                return;
              }

              sessionStorage.setItem('open-collaborator-position', response.data.value);
              this.getRouter().navTo('home');
            },
            failure: function(response) {
              sap.ui.core.BusyIndicator.hide()
              sap.m.MessageBox.error(response.msg)
            }
          })
        },
				failure: function(response) {
					sap.ui.core.BusyIndicator.hide();
					MessageBox.error(response.msg);
				}
			})
    },

    loadCompanyList: function() {
      var obj = {
        companies:system.configuration.companies
      }

      var oModel = new sap.ui.model.json.JSONModel(obj);
      this.getView().setModel(oModel);  
    }

  })
})
