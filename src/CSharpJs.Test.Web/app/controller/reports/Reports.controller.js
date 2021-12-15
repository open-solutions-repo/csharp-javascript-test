sap.ui.define([
	"app/controller/BaseDetailController",
    "app/service/BaseService",
    "app/service/reports/reportsService",
    "sap/m/MessageBox",
], function(BaseDetailController, service, reportService, MessageBox) {
	"use strict";

	return BaseDetailController.extend("app.controller.reports.Reports", {
		backPage: "home",

		onInit: function() {
			var oRouter = sap.ui.core.UIComponent.getRouterFor(this);			
			oRouter.getRoute("reports").attachMatched(this._onRouteMatched, this);
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
                reportList: [],
			};
		},

        onRefreshData: function(oEvent) {
            this.getReports();
        },

		/**
        * Este metodo retorna todos os relatorios do banco,
        * e insere no modelo.
        *
        * @return <ModelReport> oReport
        */
		getReports: function () {
            this.byId("newReport").setBusy(true);

            var me = this;
			let obj = me.getViewData();

            service.list({
				scope: this,
				api: `reports/GetReports`,
				success: function(response) {                    
					this.byId("newReport").setBusy(false);

                    obj.reportList = response.oData.data;
                    this.loadElement(obj);
				},
				failure: function(response) {
					this.byId("newReport").setBusy(false);

                    MessageBox.error(response.msg);
				}
			})
		},

        /**
        * Este metodo cria os Content de Input dentro
        * dos SimpleForm baseado no relatorio selecionado.
        */
        onChangeSelectReport: function (oEvent) { 
            const that = this;           
            that.byId("newReport").setBusy(true);
            
			let obj = that.getViewData();   
            var code = this.byId("newReport").getSelectedKey();
            
            that.report = null;
            that.report = obj.reportList.filter(report => { return report.code == code });
            that.report = that.report[0];

            reportService.reportbyName({
				scope: this,
                data: that.report.name,
				api: `reports/GetReportByName`,
				success: function(response) {                    
					this.byId("newReport").setBusy(false);

                    that.report = response.data;

                    var simpleForm = this.getForm();
                    var label = null;

                    that.report.parameters.forEach(prm => {

                        if (prm.fieldText) label = prm.fieldText;
                        else label = prm.fieldName;

                        simpleForm.addContent(new sap.m.Label({ text: label }));

                        if (prm.fieldName.indexOf('@userlogged') != -1) {
                            prm.fieldValue = localStorage.getItem("xnet-username")
                            simpleForm.addContent(new sap.m.Input({ id: 'user', value: prm.fieldValue, editable: false }));                    
                        }
                        else if (prm.fieldName.indexOf('@Select') != -1) {
                            const id = prm.fieldName.split('@')[0];
                            const query = 'S' + prm.fieldName.split('@S')[1];

                            const datas = this.getSelectItem(query);
                            var itens = [];
                            datas.oData.data.forEach(data => {
                                itens.push(new sap.ui.core.Item({ key: data.code.replace(/ /g, ""), text: data.code + " - " + data.name }));
                            });

                            simpleForm.addContent(new sap.m.ComboBox({ id: id.replace(/ /g, ""), items: itens }));
                        }
                        else if (prm.defaultValueList.length > 0) {
                            var itens = [];
                            prm.defaultValueList.forEach(data => {
                                itens.push(new sap.ui.core.Item({ key: data.value, text: data.description }));
                            });
                            simpleForm.addContent(new sap.m.ComboBox({ id: prm.fieldName, items: itens }));
                        }
                        else {
                            switch (prm.fieldType) {

                                case 'BooleanParameter':
                                    simpleForm.addContent(new sap.m.CheckBox({ id: prm.fieldName.replace(/ /g, "") }));
                                    break;

                                case 'CurrencyParameter':
                                    simpleForm.addContent(new sap.m.Input({ id: prm.fieldName.replace(/ /g, ""), type: "Number", placeholder: "00,00" }));
                                    break;

                                case 'DateParameter':
                                    if (prm.fieldDiscreteOrRange == "RangeValue")
                                        simpleForm.addContent(new sap.m.DateRangeSelection({ id: prm.fieldName.replace(/ /g, ""), placeholder: "dd/MM/yyyy - dd/MM/yyyy" }));
                                    else
                                        simpleForm.addContent(new sap.m.DatePicker({ id: prm.fieldName.replace(/ /g, ""), placeholder: "dd/MM/yyyy" }));
                                    break;

                                case 'DateTimeParameter':
                                    if (prm.fieldDiscreteOrRange == "RangeValue") {
                                        simpleForm.addContent(new sap.m.DateTimePicker({ id: "S" + prm.fieldName.replace(/ /g, ""), placeholder: "dd/MM/yyyy, HH:mm:ss" }));
                                        simpleForm.addContent(new sap.m.DateTimePicker({ id: "E" + prm.fieldName.replace(/ /g, ""), placeholder: "dd/MM/yyyy, HH:mm:ss" }));
                                    }
                                    else
                                        simpleForm.addContent(new sap.m.DateTimePicker({ id: prm.fieldName.replace(/ /g, ""), placeholder: "dd/MM/yyyy, HH:mm:ss" }));
                                    break;

                                case 'NumberParameter':
                                    simpleForm.addContent(new sap.m.Input({ id: prm.fieldName.replace(/ /g, ""), type: "Number", placeholder: "Number" }));
                                    break;

                                case 'StringParameter':
                                    simpleForm.addContent(new sap.m.Input({ id: prm.fieldName.replace(/ /g, "") }));
                                    break;

                                case 'TimeParameter':
                                    if (prm.fieldDiscreteOrRange == "RangeValue") {
                                        simpleForm.addContent(new sap.m.TimePicker({ id: "S" + prm.fieldName.replace(/ /g, "") }));
                                        simpleForm.addContent(new sap.m.TimePicker({ id: "E" + prm.fieldName.replace(/ /g, "") }));
                                    }
                                    else
                                        simpleForm.addContent(new sap.m.TimePicker({ id: prm.fieldName.replace(/ /g, "") }));
                                    break;
                            }
                        }
                    })  

                },
                failure: function(response) {
                    that.byId("newReport").setBusy(false);
                    MessageBox.error(response.msg);
                }
            })
        },

        /**
        * Este metodo ao passar uma query (Consulta) retorna todos
        * o resultado dela e insere como um objeto Json no variavel data.
        *
        * @param <String> query
        * @return JSONModel data
        */
        getSelectItem: function (query) {
            const that = this;
            var data;

            reportService.selectItem({
                scope: that,
                data: query,
                failure: function (response) {
                    MessageBox.error(response.msg);
                },
                success: function (list) {
                    data = new sap.ui.model.json.JSONModel(list);
                }
            }, that);
            return data;
        },

        /**
        * Este metodo pega o SimpleForm da View
        * e destroy todos os elementos que sejam maior de 2.
        * 
        * Maior de 2 porque 1 é label "Modelos" e o outro é o Combox com os relatório,
        * portanto tudo o resto acima de 2 e destruido.
        * Retornando um simpleform novo.
        *
        * @return simpleForm
        */
        getForm: function () {
            var simpleForm = this.byId("newSimpleFormChange471");

            var cont = simpleForm._aElements.length
            if (cont > 2) {
                for (var i = 2; i < cont; i++) {
                    simpleForm._aElements[2].destroy();
                }
            };
            return simpleForm;
        },

        /**
        * Este metodo pega o report da View e prepara ele,
        * preenchendo o campo fielvalue e verificando se o campo é obrigatório
        *
        * @return ModelReport report
        */
        prepareReport: function () {
            var validated = true;
            var contents = this.byId("newSimpleFormChange471")._aElements;

            this.report.parameters.forEach(prm => {
                contents.forEach(content => {

                    if (validated) {

                        var field;
                        if (prm.fieldName.indexOf('@Select') != -1) {
                            field = prm.fieldName.split('@')[0];
                        }
                        else {
                            field = prm.fieldName.replace(/ /g, "");
                        }

                        if (field == content.sId || "S" + field == content.sId || "E" + field == content.sId) {

                            if (!prm.fieldOptional) {
                                if (content.mProperties.value === null || content.mProperties.value === '') {
                                    sap.m.MessageBox.error('Campo "' + content.sId + '" é obrigatório!');
                                    sap.ui.core.BusyIndicator.hide();
                                    validated = false;
                                }
                            }

                            if (prm.fieldType === 'BooleanParameter') {
                                prm.fieldValue = content.mProperties.selected;
                            }
                            else if ((prm.fieldType === 'DateTimeParameter' || prm.fieldType === 'TimeParameter') && (content.sId == "S" + field || content.sId == "E" + field)) {
                                if (content.sId == "S" + field) prm.fieldValue = content.mProperties.value;
                                else if (content.sId == "E" + field) prm.fieldValue = prm.fieldValue + " - " + content.mProperties.value;
                            }
                            else if (!(content.mProperties.selectedKey === null || content.mProperties.selectedKey === "")) {
                                prm.fieldValue = content.mProperties.selectedKey;
                            }
                            else prm.fieldValue = content.mProperties.value;

                            if (prm.fieldType === 'DateParameter') {
                                prm.fieldValue = content.mProperties.value;
                            }
                        }
                        field = null;
                    }

                })
            });
            if (validated)
                return this.report;
            else
                return false;
        },
		
        /**
        * Este metodo possui a finalidade de enviar o report preparado para a api (BackEnd)
        * onde o mesmo será impresso em PDF.
        * 
        * No final recarrega a página com o metodo this.onRefreshLayout(), recomecando
        * tudo de novo.
        */
        onPressPrintReport: function () {
            const report = this.prepareReport();

            if (report) {
                global.requestPrintReport(report, '/api/reports/PrintReport');
                this.onRefreshData();
            }
        }

	});
}, true);
