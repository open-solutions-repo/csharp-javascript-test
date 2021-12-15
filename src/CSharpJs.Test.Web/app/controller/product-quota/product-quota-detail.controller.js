sap.ui.define([
	"app/controller/BaseDetailController",
	"sap/ui/core/routing/History",
	'sap/ui/model/json/JSONModel',
	"sap/ui/thirdparty/jquery",
	"app/scripts/Common",
	"app/service/BaseService",
	"sap/m/MessageToast",
	"sap/m/MessageBox",
], function(BaseDetailController, History, JSONModel, jQuery, common, service, MessageToast, MessageBox) {
	"use strict";

	return BaseDetailController.extend("app.controller.product-quota.product-quota-detail", {
		api: "ProductQuotas",
		backPage: "product-quota-list",

		onInit: function() {
			var oRouter = sap.ui.core.UIComponent.getRouterFor(this);			
			oRouter.getRoute("product-quota-detail").attachMatched(this._onRouteMatched, this);
		},

		_onRouteMatched: function(oEvent) {
			let that = this;
			this._oResourceBundle = that.getView().getModel('i18n').getResourceBundle();
			
			let obj = that.getOwnerComponent().getModel("myModel").getProperty("/data");

			this.loadElement(obj);
			this.validateAuth();
		},

		onPressButtonSave: function() {
			let me = this;
      		let obj = me.getViewData();
      
			if (!obj.u_OPEN_Limit) {
				MessageBox.warning(me._oResourceBundle.getText('base.Text.Valid.Field'));
				return;
			};

			let quota = {
				DocEntry: obj.docEntry,
				U_OPEN_Limit: obj.u_OPEN_Limit
			}

			let datas = [];
			datas.push(quota);
	
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
					
					MessageToast.show('Operação realizada com sucesso')
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
