sap.ui.define([
	"app/controller/BaseDetailController",
	"sap/ui/core/routing/History",
	"sap/ui/model/json/JSONModel",
	"sap/ui/thirdparty/jquery",
	"app/scripts/Common",
	"app/service/BaseService",
	"sap/m/MessageToast",
	"sap/m/MessageBox",
], function(BaseDetailController, History, JSONModel, jQuery, common, service, MessageToast, MessageBox) {
	"use strict";

	return BaseDetailController.extend("app.controller.terminal.terminal-detail", {
		api: "Terminal",
		backPage: "terminal-list",
		
		onInit: function() {
			var oRouter = sap.ui.core.UIComponent.getRouterFor(this);			
			oRouter.getRoute("terminal-detail").attachMatched(this._onRouteMatched, this);
		},

		_onRouteMatched: function(oEvent) {
			let that = this;
			this._oResourceBundle = that.getView().getModel('i18n').getResourceBundle();
			
			let obj = that.getOwnerComponent().getModel("myModel").getProperty("/data");
			
			this.loadElement(obj);
			this.validateAuth();
		},

		onDelete: function() {
			let me = this;
			let obj = me.getViewData();
	
			let datas = [];
			datas.push(obj.docEntry);
	
			common.question({
				title: 'Remover ',
				message: 'Deseja realmente remover?',
				scope: me,
				callback: function() {
					sap.ui.core.BusyIndicator.show()
					service.delete({
						scope: me,
						api: `${me.api}/Delete`,
						data: datas,
						success: function(response) {
							sap.ui.core.BusyIndicator.hide()
							MessageToast.show('Operação realizada com sucesso')
							this.getRouter().navTo(me.backPage)
						},
						failure: function(response) {
							sap.ui.core.BusyIndicator.hide()
							MessageBox.error(response.msg)
						}
					})
				}
			})
		}

	});
});
