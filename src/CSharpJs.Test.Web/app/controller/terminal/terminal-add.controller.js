sap.ui.define([
	"app/controller/BaseDetailController",
	"sap/ui/model/json/JSONModel",
	"sap/m/MessageBox",
	"app/service/BaseService",
	"app/scripts/Common",
	"sap/m/MessageToast",
], function(BaseDetailController, JSONModel, MessageBox, BaseService, common, MessageToast) {
	"use strict";

	return BaseDetailController.extend("app.controller.terminal.terminal-add", {
		api: "Terminal",
		backPage: "terminal-list",

		onInit: function() {
			let that = this;
			that._oRouter = that.getRouter();
			that._oRouter.getRoute('terminal-add').attachPatternMatched(that._onRouteMatched, that);
		},
	  
		_onRouteMatched: function(oEvent) {
			let that = this;
			this._oResourceBundle = that.getView().getModel('i18n').getResourceBundle();

			this.validateAuth();

			let obj = this.defaultData();
			this.loadElement(obj);

			this.GetListBranch();
		},

		defaultData: function() {
			return { 
				BranchList: [],
				OPEN_MD_TRML: {
					U_OPEN_TerminalCode: null,
					U_OPEN_BPLId: null
				}
			}
		},

		GetListBranch: function(){
			let me = this;
			let obj = me.getViewData();

			sap.ui.core.BusyIndicator.show();
			BaseService.list({
				scope: this,
				api: `BusinessPlace/Read/${null}/${null}/${null}`,
				success: function(response) {
					sap.ui.core.BusyIndicator.hide();

					obj.BranchList = response.oData.data.value;
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
      
			if (!obj.OPEN_MD_TRML.U_OPEN_BPLId) {
				MessageBox.warning(me._oResourceBundle.getText('base.Text.Valid.Field'));
				return;
			}

			this.onSaveTerminal();
		},

		onSaveTerminal: function() {
			let me = this;
			let obj = me.getViewData();
			
			let terminalList = [];
			terminalList.push(obj.OPEN_MD_TRML.U_OPEN_BPLId);
	
			common.question({
				title: 'Salvar ',
				message: 'Deseja salvar o documento?',
				scope: me,
				callback: function() {
					sap.ui.core.BusyIndicator.show();

					BaseService.save({
						scope: me,
						api: `${me.api}/Create`,
						data: terminalList,
						success: function(response) {
							sap.ui.core.BusyIndicator.hide();

							MessageToast.show('Operação realizada com sucesso!');
							this.getRouter().navTo(me.backPage);
						},
						failure: function(response) {
							sap.ui.core.BusyIndicator.hide();

							MessageBox.error(response.msg);
						}
					});
			  	}
			})
		},
		
	});
});
