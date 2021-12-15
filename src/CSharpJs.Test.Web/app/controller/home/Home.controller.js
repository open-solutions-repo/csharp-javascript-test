sap.ui.define([
    "app/controller/BaseDetailController",
    "sap/m/MessageBox",
    "sap/ui/model/json/JSONModel",
    "app/service/BaseService",
], function(BaseDetailController, MessageBox, JSONModel, service) {
  'use strict'

  return BaseDetailController.extend('app.controller.home.Home', {

    onInit: function() {
      let oRouter, oTarget

      oRouter = this.getRouter()
      oTarget = oRouter.getTarget('home')

      oTarget.attachDisplay(function(oEvent) {
        this._oData = oEvent.getParameter('data')
      }, this)

      oRouter.getRoute('home').attachPatternMatched(this._onRouteMatched, this)
    },

    _onRouteMatched: function() {
      let that = this;      
      this._oResourceBundle = that.getView().getModel('i18n').getResourceBundle();

      this.validateAuth();
      
      let obj = this.defaultData();
      this.loadElement(obj);
      
      this.SetTerminalCount();
      this.SetProductQuotaCount();
      this.SetReportCount();
    },

    defaultData: function() {
			return {
        username: sessionStorage.getItem('open-username'),
			  company: sessionStorage.getItem('open-company'),
        status: this.GetStatusCashier(),
        productQuotaCount: 0,
        terminalCount: 0,
        reportCount: 0,
        productQuotaState: 'Loaded',
        terminalState: 'Loaded',
        reportState: 'Loaded'
      };
		},

    SetTerminalCount: function(){
      let me = this;
      let obj = me.getViewData();

      obj.terminalState = "Loading";
      this.loadElement(obj);

			service.list({
				scope: this,
				api: `Terminal/Read/${null}/${null}/${null}`,
				success: function(response) {
					let datas = response.oData.data.value.filter(data => { return data.Canceled != 'Y' })

          obj.terminalCount = datas.length;
          obj.terminalState = "Loaded";
          this.loadElement(obj);
				},
				failure: function(response) {
				  MessageBox.error(response.msg);

          obj.terminalState = "Loaded";
          this.loadElement(obj);
				}
			})
		},

    SetProductQuotaCount: function(){		},

    SetReportCount: function(){		},

    GetStatusCashier: function() {    },

    onLogout: function() {
      let that = this;
      
      MessageBox.confirm(that._oResourceBundle.getText('home.Text.03'), {
        actions: [MessageBox.Action.OK, MessageBox.Action.CLOSE],
        onClose: function (sAction) {
          if(sAction == 'OK'){
            sessionStorage.removeItem('open-username');
            sessionStorage.removeItem('open-company');
            sessionStorage.removeItem('open-token');
            
            that.getRouter().navTo('authentication', {}, true);
          }
        }
      });
    },

    pressTile: function(oEvent) {
      let position = sessionStorage.getItem('open-collaborator-position');

      if (position == 1) {
        this.getRouter().navTo(oEvent.getSource().data('ref'), {
          type: oEvent.getSource().data('period')
        })
      }
      else {
        MessageBox.warning("Usuário não autorizado!");
      }
    },

    pressCashierTile: function(oEvent) {
      let status = sessionStorage.getItem('open-cashier-status');

      if(status == 1) {
        this.getRouter().navTo("cashier");
      }
      else {
        this.getRouter().navTo("opening");
      }

    },

  })
})
