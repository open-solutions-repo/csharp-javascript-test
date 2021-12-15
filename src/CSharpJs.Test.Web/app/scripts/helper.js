String.prototype.format = function (o) {
  return this.replace(/{([^{}]*)}/g,
      function (a, b) {
          var r = o[b];
          return typeof r === 'string' ? r : a;
      }
  );
};

var treatError = {
  getErrorMsg: function (xhr, status, error) {
      if (status == 'parsererror')
          return error.toString();

      if (xhr.msg) return xhr.msg;

      const getMessage = function () {
          if (xhr.responseJSON?.message?.includes("The conversion of a nvarchar data type to a datetime data type resulted in an out-of-range value.")) {
              return "A data está no formato inválido. Favor preencher novamente!";
          }
          switch (xhr.responseJSON.message) {
              case "SAP Business One Authentication failed":
                  return "Falha na autenticação. Verifique o Usuário e Senha.";

              case "Illegal value entered":
                  return "Falta campos a preencher";

              case "Esta entrada já existe nas tabelas seguintes (ODBC -2035)":
                  return "Código ja existe, favor alterar!";

              case "Authorization has been denied for this request.":
                  application.logoutUser("Token expirou, favor logar novamente");
                  return "Token expirou, favor logar novamente";

              case null:
                  return "Erro";
              //case undefined:
              //    console.error(xhr.responseJSON.StackTrace);
              //    return xhr.responseJSON.Message;
              //    break;

              default:
                  return xhr.responseJSON.message;
          }
      };

      if ((xhr.responseJSON !== undefined) && (typeof (xhr.responseJSON === "object"))) {
          if ((xhr.responseJSON.error !== null) && (xhr.responseJSON.error !== undefined)) {
              if (xhr.responseJSON.error == 'invalid_grant') return "Falha na autenticação. Verifique o Usuário e Senha.";
              if (xhr.responseJSON.error == 'access_denied') return xhr.responseJSON.error_description;
              if (xhr.responseJSON.error.code) {
                  switch (xhr.responseJSON.error.code) {
                      case -5002:
                          return "Favorecido não permite operação com essa moeda";
                          break;

                      default:
                          return xhr.responseJSON.error.message.value;
                          break;
                  }
              } else {
                  return getMessage();
              }
          } else {
              return getMessage();
          }
      } else {
          switch (xhr.status) {
              case 404:
                  return 'Arquivo não encontrado \n URL:' + xhr.requestURL;

              default:
                  return xhr.status + ' - ' + xhr.statusText + '\n URL:' + xhr.requestURL;
          }
      }
  },
  showError: function (xhr, status, error) {
      return sap.m.MessageBox.error(treatError.getErrorMsg(xhr));
  },

}

var clone = function (obj) {
  return JSON.parse(JSON.stringify(obj));
}

const application = {
  logoutUser: function (message) {
      sessionStorage.removeItem('open-company');
      sessionStorage.removeItem('open-username');
      sessionStorage.removeItem('open-password');
      sessionStorage.removeItem('open-token');
      sessionStorage.removeItem('open-access');
      sessionStorage.removeItem('open-branch');
      sessionStorage.removeItem('QtyDec');
      sessionStorage.removeItem('UseCatalogNumber');
      system = {};

      const goToIndex = function () {
          window.location = '/';
      };

      if (message) {
          sap.m.MessageBox.alert(message);
          setTimeout(goToIndex, 3000);
      }
      else
          goToIndex();
  },
}

var convert = {
  toFloat: function (valueString, precision) {
      return parseFloat(valueString.replace(/[^\d\-.,]/g, "").replace(",", ".").replace(/\.(?=.*\.)/g, "")).toFixed(precision || 2);
  },
  toInt: function (valueString) {
      return parseInt(valueString.replace(/[^\d\-.,]/g, "").replace(",", ".").replace(/\.(?=.*\.)/g, "")).valueOf(1);
  },
  moneyStringToFloat: function (value) {
      var myRe = new RegExp("[.][0-9]+$");
      if (myRe.test(value))
          return parseFloat(value.replace(",", ""));
      else
          return parseFloat((value.replace(".", "")).replace(",", "."));
  },
  time2dec: function (tIn) {
      if (typeof (tIn) == 'number')
          return tIn;
      if ((tIn == '') || (tIn == null))
          return 0;
      if (tIn.indexOf('h') >= 0 || tIn.indexOf(':') >= 0)
          return convert.hm2dec(tIn.split(/[h:]/));
      if (tIn.indexOf('m') >= 0)
          return convert.hm2dec([0, tIn.replace('m', '')]);
      if (tIn.indexOf(',') >= 0)
          return parseFloat(tIn.split(',').join('.')).toFixed(2);
      if (tIn.indexOf('.') >= 0)
          return parseFloat(tIn);
      return parseInt(tIn, 10);
  },
  hm2dec: function (hoursMinutes) {
      var hours = parseInt(hoursMinutes[0], 10);
      var minutes = hoursMinutes[1] ? parseInt(hoursMinutes[1], 10) : 0;
      return (hours + minutes / 60);
  },
  dec2hhmm: function (minutes) {
      if (minutes !== undefined) {
          if (minutes.length === 8) v = v.slice(0, -3);
          var sign = minutes < 0 ? "-" : "";
          var min = Math.floor(Math.abs(minutes));
          var sec = Math.floor((Math.abs(minutes) * 60) % 60);
          return sign + (min < 10 ? "0" : "") + min + ":" + (sec < 10 ? "0" : "") + sec;
          //var hour = parseInt(v);
          //var minute = ((v - hour) / 100 * 60).toFixed(2).substring(2, 4);
          //return ((hour.toString().length >= 2) ? '' : '0') + hour.toString() + ':' + minute;
      } return null;
  }
}

var Json = {
  formatQueryParam: function (obj) {
      var prop = null;
      var filter = '';
      for (prop in obj)
          filter += ((filter === '') ? '?' : '&') + prop + '=' + obj[prop];

      return filter;
  },

  getPath: function (data, objParam) {
      var debugMode = data.debugMode || false;
      var module = data.module || 'xsjs';
      var url = (typeof (data) === 'string') ? data : data.url;

      var path = jQuery.sap.getModulePath(module, '/' + url);

      if ((sessionStorage.getItem('debugMode') === 'true') || (debugMode)) {
          path = path.replace('.xsjs', '.json');
      } else {
          var queryParam = Json.formatQueryParam(objParam);
          path += queryParam;
      }

      return path;
  },

  getModel: function (data, objParam) {
      var path = Json.getPath(data, objParam);
      return new sap.ui.model.json.JSONModel(path);
  },

  getData: function (data, objParam) {
      //Load data with sync request
      var path = Json.getPath(data, objParam);
      var model = new sap.ui.model.json.JSONModel();
      model.loadData(path, '', false);
      return model.getData();
  },

  encodeHtml: function (element) {
      var json = true;
      var treeObject = {
      };

      // If string convert to document Node
      if (typeof element === "string") {
          if (window.DOMParser) {
              parser = new DOMParser();
              docNode = parser.parseFromString(element, "text/xml");
          } else { // Microsoft strikes again
              docNode = new ActiveXObject("Microsoft.XMLDOM");
              docNode.async = false;
              docNode.loadXML(element);
          }
          element = docNode.firstChild;
      }

      //Recursively loop through DOM elements and assign properties to object
      function treeHTML(element, object) {
          object["type"] = element.nodeName;
          var nodeList = element.childNodes;
          if (nodeList != null) {
              if (nodeList.length) {
                  object["content"] = [];
                  for (var i = 0; i < nodeList.length; i++) {
                      if (nodeList[i].nodeType == 3) {
                          object["content"].push(nodeList[i].nodeValue);
                      } else {
                          object["content"].push({
                          });
                          treeHTML(nodeList[i], object["content"][object["content"].length - 1]);
                      }
                  }
              }
          }
          if (element.attributes != null) {
              if (element.attributes.length) {
                  object["attributes"] = {
                  };
                  for (var i = 0; i < element.attributes.length; i++) {
                      object["attributes"][element.attributes[i].nodeName] = element.attributes[i].nodeValue;
                  }
              }
          }
      }

      treeHTML(element, treeObject);
      return (json) ? JSON.stringify(treeObject) : treeObject;
  }
}

var global = {
    checkAuthorization: function (url) {
        var me = this;
        var result = {};

        if ((url === null) || (url === undefined)) {
            result = { hasError: false };
        } else {
            global.request({
                type: 'GET',
                url: '/api/authorization/' + url,
                dataType: 'json',
                context: me,
                async: false,
                error: function (xhr, status, error) {
                    result = {
                        hasError: true,
                        error: xhr
                    };
                },
                success: function (response) {
                    result = {
                        hasError: false
                    };
                }
            });
        }
        return result;
    },

    loadUserAccess: function () {
        var access = sessionStorage.getItem('open-access');
        if ((access != null) && (access != '') && (access != undefined)) {
            return JSON.parse(access);
        } else return false;
    },

    requestPrintReport: (data, urlReport, redirect = false, that = false) => {
        sap.ui.core.BusyIndicator.show();

        var url = system.configuration.urlReport + urlReport;
        var request = new XMLHttpRequest();

        request.open("POST", url, true);

        request.setRequestHeader("authorization", "Basic eG5ldF9yZXBvcnRzOnJlcG9ydHNAQWRtMTI=");
        request.setRequestHeader("company", sessionStorage.getItem('open-company'));
        request.setRequestHeader("content-type", "application/json");

        request.responseType = "blob";

        request.onload = function (e) {
            if (this.status === 200) {
                var filename = "";
                var disposition = this.getResponseHeader('Content-Disposition');
                if ((disposition && disposition.indexOf('inline') !== -1) || (disposition && disposition.indexOf('attachment') !== -1)) {
                    var filenameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;
                    var matches = filenameRegex.exec(disposition);
                    if (matches != null && matches[1]) filename = matches[1].replace(/['"]/g, '');
                }
                var type = this.getResponseHeader('Content-Type');

                var blob;
                if (typeof File === 'function') {
                    try {
                        blob = new File([this.response], filename, { type: type });
                    } catch (e) { /* Edge */ }
                }
                if (typeof blob === 'undefined') {
                    blob = new Blob([this.response], { type: type });
                }

                if (typeof window.navigator.msSaveBlob !== 'undefined') {
                    // IE workaround for "HTML7007: One or more blob URLs were revoked by closing the blob for which they were created. These URLs will no longer resolve as the data backing the URL has been freed."
                    window.navigator.msSaveBlob(blob, filename);
                } else {
                    var URL = window.URL || window.webkitURL;
                    var downloadUrl = URL.createObjectURL(blob);

                    if (filename) {
                        // use HTML5 a[download] attribute to specify filename
                        var a = document.createElement("a");
                        // safari doesn't support this yet
                        if (typeof a.download === 'undefined') {
                            window.location = downloadUrl;
                        } else {
                            a.href = downloadUrl;
                            a.download = filename;
                            document.body.appendChild(a);
                            a.click();
                        }
                    } else {
                        window.location = downloadUrl;
                    }

                    setTimeout(function () { URL.revokeObjectURL(downloadUrl); }, 100); // cleanup

                    if (redirect) that.nav.to(that.toPage || that.backPage, { data: [] });
                }
            }
            else {
                sap.m.MessageBox.error('Erro ao imprimir! - ' + this.statusText);
                if (redirect) that.nav.to(that.toPage || that.backPage, { data: [] });
            }
            sap.ui.core.BusyIndicator.hide();

        };
        request.send(JSON.stringify(data));
    },

    requestReport: function (data, prop, url) {
        data.url = system.configuration.urlReport + prop;
        data.timeout = system.configuration.timeout;
        if (prop == undefined)
            prop = {
            }

        if ((prop.headerAuth === true) || (prop.headerAuth === null) || (prop.headerAuth === undefined)) {
            data.headers = data.urlLic ? {
                "content-type": "application/json"
            } : {
                    "authorization": "Bearer " + global.getToken(),
                    "content-type": "application/json",
                    "username": sessionStorage.getItem('open-username'),
                    "password": sessionStorage.getItem('open-password'),
                    "company": sessionStorage.getItem('open-company') || '',
                    "token": sessionStorage.getItem('open-token')
                };
        }
        if ((system.configuration.debugMode === true) || (prop.debugMode === true)) {
            data.type = 'GET';
            data.method = 'GET';
        }

        data.beforeSend = function (jqxhr, settings) {
            jqxhr.requestURL = data.url;
        };

        $.ajax(data);
    },

    request: function (data, prop, url) {
        data.url = data.urlLic ? data.urlLic : global.formatUrl(data.url, data.method || data.type);
        data.timeout = system.configuration.timeout;
        if (prop == undefined)
            prop = {
            }

        if ((prop.headerAuth === true) || (prop.headerAuth === null) || (prop.headerAuth === undefined)) {
            data.headers = data.urlLic ? {
                "content-type": "application/json"
            } : {
                    "authorization": "Bearer " + global.getToken(),
                    "content-type": "application/json",
                    "username": sessionStorage.getItem('open-username'),
                    "password": sessionStorage.getItem('open-password'),
                    "company": sessionStorage.getItem('open-company') || '',
                    "token": sessionStorage.getItem('open-token')
                };
        }
        if ((system.configuration.debugMode === true) || (prop.debugMode === true)) {
            data.type = 'GET';
            data.method = 'GET';
        }

        data.beforeSend = function (jqxhr, settings) {
            jqxhr.requestURL = data.url;
        };

        $.ajax(data);
    },

    formatUrl: function (url, method) {
        var hostUrl = system.configuration.url + url;
        if (system.configuration.debugMode === true) {
            var idx = hostUrl.indexOf('?');
            hostUrl = hostUrl.substring(0, (idx == -1) ? hostUrl.length : idx)
            hostUrl = '/debug/' + (hostUrl.replace('http://', '').replace('https://', '').replace(':', '-')) + '/' + method + '.json';
        }
        return hostUrl;
    },

    getToken: function () { return sessionStorage.getItem('open-token'); },

    checkAuthorization: function () { return true; }
}

function getQueryString() {
  var url = window.location.href;
  var split = url.split('?');

  var qs = split[1] || null;
  return qs;
}

function getUrlParms() {
  var url = window.location.href;
  var split = url.split('?');

  var qs = split[1] || null;

  var params = {
  }
  if (qs !== null) {
      var brk = qs.split('&');
      for (var index in brk) {
          var key = (brk[index].split('=')[0]).toLowerCase();
          var value = brk[index].split('=')[1] || null;
          params[key] = value;
      }
  }
  return params;
}

var Base64 = {
    // private property
    _keyStr: "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=",

    // public method for encoding
    encode: function (input) {
        var output = "";
        var chr1, chr2, chr3, enc1, enc2, enc3, enc4;
        var i = 0;

        input = Base64._utf8_encode(input);

        while (i < input.length) {

            chr1 = input.charCodeAt(i++);
            chr2 = input.charCodeAt(i++);
            chr3 = input.charCodeAt(i++);

            enc1 = chr1 >> 2;
            enc2 = ((chr1 & 3) << 4) | (chr2 >> 4);
            enc3 = ((chr2 & 15) << 2) | (chr3 >> 6);
            enc4 = chr3 & 63;

            if (isNaN(chr2)) {
                enc3 = enc4 = 64;
            } else if (isNaN(chr3)) {
                enc4 = 64;
            }

            output = output +
                this._keyStr.charAt(enc1) + this._keyStr.charAt(enc2) +
                this._keyStr.charAt(enc3) + this._keyStr.charAt(enc4);

        }

    return output;
    },

    // public method for decoding
    decode: function (input) {
        var output = "";
        var chr1, chr2, chr3;
        var enc1, enc2, enc3, enc4;
        var i = 0;

        input = input.replace(/[^A-Za-z0-9\+\/\=]/g, "");

        while (i < input.length) {

            enc1 = this._keyStr.indexOf(input.charAt(i++));
            enc2 = this._keyStr.indexOf(input.charAt(i++));
            enc3 = this._keyStr.indexOf(input.charAt(i++));
            enc4 = this._keyStr.indexOf(input.charAt(i++));

            chr1 = (enc1 << 2) | (enc2 >> 4);
            chr2 = ((enc2 & 15) << 4) | (enc3 >> 2);
            chr3 = ((enc3 & 3) << 6) | enc4;

            output = output + String.fromCharCode(chr1);

            if (enc3 != 64) {
                output = output + String.fromCharCode(chr2);
            }
            if (enc4 != 64) {
                output = output + String.fromCharCode(chr3);
            }

        }

        output = Base64._utf8_decode(output);

        return output;

    },

    // private method for UTF-8 encoding
    _utf8_encode: function (string) {
        string = string.replace(/\r\n/g, "\n");
        var utftext = "";

        for (var n = 0; n < string.length; n++) {

            var c = string.charCodeAt(n);

            if (c < 128) {
                utftext += String.fromCharCode(c);
            }
            else if ((c > 127) && (c < 2048)) {
                utftext += String.fromCharCode((c >> 6) | 192);
                utftext += String.fromCharCode((c & 63) | 128);
            }
            else {
                utftext += String.fromCharCode((c >> 12) | 224);
                utftext += String.fromCharCode(((c >> 6) & 63) | 128);
                utftext += String.fromCharCode((c & 63) | 128);
            }
        }
        return utftext;
    },

    // private method for UTF-8 decoding
    _utf8_decode: function (utftext) {
        var string = "";
        var i = 0;
        var c = c1 = c2 = 0;

        while (i < utftext.length) {

            c = utftext.charCodeAt(i);

            if (c < 128) {
                string += String.fromCharCode(c);
                i++;
            }
            else if ((c > 191) && (c < 224)) {
                c2 = utftext.charCodeAt(i + 1);
                string += String.fromCharCode(((c & 31) << 6) | (c2 & 63));
                i += 2;
            }
            else {
                c2 = utftext.charCodeAt(i + 1);
                c3 = utftext.charCodeAt(i + 2);
                string += String.fromCharCode(((c & 15) << 12) | ((c2 & 63) << 6) | (c3 & 63));
                i += 3;
            }
        }
        return string;
    }
}

Array.prototype.findElement = function (key, value) {
  for (var cont = 0; cont < this.length; cont++) {
      if (this[cont][key] == value)
          return this[cont];
  }
  return null;
}

Array.prototype.elementExists = function (key, value) {
  var status = false;
  var el = this.findElement(key, value);
  return (el) ? true : false;
}

FileHelper = function () {
    var progress = document.querySelector('.percent');

    function updateProgress(evt) {
        if (evt.lengthComputable) {
            var percentLoaded = Math.round((evt.loaded / evt.total) * 100);
            progress.style.width = percentLoaded + '%';
            progress.textContent = percentLoaded + '%';
        }
    };

    function errorHandler(evt) {
        switch (evt.target.error.code) {
            case evt.target.error.NOT_FOUND_ERR:
                alert('File Not Found!');
                break;
            case evt.target.error.NOT_READABLE_ERR:
                alert('File is not readable');
                break;
            case evt.target.error.ABORT_ERR:
                break; // noop
            default:
                alert('An error occurred reading this file.');
        }
    };

    function loadFile(e) {
        var list = [];
        var fileData = atob(e.target.result.substr(e.target.result.indexOf(',') + 1, e.target.result.length));

        var rows = fileData.split(/\r?\n|\r/);
        e.currentTarget.callback.fn.call(e.currentTarget.callback.scope, rows);
    };

    return {
        clearInput: function (inputId, pbId) {},

        load: function (f, callback, scope) {
            var me = this;
            if ((f !== undefined) && (f !== null)) {
                var reader = new FileReader();
                reader.callback = {
                    fn: callback,
                    scope: scope
                };
                reader.onerror = errorHandler;
                reader.onprogress = function (evt) {
                    //EXIBIR A BARRA DE CARREGAMENTO
                    if (evt.lengthComputable) {
                        var percentLoaded = Math.round((evt.loaded / evt.total) * 100);
                        data.progressId.obj().updateProgress(percentLoaded, percentLoaded + '%', true);
                    }
                }.bind(me);

                reader.onabort = function (e) {
                    alert('File read cancelled');
                };
                reader.onloadstart = function (e) {
                    data.progressId.obj().updateProgress(0, '0%', true);
                }.bind(me);

                reader.onload = (function (theFile) {
                    return loadFile;
                })(f);

                reader.readAsDataURL(f);
            }
        }
    };
}();

