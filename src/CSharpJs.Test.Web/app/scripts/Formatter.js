jQuery.sap.declare('app.scripts.Formatter')
sap.ui.define(
  ['sap/ui/core/format/DateFormat', 'sap/ui/core/format/NumberFormat'],
  function(dateFormat) {
    'use strict'
    app.scripts.Formatter = {
      date: function(value) {
        if (value) {
          if (moment(value, 'DD/MM/YYYY', true).isValid()) return value
          var oDateFormat = sap.ui.core.format.DateFormat.getDateTimeInstance({
            pattern: 'dd/MM/yyyy',
            UTC: true
          })
          return oDateFormat.format(new Date(value))
        } else {
          return value
        }
      },
      dateYear: function(value) {
        if (value) {
          if (moment(value, 'DD/MM/YYYY', true).isValid()) return value
          var oDateFormat = sap.ui.core.format.DateFormat.getDateTimeInstance({
            pattern: 'yyy',
            UTC: true
          })
          return oDateFormat.format(new Date(value))
        } else {
          return value
        }
      },
      dateMonth: function(value) {
        if (value) {
          if (moment(value, 'DD/MM/YYYY', true).isValid()) return value
          var oDateFormat = sap.ui.core.format.DateFormat.getDateTimeInstance({
            pattern: 'MM/yyyy',
            UTC: true
          })
          return oDateFormat.format(new Date(value))
        } else {
          return value
        }
      },
      dateHour: function(value) {
        if (value) {
          if (moment(value, 'DD/MM/YYYY HH:mm:SS', true).isValid()) return value
          var oDateFormat = sap.ui.core.format.DateFormat.getDateTimeInstance({
            pattern: 'dd/MM/yyyy HH:mm:ss'
          })
          return oDateFormat.format(new Date(value))
        } else {
          return value
        }
      },
      hour: function(value) {
        if (value != null) {
          value = '0000' + value
          value = value.substring(value.length - 4)
          return value.substring(0, 2) + ':' + value.substring(2)
        }
        return ''
      },
      renderNumber: function(value) {
        var numberFormat = sap.ui.core.format.NumberFormat.getFloatInstance({
          minIntegerDigits: 1,
          minFractionDigits: 2,
          maxFractionDigits: 2,
          groupingEnabled: true,
          groupingSeparator: '.',
          decimalSeparator: ','
        })

        return numberFormat.format(value)
      },
      renderNumberDecimal: function(value) {
        var numberFormat = sap.ui.core.format.NumberFormat.getFloatInstance({
          minIntegerDigits: 1,
          minFractionDigits: 4,
          maxFractionDigits: 4,
          groupingEnabled: true,
          groupingSeparator: '.',
          decimalSeparator: ','
        })

        return numberFormat.format(value)
      },
      renderLineNumber: function(value) {
        return value != null && value != undefined ? value + 1 : null
      }
    }
  }
)
