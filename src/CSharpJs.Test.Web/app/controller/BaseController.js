sap.ui.define([
    'sap/ui/core/mvc/Controller',
    'sap/ui/core/routing/History',
    'sap/ui/core/UIComponent',
    'app/scripts/helper',
    'app/scripts/string',
    'app/scripts/init',
    'app/scripts/Formatter',
    'app/scripts/Common',
    'app/scripts/moment',
    'sap/m/MessageBox',
    'sap/m/MessageToast',
    'app/service/BaseService',
    'sap/ui/model/json/JSONModel'
],(
    Controller,
    History,
    UIComponent,
    helper,
    string,
    init,
    Formatter,
    common,
    moment,
    msg,
    toast,
    service,
    JSONModel
  ) =>
    Controller.extend('app.controller.BaseController', {
    baseAccessKey: null,
    rawDataLoaded: null,
    isLoadElement: false,

    onInit() {
      const me = this
      me.hasChange = false
      me.initToolTip(me.tooltips)
    },

    defaultData: {},

    setListController(controller) {
      this.listController = controller
    },

    initToolTip(list) {
      const me = this
      if (list !== null && list !== undefined) {
        list.forEach(el => {
          me.getView()
            .byId(el.id)
            .setTooltip(
              new sap.ui.commons.RichTooltip({
                text: el.text
              })
            )
        }, me)
      }
    },

    get(id) {
      const me = this
      sap.ui.core.BusyIndicator.show()
      service.get({
        scope: me,
        data: id,
        success(response) {
          const data = new JSONModel(response)
          me.loadData(data)
          sap.ui.core.BusyIndicator.hide()
        },
        failure(response) {
          sap.ui.core.BusyIndicator.hide()
          msg.error(response.msg)
        }
      })
    },

    getRouter() {
      return UIComponent.getRouterFor(this)
    },

    onNavBack() {
      let oHistory
      let sPreviousHash
      this.oData = null
      oHistory = History.getInstance()
      sPreviousHash = oHistory.getPreviousHash()

      if (sPreviousHash !== undefined) {
        window.history.go(-1)
      } else {
        this.getRouter().navTo('home', {}, true)
      }
    },

    getData() {
      // TODO IMPLEMENTATION
      return null
    },

    loadData(data) {
      this.getView().setModel(data)
      this.refreshLayout(data.getData())
    },

    loadObj(obj) {
      this.isLoadElement = true
      try {
        const data = new JSONModel(obj)
        this.loadData(data)
      } finally {
        this.isLoadElement = false
      }
    },

    getViewData() {
      return this.getView().getModel().getData();
    },

    loadElement(data) {
      this.element = data
      this.loadObj(data)
    },

    getFieldValue(fieldId, prop) {
      const value = this.byId(fieldId).getValue()
      return prop ? value[value] : value
    },

    hasChanged() {
      return true
    },

    setModelValue(propertyBind, value) {
      this.getView().getModel().setProperty(propertyBind, value)
    },

    isAuthorized(accessKey, callback, scope) {
      sap.ui.core.BusyIndicator.show()
      const auth = global.checkAuthorization(accessKey)
      if (!auth.hasError) {
        sap.ui.core.BusyIndicator.hide()
        callback.call(scope)
      } else {
        sap.ui.core.BusyIndicator.hide()
        toast.show(
          (auth.error.requestURL === undefined
            ? ''
            : `${auth.error.requestURL} `) + auth.error.statusText
        )
      }
    },

    onNew() {
      this.getRouter().navTo(this.detail, {
        code: 0
      })
    },

    onNavBackPage: function() {
      this.getRouter().navTo(this.backPage)
    },

    refreshLayout: function(data) {
      //TODO IMPLEMENTATION
    },

    validateAuth: function() {
      let that = this; 
      
      const username = sessionStorage.getItem('open-username');
      const company = sessionStorage.getItem('open-company');
      const token = sessionStorage.getItem('open-token');

      if (token === undefined || token === null || token === ''
      || username === undefined || username === null || username === ''
      || company === undefined || company === null || company === ''
      ) {
        msg.warning(that._oResourceBundle.getText('home.Text.04'), {
          actions: [msg.Action.CLOSE],
          onClose: function (sAction) {
            that.getRouter().navTo('authentication')
            return
          }
        })
      }
		},
    
  })
)
