sap.ui.define([
    'sap/ui/core/UIComponent', 
    'sap/ui/model/json/JSONModel', 
    'sap/ui/Device'
], function(UIComponent, JSONModel, Device) {
    'use strict'

    return UIComponent.extend('app.Component', {
      
      metadata: { manifest: 'json' },

      init: function() {
        var oDeviceModel = new JSONModel(Device)
        oDeviceModel.setDefaultBindingMode('OneWay')
        this.setModel(oDeviceModel, 'device')
        var oI18nModel = new sap.ui.model.resource.ResourceModel({ bundleUrl: 'i18n/i18n.properties' })
        this.setModel(oI18nModel, 'i18n')

        UIComponent.prototype.init.apply(this, arguments)

        this.getRouter().initialize()
      },
    })
  }
)
