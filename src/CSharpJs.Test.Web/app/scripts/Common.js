jQuery.sap.declare('app.scripts.Common')
jQuery.sap.require('sap.m.MessageBox')
app.scripts.Common = {
  sum: function(list, prop) {
    var total = 0
    for (var i = 0, _len = list.length; i < _len; i++) {
      total += list[i][prop]
    }
    return total
  },
  question: function(data) {
    var me = this
    sap.m.MessageBox.show(data.message, {
      title: data.title,
      icon: sap.m.MessageBox.Icon.QUESTION,
      actions: [sap.m.MessageBox.Action.YES, sap.m.MessageBox.Action.NO],
      onClose: function(btn) {
        if (btn == 'YES') {
          data.callback.call(data.scope)
        }
      }
    })
  },
  onOSRead: function(valor) {
    var resultado = valor.split('-')
    return resultado[1]
  },
  onZeroPress: function(valor) {
    valor = valor.startsWith('0', 0) ? valor.substring(1) : valor
    return valor.startsWith('0', 0) ? valor.substring(1) : valor
  },
  calculateComponentsCosts: function(
    component,
    ignoreCompositionQuantity,
    compositionQuantity,
    projectQuantity
  ) {
    var costs = 0
    for (var i = 0; i < component.elements.length; i++) {
      var item = component.elements[i]
      costs +=
        (ignoreCompositionQuantity ? 1 : compositionQuantity) *
        item.total *
        projectQuantity
    }
    return costs
  },
  calculateCompositionResume: function(
    composition,
    ignoreCompositionQuantity,
    projectQuantity
  ) {
    var prjQuantity = projectQuantity || 1
    var costs = 0,
      variableExpenses = 0
    var cp = composition

    if (cp.components) {
      for (var j = 0; j < cp.components.length; j++) {
        var component = cp.components[j]
        costs += app.scripts.Common.calculateComponentsCosts(
          component,
          ignoreCompositionQuantity,
          cp.quantity,
          prjQuantity
        )
      }
    }

    if (cp.taxs) {
      for (var j = 0; j < cp.taxs.length; j++) {
        var tax = cp.taxs[j]
        variableExpenses += tax.rate
      }
    }

    if (cp.commissions) {
      for (var j = 0; j < cp.commissions.length; j++) {
        var commission = cp.commissions[j]
        variableExpenses += commission.rate
      }
    }

    if (cp.additionalExpenses) {
      for (var j = 0; j < cp.additionalExpenses.length; j++) {
        var additionalExpense = cp.additionalExpenses[j]
        variableExpenses += additionalExpense.rate
      }
    }

    var fixExpenses = cp.fixExpenses
    var profitMargin = cp.profitMargin
    var discount = cp.discount

    var mkp =
      1 - (fixExpenses + variableExpenses + profitMargin - discount) / 100
    var pricingSale = costs / mkp

    return {
      cost: costs,
      fixExpenses: (cp.fixExpenses / 100) * pricingSale,
      variableExpenses: (variableExpenses / 100) * pricingSale,
      profitMargin: (cp.profitMargin / 100) * pricingSale,
      discount: (cp.discount / 100) * pricingSale,
      pricingSale: pricingSale
    }
  },
  listCount: function(list) {
    return list.length
  },
  renderDecimal2hhmm: function(value) {
    try {
      return convert.dec2hhmm(value)
    } catch (e) {
      return value
    }
  },
  validTime: function(oEvent) {
    var src = oEvent.getSource()
    var status = true
    src.setValueState('None')

    var value = src.getValue()
    if (src.getValue().length > 0) {
      //Verifica se conseguiu converter para Decimal a hora e minuto informado
      if (
        parseFloat(convert.time2dec(value.replace(',', '.')))
          .toString()
          .indexOf('N') >= 0
      ) {
        src.setValueState('Error')
        status = false
      }
    }

    return true
  },
  currentPasswordEnabled: true,
  closeEnabled: true,
  loadConfiguration: function(url) {
    if (system.configuration == undefined) {
      sap.ui.core.BusyIndicator.show()
      $.ajax({
        type: 'GET',
        timeout: 90000,
        url: url,
        dataType: 'json',
        async: false,
        error: function(xhr, status, error) {
          sap.ui.core.BusyIndicator.hide()
          alert('Não foi possível carregar o arquivo de configuração.')
          return null
        },
        success: function(response) {
          sap.ui.core.BusyIndicator.hide()
          system.configuration = response
          $.ajaxSetup({ timeout: response.timeout })
        }
      })
    }
  }
}
