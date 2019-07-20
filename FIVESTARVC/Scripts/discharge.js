/// <reference path="jquery.validate.js" />  
/// <reference path="jquery.validate.unobtrusive.js" />  
$.validator.unobtrusive.adapters.add("discharge", "lastadmitted");
$.validator.unobtrusive.adapters.addSingleVal("discharge", "date");
$.validator.addMethod("discharge", function (value, element, discharge) {
    if (value) {
        return Date.parse(value) < Date.parse(discharge.lastadmitted);
    }
    return true;
});  