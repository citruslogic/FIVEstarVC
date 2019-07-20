/// <reference path="jquery.validate.js" />  
/// <reference path="jquery.validate.unobtrusive.js" />  
$.validator.unobtrusive.adapters.add(
    'isdateafter', ['propertytested', 'allowequaldates'], function (options) {
        options.rules['isdateafter'] = options.params;
        options.messages['isdateafter'] = options.message;
    });
$.validator.addMethod("isdateafter", function (value, element, params) {
    var startdatevalue = new Date($('input[name="' + params.propertytested + '"]').val());
    var d = new Date(value.replace(/-/g, '/').replace('T', ' '));

    if (!value || !d) return true;

    if (params.allowequaldates === "True" && startdatevalue.valueOf() <= d.valueOf()) {
        return true;
    }

    if (startdatevalue < d) {
        return true;
    }

    return false;
    //return params.allowequaldates ? Date.parse(startdatevalue) <= Date.parse(value) :
    //    Date.parse(startdatevalue) < Date.parse(value);
}, '');