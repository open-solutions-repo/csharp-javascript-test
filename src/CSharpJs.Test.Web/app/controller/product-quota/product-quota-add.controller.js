sap.ui.define([
	"app/controller/BaseDetailController",
	"sap/ui/model/json/JSONModel",
	"sap/m/MessageBox",
	"app/service/BaseService",
	"app/scripts/Common",
	"sap/m/MessageToast",
], function(BaseDetailController, JSONModel, MessageBox, BaseService, common, MessageToast) {
	"use strict";

	return BaseDetailController.extend("app.controller.product-quota.product-quota-add", {
		api: "ProductQuotas",
		backPage: "product-quota-list",

		onInit: function() {
			let that = this
			that._oRouter = that.getRouter()
			that._oRouter.getRoute('product-quota-add').attachPatternMatched(that._onRouteMatched, that)
		},
	  
		_onRouteMatched: function(oEvent) {
			let that = this;
			this._oResourceBundle = that.getView().getModel('i18n').getResourceBundle();

			this.validateAuth();

			let obj = this.defaultData();
			this.loadElement(obj);

			this.onRefreshData();
		},

		defaultData: function() {
			return { 
				OPEN_TD_PDQT: {
					DocNum: 0,
					DocEntry: 0,
					U_OPEN_GroupItem: null, 
					U_OPEN_Manufacturer: null,
					U_OPEN_Limit: 1
				},
				GroupItemList: [],
				ManufacturerList: []
			}
		},

		onRefreshData: function() {
			this.GetGroupItemList();
			this.GetManufacturerList();
		},

		GetGroupItemList: function() {
			let me = this;
			let obj = me.getViewData();

			sap.ui.core.BusyIndicator.show();
			BaseService.list({
				scope: this,
				api: `Item/Groups/${null}`,
				success: function(response) {
					sap.ui.core.BusyIndicator.hide();
					
					obj.GroupItemList = response.oData.data.value;
					obj.GroupItemList.push({ Number: '*', GroupName: 'Todos'  })
					
					this.loadElement(obj);
				},
				failure: function(response) {
					sap.ui.core.BusyIndicator.hide();
				  	MessageBox.error(response.msg);
				}
			})
		},

		GetManufacturerList: function() {
			let me = this;
			let obj = me.getViewData();

			sap.ui.core.BusyIndicator.show();
			BaseService.list({
				scope: this,
				api: `Manufacturer/Read/${null}`,
				success: function(response) {
					sap.ui.core.BusyIndicator.hide();
					
					obj.ManufacturerList = response.oData.data.value;
					obj.ManufacturerList.push({ Code: '*', ManufacturerName: 'Todos'  })

					this.loadElement(obj);
				},
				failure: function(response) {
					sap.ui.core.BusyIndicator.hide();
				  	MessageBox.error(response.msg);
				}
			})
		},

		onPressButtonSave: function() {
			let me = this;
      		let obj = me.getViewData();
      
			if (!obj.OPEN_TD_PDQT.U_OPEN_GroupItem 
				|| !obj.OPEN_TD_PDQT.U_OPEN_Manufacturer 
				|| obj.OPEN_TD_PDQT.U_OPEN_Limit < 1) {
				MessageBox.warning(me._oResourceBundle.getText('base.Text.Valid.Field'));
				return;
			};

			if (obj.OPEN_TD_PDQT.U_OPEN_GroupItem === '*'
				&& obj.OPEN_TD_PDQT.U_OPEN_Manufacturer === '*') {
				MessageBox.warning("Prenchimento incorreto para Grupo de Item e Fabricante.");
				return;
			};

			let datas = [];
        	datas.push(obj.OPEN_TD_PDQT);

			common.question({
				title: 'Salvar ',
				message: 'Deseja salvar o documento?',
				scope: me,
				callback: function() {
					sap.ui.core.BusyIndicator.show()
		
					BaseService.save({
						scope: me,
						api: `${me.api}/Create`,
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
		},
	});
});
