sap.ui.define([
	"app/controller/BaseListController",
	"sap/ui/core/routing/History",
	"sap/ui/model/json/JSONModel",
	"sap/ui/thirdparty/jquery",
	"app/service/BaseService",
	"sap/ui/table/RowAction",
	"sap/ui/table/RowActionItem",
	"sap/m/MessageBox",
], function(BaseListController, History, JSONModel, jQuery, BaseService, RowAction, RowActionItem, MessageBox) {
	"use strict";

	return BaseListController.extend("app.controller.terminal.terminal-list", {
		api: "Terminal",
		backPage: "home",
		
		onInit: function() {
			var oRouter = sap.ui.core.UIComponent.getRouterFor(this);			
			oRouter.getRoute("terminal-list").attachMatched(this._onRouteMatched, this);

			var fnPress = this.handleActionPress.bind(this);
			this.modes = [
				{
					key: "Navigation",
					text: "Navigation",
					handler: function(){
						var oTemplate = new RowAction({items: [
							new RowActionItem({
								type: "Navigation",
								press: fnPress,
								visible: "{Available}"
							})
						]});
						return [1, oTemplate];
					}
				}
			];

			this.getView().setModel(new JSONModel({items: this.modes}), "modes");
			this.switchState("Navigation");
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
			  terminalList: [],
			};
		},

		switchState : function(sKey) {
			var oTable = this.byId("TableId");
			var iCount = 0;
			var oTemplate = oTable.getRowActionTemplate();
			if (oTemplate) {
				oTemplate.destroy();
				oTemplate = null;
			}

			for (var i = 0; i < this.modes.length; i++) {
				if (sKey == this.modes[i].key) {
					var aRes = this.modes[i].handler();
					iCount = aRes[0];
					oTemplate = aRes[1];
					break;
				}
			}

			oTable.setRowActionTemplate(oTemplate);
			oTable.setRowActionCount(iCount);
		},

		onRefreshData : function(){
			var me = this;
			let obj = me.getViewData();

			me.byId('TableId').setBusy(true)
			BaseService.list({
				scope: this,
				api: `${me.api}/Read/${null}/${null}/${null}`,
				success: function(response) {
					me.byId('TableId').setBusy(false)
					let datas = response.oData.data.value;

				  	obj.terminalList = datas;
					this.loadElement(obj);
				},
				failure: function(response) {
					me.byId('TableId').setBusy(false)
				  	MessageBox.error(response.msg);
				}
			})
		},

		onNavProductQuotaAdd : function(oEvent){
			this.getRouter().navTo("terminal-add");
		},

		handleActionPress : function(oEvent) {
			var oRow = oEvent.getParameter("row");
			let obj = oRow.getBindingContext().getObject();

			this.getOwnerComponent().getModel("myModel").setProperty("/data", obj);
			this.getRouter().navTo("terminal-detail");
		}
		
	});
}, true);
